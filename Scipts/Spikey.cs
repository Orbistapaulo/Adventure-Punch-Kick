using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikey : MonoBehaviour
{
    public bool moveRight;
    public float speed;
    public float chaseSpeed; 
    public Animator animator;
    public Transform playerTransform;
    public bool isChasing;
    public float chaseDistance;
    public float jumpForce = 5f; // Force applied when the enemy jumps
    private Rigidbody2D rb;
    private float originalSpeed; // To store the original walking speed
    private AudioSource audioSourceComponent;
    public float soundEffectDelay = 0.5f; // Delay before playing sound again
    private float lastSoundTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        // Ensure the animator is attached to the GameObject
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    audioSourceComponent = GetComponent<AudioSource>();
        // Get the Rigidbody2D component for physics-based movement
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D not found on the enemy. Ensure the Rigidbody2D component is attached.");
        }

        originalSpeed = speed; // Store the initial speed
    }

    // Update is called once per frame
    void Update()
    {
        if (isChasing)
        {
            speed = chaseSpeed; // Increase speed when chasing

            if (transform.position.x > playerTransform.position.x)
            {
                transform.localScale = new Vector3(13, 13);
                transform.position += Vector3.left * speed * Time.deltaTime;
                  PlaySoundWithDelay();
            }
            else if (transform.position.x < playerTransform.position.x)
            {
                transform.localScale = new Vector3(-13, 13);
                transform.position += Vector3.right * speed * Time.deltaTime;
                  PlaySoundWithDelay();
            }
        }
        else
        {
            speed = originalSpeed; // Reset speed to original when not chasing

            if (moveRight)
            {
                transform.Translate(2 * Time.deltaTime * speed, 0, 0);
                transform.localScale = new Vector2(-13, 13);
                  PlaySoundWithDelay();
            }
            else
            {
                // Check if the player is close enough to start chasing
                if (Vector2.Distance(transform.position, playerTransform.position) < chaseDistance)
                {
                    isChasing = true;
                }

                transform.Translate(-2 * Time.deltaTime * speed, 0, 0);
                transform.localScale = new Vector2(13, 13);
                  PlaySoundWithDelay();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D trig)
    {
        if (trig.gameObject.CompareTag("Turn"))
        {
              PlaySoundWithDelay();
            // Change direction
            moveRight = !moveRight;
        }

        // Check if the object has the tag "Pullable"
        if (trig.CompareTag("Pullable"))
        {
            Jump(); // Trigger the jump
        }
    }

    // Method to make the enemy jump
    void Jump()
    {
        if (rb != null)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); // Apply vertical jump force
            animator.SetTrigger("Jump"); // Optionally trigger a jump animation
        }
    }
      private void PlaySoundWithDelay()
    {
        if (Time.time >= lastSoundTime + soundEffectDelay) // Check if enough time has passed
        {
            audioSourceComponent.Play(); // Play sound
            lastSoundTime = Time.time; // Update the last sound time
        }
    }
}
