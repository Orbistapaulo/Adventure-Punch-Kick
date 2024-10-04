using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMovement : MonoBehaviour
{
    public bool moveRight;
    public float speed;
    public float chaseSpeed; // Speed when the monster is chasing the player
    public Animator animator;
    public Transform playerTransform;
    public bool isChasing;
    public float chaseDistance;
    public float jumpForce = 5f; // Force applied when the enemy jumps
    private Rigidbody2D rb; // Reference to the Rigidbody2D component
    private AudioSource audioSourceComponent; // Renamed variable to avoid conflict
    private float originalSpeed; // To store the original walking speed

    // Sound effect delay
    public float soundEffectDelay = 0.5f; // Delay before playing sound again
    private float lastSoundTime = 0f; // Track the last time a sound was played

    // Start is called before the first frame update
    void Start()
    {
        // Ensure the animator is attached to the GameObject
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        audioSourceComponent = GetComponent<AudioSource>(); // Use the new variable name
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component
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
                transform.localScale = new Vector3(4, 4);
                transform.position += Vector3.left * speed * Time.deltaTime;
                PlaySoundWithDelay(); // Play sound when moving left
            }
            else if (transform.position.x < playerTransform.position.x)
            {
                transform.localScale = new Vector3(-4, 4);
                transform.position += Vector3.right * speed * Time.deltaTime;
                PlaySoundWithDelay(); // Play sound when moving right
            }
        }
        else
        {
            speed = originalSpeed; // Reset speed to original when not chasing

            if (moveRight)
            {
                transform.Translate(2 * Time.deltaTime * speed, 0, 0);
                transform.localScale = new Vector2(-4, 4);
                PlaySoundWithDelay(); // Play walking sound
            }
            else
            {
                // Check if the player is close enough to start chasing
                if (Vector2.Distance(transform.position, playerTransform.position) < chaseDistance)
                {
                    isChasing = true;
                }

                transform.Translate(-2 * Time.deltaTime * speed, 0, 0);
                transform.localScale = new Vector2(4, 4);
                PlaySoundWithDelay(); // Play walking sound
            }

            // Trigger the walking animation
            animator.SetBool("Walk", true);
        }
    }

    void OnTriggerEnter2D(Collider2D trig)
    {
        if (trig.gameObject.CompareTag("Turn"))
        {
            // Change direction
            moveRight = !moveRight;
            PlaySoundWithDelay(); // Play walking sound
        }

        // Check if the object has the tag "pullable"
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
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); // Apply jump force
            animator.SetTrigger("Jump"); // Optionally trigger a jump animation

            // Increase the size during the jump
            float jumpScaleFactor = 5f; // Adjust this value to make the sprite bigger during the jump
            transform.localScale = new Vector3(transform.localScale.x * jumpScaleFactor, transform.localScale.y * jumpScaleFactor, transform.localScale.z);
        }
    }

    // Method to play sound with delay
    private void PlaySoundWithDelay()
    {
        if (Time.time >= lastSoundTime + soundEffectDelay) // Check if enough time has passed
        {
            audioSourceComponent.Play(); // Play sound
            lastSoundTime = Time.time; // Update the last sound time
        }
    }
}
