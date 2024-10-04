using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; // For using Action (events)

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogueScript;
    private bool playerDetected;

    // Declare an event for when dialogue starts
    public event Action OnDialogueStart;

    // Detect trigger with player
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If we triggered the player, enable playerDetected and show the indicator
        if (collision.tag == "Player")
        {
            playerDetected = true;
            dialogueScript.ToggleIndicator(playerDetected);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // If we lost the trigger with the player, disable playerDetected and hide the indicator
        if (collision.tag == "Player")
        {
            playerDetected = false;
            dialogueScript.ToggleIndicator(playerDetected);
            dialogueScript.EndDialogue();
        }
    }

    // While detected, if we interact, start the dialogue
    private void Update()
    {
        if (playerDetected && Input.GetKeyDown(KeyCode.E))
        {
            // Trigger the dialogue start
            dialogueScript.StartDialogue();
            
            // Invoke the OnDialogueStart event to notify subscribers (like AudioManager)
            OnDialogueStart?.Invoke();
        }
    }
}
