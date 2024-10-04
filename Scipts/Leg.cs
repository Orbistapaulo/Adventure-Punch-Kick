using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg : MonoBehaviour
{
    // Reference to the enemy script, if needed for additional functionality
    public Enemy enemy;

    // Start is called before the first frame update
    void Start()
    {
        // Optionally, you can find the Enemy component if needed
        // enemy = GetComponent<Enemy>(); // Uncomment if the Leg and Enemy are on the same GameObject
    }

    // Update is called once per frame
    void Update()
    {
        // Any logic that needs to be executed every frame can go here
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the object collided with has the "Weakpoint" tag
        if (collision.gameObject.CompareTag("Weakpoint"))
        {
            // Get the Enemy component from the collided GameObject
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Call the Die method on the Enemy script
                enemy.Die();
            }
        }
    }
}
