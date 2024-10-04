using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;
    public float pushForce = 10f; // Adjust the push force as needed
    public float pullForce = 1f;  // Adjust the pull force as needed
    public float normalSpeed = 5f; // The player's normal movement speed
    public float pullSpeed = 2f;   // The reduced speed while pulling
    public LayerMask obstacleLayer; // Define which objects are considered obstacles
    private bool isKicking = false;
    private bool isAttacking = false;
    private float kickCooldown = 0.5f;
    private float lastKickTime;
    private float attackCooldown = 0.5f;
    private float lastAttackTime;
    public PlayerMovement controller; // Reference to the PlayerMovement script
    private Rigidbody2D rb; // Track pulling/pushing state
    private bool canPull = false;  // Track if the player can pull an object
    private bool canPush = false;  // Track if the player can push an object
    private float horizontalMovement; // Track horizontal movement
    public Transform AttackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public GameObject Punch;
    public Transform Kickpoint;
    public int attackDamage = 40;
    public float knockbackForce = 500f;
    private AudioManager audioManager;
    void Start()
    {
        lastKickTime = -kickCooldown;
        lastAttackTime = -attackCooldown;
        rb = GetComponent<Rigidbody2D>();
    audioManager = FindObjectOfType<AudioManager>();
        // Check if essential components are assigned
        if (controller == null)
            Debug.LogError("PlayerMovement controller is not assigned.");
        if (animator == null)
            Debug.LogError("Animator is not assigned.");
        if (Kickpoint == null)
            Debug.LogError("Kickpoint is not assigned.");
        if (AttackPoint == null)
            Debug.LogError("AttackPoint is not assigned.");
    }

    void Update()
    {
        // Update horizontal movement value
        if (controller != null)
            horizontalMovement = controller.GetHorizontalMovement();
        else
            Debug.LogError("PlayerMovement controller is null in Update.");

        // Check for nearby objects that can be pulled or pushed
        CheckForPullableObject();

        // Handle attack input
        if (Input.GetKeyDown(KeyCode.J) && !isAttacking && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
        }

        // Handle kick input
        if (Input.GetKeyDown(KeyCode.L) && !isKicking && Time.time >= lastKickTime + kickCooldown)
        {
            Kick();
        }

        // Handle push/pull input
        if (Input.GetKey(KeyCode.K))
        {
            if (canPush && IsPlayerMovingTowardsObject()) // Push
            {
                StartPush();
            }
            else if (canPull && !IsPlayerMovingTowardsObject()) // Pull
            {
                StartPull();
            }
        }
        else
        {
            StopPush();
            StopPull();
        }
    }

    void Kick()
{
    if (Kickpoint == null)
    {
        Debug.LogError("Kickpoint is not assigned in Kick method.");
        return;
    }

    if (animator != null)
    {
        animator.SetTrigger("Kick");
          audioManager.PlayKickSound();
    }
    else
    {
        Debug.LogError("Animator is null in Kick method.");
    }

    isKicking = true;

    // Find enemies within the kick range
    Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(Kickpoint.position, attackRange, enemyLayers);
    
    foreach (Collider2D enemyCollider in hitEnemies)
    {
        // Check if the object has the "Enemy" tag
        if (enemyCollider.CompareTag("Enemy")||enemyCollider.CompareTag("Weakpoint"))
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage); // Apply damage to the enemy
            }
            else
            {
                Debug.LogWarning("Enemy does not have the Enemy component attached.");
            }

            Rigidbody2D enemyRb = enemyCollider.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                Vector2 knockbackDirection = (enemyCollider.transform.position - transform.position).normalized;
                enemyRb.AddForce(knockbackDirection * knockbackForce);
            }
        }
        else
        {
            Debug.LogWarning("The kicked object is not tagged as 'Enemy'.");
        }
    }

    lastKickTime = Time.time;
    StartCoroutine(ResetKickState());
}


    void Attack()
    {
        if (isAttacking) return;
        
        if (animator != null)
        {
            animator.SetTrigger("Attack");
             audioManager.PlayAttackSound();
        }
        else
        {
            Debug.LogError("Animator is null in Attack method.");
        }

        isAttacking = true;
        lastAttackTime = Time.time;

        if (AttackPoint != null && Punch != null)
        {
            Instantiate(Punch, AttackPoint.position, AttackPoint.rotation);
        }
        else
        {
            Debug.LogError("Punch or AttackPoint is not assigned in Attack method.");
        }

        StartCoroutine(ResetAttackState());
    }

    bool IsPlayerMovingTowardsObject()
    {
        foreach (Collider2D obstacle in Physics2D.OverlapCircleAll(transform.position, 0.75f, obstacleLayer))
        {
            float objectPositionX = obstacle.transform.position.x;
            float playerPositionX = transform.position.x;
            if ((playerPositionX < objectPositionX && horizontalMovement > 0) || 
                (playerPositionX > objectPositionX && horizontalMovement < 0))
            {
                return true; // Player is moving towards the object
            }
        }
        return false; // Player is moving away from the object
    }

    void StartPush()
    {
        if (animator != null)
        {
            animator.SetBool("Pushing", true);
        
        }

        Collider2D[] obstacles = Physics2D.OverlapCircleAll(transform.position, 0.75f, obstacleLayer);

        foreach (Collider2D obstacle in obstacles)
        {
            // Check if the object is tagged as "Pullable"
            if (obstacle.CompareTag("Pullable"))
            {
                Rigidbody2D obstacleRb = obstacle.GetComponent<Rigidbody2D>();
                if (obstacleRb != null)
                {
                    Vector2 pushDirection = new Vector2(horizontalMovement, 0f).normalized;
                    obstacleRb.AddForce(pushDirection * pushForce);
                }
            }
        }
    }

    void StartPull()
    {
        if (gameObject.CompareTag("Player"))
        {
            if (animator != null)
            {
                animator.SetBool("Pulling", true);
              
            }

            controller.SetSpeed(pullSpeed);
            controller.SetPulling(true);

            Collider2D[] obstacles = Physics2D.OverlapCircleAll(transform.position, 0.75f, obstacleLayer);

            foreach (Collider2D obstacle in obstacles)
            {
                // Check if the object is tagged as "Pullable"
                if (obstacle.CompareTag("Pullable"))
                {
                    Rigidbody2D obstacleRb = obstacle.GetComponent<Rigidbody2D>();
                    if (obstacleRb != null)
                    {
                        StartCoroutine(DragObjectWithPlayer(obstacleRb));
                    }
                }
            }

            controller.DisableFlipping(true); // Disable character flipping when pulling
        }
    }

    IEnumerator DragObjectWithPlayer(Rigidbody2D pulledObjectRb)
    {
        Vector2 offset = pulledObjectRb.transform.position - transform.position;

        while (Input.GetKey(KeyCode.K) && !IsPlayerMovingTowardsObject())
        {
            if (pulledObjectRb != null)
            {
                Vector2 targetPosition = (Vector2)transform.position + offset;
                pulledObjectRb.MovePosition(targetPosition);
            }
            yield return null;
        }
        StopPull();
    }

    void StopPull()
    {
        if (animator != null)
        {
            animator.SetBool("Pulling", false);
        }

        controller.DisableFlipping(false);
        controller.SetSpeed(normalSpeed);
        controller.SetPulling(false);
    }

    void StopPush()
    {
        if (animator != null)
        {
            animator.SetBool("Pushing", false);
        }
    }

    void CheckForPullableObject()
    {
        Collider2D[] obstacles = Physics2D.OverlapCircleAll(transform.position, 0.75f, obstacleLayer);
        canPull = false;
        canPush = false;

        foreach (Collider2D obstacle in obstacles)
        {
            if (obstacle.GetComponent<Rigidbody2D>() != null)
            {
                canPull = true;
                canPush = true;
                break; // Exit early once an object is found
            }
        }
    }

    IEnumerator ResetKickState()
    {
        yield return new WaitForSeconds(kickCooldown);
        isKicking = false;
    }

    IEnumerator ResetAttackState()
    {
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        if (AttackPoint == null) return;
        Gizmos.DrawWireSphere(AttackPoint.position, attackRange);
    }
}
