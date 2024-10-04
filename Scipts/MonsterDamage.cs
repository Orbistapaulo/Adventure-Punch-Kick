using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDamage : MonoBehaviour
{
    public int damage;
    public PlayerHealth playerHealth;
    public PlayerMovement playerMovement;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
 private void OnCollisionEnter2D(Collision2D collision)
{
    if (collision.gameObject.CompareTag("Player"))
    {
        // Check if the player is hitting a Weakpoint
        if (collision.transform.position.x <= transform.position.x)
        {
            playerMovement.KnockFromRight = true;
        }
        else
        {
            playerMovement.KnockFromRight = false;
        }

        // Check if the collision with the player is not with a Weakpoint
        if (!collision.otherCollider.CompareTag("Weakpoint"))
        {
            playerMovement.KBCounter = playerMovement.KBTotalTime;
            playerHealth.TakeDamage(damage);
        }
    }
}


}
