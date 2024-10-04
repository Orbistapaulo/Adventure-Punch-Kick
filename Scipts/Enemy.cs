using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 100;
    public GameObject deathEffect;
    public float bounceForce = 10f; // Adjust this value for bounce height
    public float scaleMultiplier = 2f; // Multiplier to make the death effect bigger
    public MonsterMovement monsterMovement;
    public Spikey movementSpike;
    public Pointy movementPoint; // Reference to the MonsterMovement script
    public float speedIncreaseFactor = 1.5f; // Multiplier for increasing speed on hit

    // Animator reference
    public Animator animator;

    // Invincibility properties
    private bool isInvincible = false;  // Track whether the enemy is invincible
    public float invincibilityDuration = 1f;  // Duration of invincibility
    public AudioManager audioManager; 
    private int playerLayer;
    private int enemyLayer;
    private int punchLayer; // Layer for the punch attack
    private int blockLayer;
    private int weakpointLayer;

    // Delay properties for looping SFX
    public float sfxDelay = 0.5f; // Delay between SFX loops
    private Coroutine sfxCoroutine;

    // Walking sound properties
    public float maxVolume = 1.0f; // Maximum volume when close to the player
    public float minVolume = 0.2f; // Minimum volume when far from the player
    public float maxDistance = 10f; // Distance at which the sound is at minimum volume
    private AudioSource walkingSFX; // Reference to the enemy walking sound source
    private Transform playerTransform; // Reference to the player transform

    void Awake()
    {
        // Initialize health and animator layer for invincibility
        health = 100; // Set initial health
        animator.SetLayerWeight(1, 0); // Ensure invincibility layer is off initially
        
        // Get layer indices for collision ignoring
        playerLayer = LayerMask.NameToLayer("Player");
        enemyLayer = LayerMask.NameToLayer("Enemy");
        punchLayer = LayerMask.NameToLayer("Punch"); // Assuming "Punch" is the name of the layer for the punch attack
        blockLayer = LayerMask.NameToLayer("Blocked");
        weakpointLayer = LayerMask.NameToLayer("Weakpoint");
        
        // Ensure MonsterMovement script is assigned
        if (monsterMovement == null)
        {
            monsterMovement = GetComponent<MonsterMovement>();
        }

        // Get a reference to the player's transform
        GameObject player = GameObject.FindGameObjectWithTag("Player"); // Assuming the player has the "Player" tag
        if (player != null)
        {
            playerTransform = player.transform;
        }

        // Get the walking sound source component (make sure to assign this in the inspector)
        walkingSFX = GetComponent<AudioSource>();
    }

    public void TakeDamage(int damage)
    {
        if (!isInvincible) // Only take damage if not invincible
        {
            health -= damage;
            audioManager.PlayHitSound();
            if (health <= 0)
            {
                Die();
            }
            else
            {
                // Start chasing the player by disabling patrol in MonsterMovement
                if (monsterMovement != null)
                {
                    monsterMovement.isChasing = true; // Enable chasing
                    
                    // Increase the monster's chase speed after getting hit
                    monsterMovement.chaseSpeed *= speedIncreaseFactor;
                }

                StartCoroutine(InvincibilityCoroutine()); // Start invincibility after taking damage
            }
        }
    }

    private void PlaySfxWithDelay(System.Action sfxAction)
    {
        if (sfxCoroutine != null)
        {
            StopCoroutine(sfxCoroutine); // Stop previous SFX coroutine if it's running
        }
        sfxCoroutine = StartCoroutine(PlaySfxCoroutine(sfxAction));
    }

    private IEnumerator PlaySfxCoroutine(System.Action sfxAction)
    {
        while (true)
        {
            sfxAction.Invoke(); // Play the sound effect
            yield return new WaitForSeconds(sfxDelay); // Wait for the specified delay
        }
    }

    // Coroutine to handle invincibility duration
    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true; // Set enemy to invincible
        animator.SetLayerWeight(1, 1); // Activate blinking or invincibility animation layer

        // Ignore collisions with player, other enemies, and punch attacks
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
        Physics2D.IgnoreLayerCollision(punchLayer, enemyLayer, true); // Ignore collisions with punch attacks
        Physics2D.IgnoreLayerCollision(playerLayer, blockLayer, true); 
        Physics2D.IgnoreLayerCollision(playerLayer, weakpointLayer, true); 
        yield return new WaitForSeconds(invincibilityDuration); // Wait for the duration

        animator.SetLayerWeight(1, 0); // Deactivate blinking or invincibility animation layer
        isInvincible = false; // Set enemy back to vulnerable

        // Re-enable collisions with player, other enemies, and punch attacks
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
        Physics2D.IgnoreLayerCollision(punchLayer, enemyLayer, false); 
        Physics2D.IgnoreLayerCollision(playerLayer, blockLayer, true); 
        Physics2D.IgnoreLayerCollision(playerLayer, weakpointLayer, true); // Re-enable collisions with punch attacks
    }

    // Function to allow enemies to pass through each other when they collide
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if colliding with the Leg object
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Blocked") || collision.gameObject.CompareTag("Weakpoint")) // Check if colliding with another enemy
        {
            StartCoroutine(TemporaryPassThrough());
        }
    }

    private IEnumerator TemporaryPassThrough()
    {
        // Ignore collisions between enemies
        Physics2D.IgnoreLayerCollision(enemyLayer, enemyLayer, true);
        Physics2D.IgnoreLayerCollision(enemyLayer, blockLayer, true);
        Physics2D.IgnoreLayerCollision(enemyLayer, weakpointLayer, true);
        yield return new WaitForSeconds(1000f); // Adjust the time to allow passing through

        // Re-enable collisions between enemies
        // (This line seems to be missing in the original code. Consider adding if necessary.)
    }

    public void Die()
    {
        audioManager.PlayMonsterDeathSFX();
        // Instantiate the death effect
        GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);

        // Set the scale of the instantiated effect to be bigger
        effect.transform.localScale *= scaleMultiplier; // Scale it up by the multiplier

        // Get the Rigidbody2D component from the death effect prefab
        Rigidbody2D effectRb = effect.GetComponent<Rigidbody2D>();

        // Apply a bounce force if the Rigidbody2D is present
        if (effectRb != null)
        {
            effectRb.velocity = new Vector2(0, bounceForce); // Add upward force
            effectRb.gravityScale = 3f;
        }

        // Destroy the enemy object
        Destroy(gameObject);
    }

    public void ReenableCollisions()
    {
        // Re-enable collisions with player and other enemies
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
        Physics2D.IgnoreLayerCollision(punchLayer, enemyLayer, false);
    }

    private void Update()
    {
        AdjustEnemySoundVolume(); // Call the method to adjust the sound volume
    }

    private void AdjustEnemySoundVolume()
    {
        if (playerTransform != null && walkingSFX != null)
        {
            float distance = Vector2.Distance(transform.position, playerTransform.position);
            float volume = Mathf.Lerp(maxVolume, minVolume, distance / maxDistance);
            walkingSFX.volume = volume; // Set the volume based on distance
        }
    }
}
