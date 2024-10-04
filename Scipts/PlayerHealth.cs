using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public Image[] hearts;
    public static int health = 3;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    public Animator animator;

    // Bounce and death effect properties
    public float bounceForce = 20f; 
    public float scaleMultiplier = 2f; 
    private Rigidbody2D rb;
    private bool hasBounced = false;
    private bool isDead;

    // Invincibility properties
    private bool isInvincible = false;  
    public float invincibilityDuration = 3f;  

    // Layer references
    private int playerLayer;
    private int enemyLayer;
    private Vector3 respawnPoint;

    // Time before the player is destroyed after falling
    public float fallDestroyDelay = 3f; 

    // Child collider reference (e.g., for "Leg" object)
    private Collider2D legCollider;

    // Weakpoint collision tracking
    private bool isWeakpointColliding = false;
      public AudioManager audioManager;
    void Awake()
    {
        health = 3;
        playerLayer = LayerMask.NameToLayer("Player");
        enemyLayer = LayerMask.NameToLayer("Enemy");
        animator.SetLayerWeight(1, 0);

        // Find the leg collider
        legCollider = transform.Find("Leg").GetComponent<Collider2D>();  // Assuming the Leg object is named "Leg"
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>(); 
        respawnPoint = transform.position;
    }

    void Update()
    {
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        foreach (Image img in hearts)
        {
            img.sprite = emptyHeart;
        }
        for (int i = 0; i < health; i++)
        {
            hearts[i].sprite = fullHeart;
        }
    }

    public void TakeDamage(int damage)
    {
        if (!isInvincible && !isDead && !isWeakpointColliding)  // Added weakpoint check here
        {
            health -= damage;
              audioManager.PlayDamageSound();
             
            if (damage > 0) 
            {
                StartCoroutine(InvincibilityCoroutine()); 
            }

            if (health <= 0)
            {
                isDead = true;
                PlayerManager.isGameOver = true;
                Die(); 
            }
        }
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;

        Debug.Log($"Ignoring collision between layers: {playerLayer} and {enemyLayer}");

        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
        
        var enemy = GameObject.FindGameObjectWithTag("Enemy");
        if (enemy != null)
        {
            Physics2D.IgnoreCollision(legCollider, enemy.GetComponent<Collider2D>(), true);
        }
      

        animator.SetLayerWeight(1, 1);

        yield return new WaitForSeconds(invincibilityDuration);

        isInvincible = false;

        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
        
        if (enemy != null)
        {
            Physics2D.IgnoreCollision(legCollider, enemy.GetComponent<Collider2D>(), false);
        }

        animator.SetLayerWeight(1, 0);
    }

    private void Die()
    {
        animator.SetBool("Knockedback", true);
         audioManager.PlayPlayerDeathSound();
           audioManager.StopBackgroundMusic();
        if (rb != null)
        {
            rb.velocity = new Vector2(0, bounceForce);
            rb.gravityScale = 5f;
        }
        
        StartCoroutine(FallAndDestroy());
    }

    private IEnumerator FallAndDestroy()
    {
        yield return new WaitForSeconds(fallDestroyDelay);
        Destroy(gameObject);
    }

    private void Respawn()
    {
        health = 3;
        transform.position = respawnPoint;
        animator.SetBool("Knockedback", false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("FallDeath") && !hasBounced)
        {
            hasBounced = true; 
            PlayerManager.isGameOver = true;
            StartCoroutine(DelayedDeath());
        }

        // Check if collided with a Weakpoint
        if (collision.gameObject.CompareTag("Weakpoint"))
        {
            isWeakpointColliding = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Reset the weakpoint collision status when exiting
        if (collision.gameObject.CompareTag("Weakpoint"))
        {
            isWeakpointColliding = false;
        }
    }

    private IEnumerator DelayedDeath()
    {
        if (rb != null)
        {
            rb.velocity = new Vector2(0, bounceForce);
        }

        animator.SetBool("Knockedback", true); 

        TakeDamage(health); 

        yield return new WaitForSeconds(1.5f);
        
        Die(); 
    }
}
