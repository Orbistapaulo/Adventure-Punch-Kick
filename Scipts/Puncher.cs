using UnityEngine;

public class Puncher : MonoBehaviour
{
    public float speed = 10f; // Reduced speed for short-range
    public float range = 0.1f; // Shorter range
    public int damage = 40;
    public float knockbackForce = 1f;
    public float lifetime = 0.5f; // Time before the punch is destroyed

    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = transform.right * speed;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, range);
        foreach (Collider2D enemyCollider in hitEnemies)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    Vector2 knockbackDirection = (enemy.transform.position - transform.position).normalized;
                    enemyRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
                }
                Destroy(gameObject);
                break;
            }
        }
    }
     private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Blocked"))
        {
            // Destroy this punch object if it hits an object tagged as "Blocked"
            Destroy(gameObject);
        }
    }
}
