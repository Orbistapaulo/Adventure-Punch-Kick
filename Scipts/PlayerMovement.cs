using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class PlayerMovement : MonoBehaviour
{
    [Range(0, .3f)][SerializeField] private float m_MovementSmoothing = .05f;
    [SerializeField] private float m_JumpForce = 400f;  // Jump force variable
    private Rigidbody2D m_Rigidbody2D;
    private float horizontal;
    private float speed = 5f;  // Default walking speed
    private bool isFacingRight = true;
    private Vector3 m_Velocity = Vector3.zero;  // For smoothing
    public Animator animator;
    public event Action OnJump;
    [SerializeField] private float runSpeedMultiplier = 1.5f;  // Run speed multiplier
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    public GameObject gameComplete;
    private bool isJumping = false;  // Track jumping state
    public bool canFlip = true;  // Control whether the player can flip or not
    private bool canMove = true; // Control whether the player can move
    public bool isPulling = false; // Track pulling state (added)
    public float KBForce;
    public float KBVerticalForce = 5f; // Vertical force applied during knockback (for bouncing)
    public float KBCounter;
    public float KBTotalTime;
    public bool KnockFromRight;
    bool freezeInput;
    bool freezePlayer;
    private bool isKnockedback = false; // To keep track if the player is still in knockback state
  public AudioManager audioManager;
    void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();  // Ensure m_Rigidbody2D is initialized
    }

   void Update()
{
    if (!canMove || isKnockedback) return;

    horizontal = Input.GetAxisRaw("Horizontal");
    animator.SetFloat("Speed", Mathf.Abs(horizontal));

    if (!isPulling && !isKnockedback && Input.GetButtonDown("Jump") && IsGrounded())
    {
        m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        isJumping = true;
        animator.SetBool("Takeoff", true);

        OnJump?.Invoke();
    }

    if (Input.GetButtonUp("Jump") && m_Rigidbody2D.velocity.y > 0f)
    {
        m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, m_Rigidbody2D.velocity.y * 0.5f);
    }

    // Check if the player is falling
    if (m_Rigidbody2D.velocity.y < 0f && !IsGrounded())
    {
         animator.SetBool("Takeoff", false);
        animator.SetBool("isJumping", true);  // Trigger the falling animation
    }
    else if (IsGrounded())
    {
        animator.SetBool("isJumping", false);  // Stop the falling animation when grounded
    }

    if (canFlip)
    {
        Flip();
    }
}

    private void FixedUpdate()
    {
        // If knockback counter is active, handle knockback movement and bouncing
        if (KBCounter > 0)
        {
            isKnockedback = true; // Mark player as knockedback
            animator.SetBool("Knockedback", true); // Activate knockback animation

            // Apply knockback force based on direction and add vertical bouncing force
            if (KnockFromRight)
            {
                m_Rigidbody2D.velocity = new Vector2(-KBForce, KBVerticalForce);
            }
            else
            {
                m_Rigidbody2D.velocity = new Vector2(KBForce, KBVerticalForce);
            }

            // Decrease the knockback counter
            KBCounter -= Time.deltaTime;
        }
        else
        {
            // Keep the knockback animation active until the player hits the ground
            if (isKnockedback && !IsGrounded())
            {
                animator.SetBool("Knockedback", true); // Keep knockback animation active
            }
            else if (isKnockedback && IsGrounded())
            {
                isKnockedback = false;  // Reset knockback state
                animator.SetBool("Knockedback", false);  // Stop knockback animation
            }

            // Regular movement logic when not being knocked back
            if (!canMove) return; // Skip movement update if player cannot move

            // Determine the current speed (running or walking)
            float currentSpeed = speed;

            // Check if running key (e.g., Left Shift) is pressed
            if (Input.GetKey(KeyCode.LeftShift))
            {
                currentSpeed *= runSpeedMultiplier;  // Multiply speed by run multiplier
            }

            // Get horizontal input only if not knockbacked
            horizontal = Input.GetAxisRaw("Horizontal");
            animator.SetFloat("Speed", Mathf.Abs(horizontal));

            // Calculate target velocity with the current speed
            Vector3 targetVelocity = new Vector2(horizontal * currentSpeed, m_Rigidbody2D.velocity.y);

            // Apply smoothing to the character movement
            m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
        }

        // Check if the player has landed
         if (IsGrounded())
    {
        if (isJumping)
        {
            isJumping = false;
            animator.SetBool("isJumping", false);  // Stop the jumping animation
             // Stop the falling animation upon landing
        }
    }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
{
    // Check if a flip is needed based on movement direction
    if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
    {
        // Flip the character and its child objects
        isFacingRight = !isFacingRight;

        // Flip the parent object
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;

        // Flip all child objects
        foreach (Transform child in transform)
        {
            if (child != null)
            {
                child.Rotate(0f, 180f, 0f);
            }
        }
    }
}


    // Method to enable or disable flipping (called from PlayerCombat script)
    public void DisableFlipping(bool disable)
    {
        canFlip = !disable;  // Set whether the player can flip based on input from PlayerCombat
    }

    // Method to enable or disable movement (called from PlayerCombat script)
    public void SetMovement(bool enable)
    {
        canMove = enable;  // Set whether the player can move
    }

    // Method to set the speed (used when pulling objects)
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;  // Update the player's speed
    }

    // Method to set the pulling state (called from PlayerCombat)
    public void SetPulling(bool pulling)
    {
        isPulling = pulling;  // Update pulling state
    }

    // Method to return the current horizontal movement value
    public float GetHorizontalMovement()
    {
        return horizontal;
    }

  private void OnTriggerEnter2D(Collider2D collision)
{
    if (collision.tag == "Win")
    {
        // Play the game completion sound
        audioManager.PlayGameCompleteSound();
        
        // Show the game complete UI
        gameComplete.gameObject.SetActive(true);
        
        // Freeze the game by setting time scale to 0
        Time.timeScale = 0;

        // Disable player movement
        canMove = false;

        // Optionally, disable player flipping as well if needed
        canFlip = false;
    }
}

}
