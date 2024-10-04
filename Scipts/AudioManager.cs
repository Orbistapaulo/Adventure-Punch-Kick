using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource; // Plays background music
    [SerializeField] AudioSource sfxSource;   // Plays sound effects
    [SerializeField] AudioSource dialogueSource;
    [SerializeField] AudioSource enemySFX; // Plays enemy sound effects
    public DialogueTrigger dialogue;
    public PlayerMovement playerMovement; // Reference to player movement script

    public AudioClip background;
    public AudioClip jump;
    public AudioClip message;
    public AudioClip attackSFX; // Sound effect for attack
    public AudioClip kickSFX;    // Sound effect for kick
    public AudioClip damageSFX;  // Sound effect for taking damage
    public AudioClip playerDeathSFX; // Sound effect for player death
    public AudioClip gameCompleteSFX;
    [Header("---------------Enemy-----------------")]
    public AudioClip slimeJumpSFX;
    public AudioClip MonsterHitSFX;
    public AudioClip MonsterDeathSFX;

    private bool isHitRecently; // Flag to track if the enemy was hit recently
    private bool hasPlayedVictorySound = false; // Flag to ensure victory sound is played only once

    void Start()
    {
        // Set and play the background music at the start
        if (musicSource != null && background != null)
        {
            musicSource.clip = background;
            musicSource.loop = true; // Loop the background music
            musicSource.Play();
        }
        else
        {
            Debug.LogError("Music source or background clip is not assigned.");
        }

        // Subscribe to the OnDialogueStart event from DialogueTrigger if not null
        if (dialogue != null)
        {
            dialogue.OnDialogueStart += PlayMessageSound;
        }
        else
        {
            Debug.LogError("DialogueTrigger is not assigned.");
        }

        // Subscribe to the OnJump event from PlayerMovement if not null
        if (playerMovement != null)
        {
            playerMovement.OnJump += PlayJumpSound;
        }
        else
        {
            Debug.LogError("PlayerMovement is not assigned.");
        }
    }

    public void StopBackgroundMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

   public void PlayGameCompleteSound()
{
    // Ensure victory sound is played only once
    if (!hasPlayedVictorySound && gameCompleteSFX != null)
    {
        // Stop any currently playing music or sound
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
        
        // Assign the gameCompleteSFX and ensure it's not set to loop
        musicSource.loop = false; // Ensure it doesn't loop
        musicSource.clip = gameCompleteSFX;
        musicSource.Play();

        hasPlayedVictorySound = true; // Set the flag to prevent replaying
    }
}


    void PlayMessageSound()
    {
        // Play message sound when dialogue starts
        dialogueSource.clip = message; // Use dialogueSource for dialogue sounds
        dialogueSource.Play();
    }

    void PlayJumpSound()
    {
        // Play jump sound
        sfxSource.clip = jump;
        sfxSource.Play();
    }

    // Method to play attack sound effect
    public void PlayAttackSound()
    {
        sfxSource.clip = attackSFX;
        sfxSource.Play();
    }

    // Method to play kick sound effect
    public void PlayKickSound()
    {
        sfxSource.clip = kickSFX;
        sfxSource.Play();
    }

    // Method to play damage sound effect
    public void PlayDamageSound()
    {
        sfxSource.clip = damageSFX;
        sfxSource.Play();
    }

    // Method to play player death sound effect
    public void PlayPlayerDeathSound()
    {
        sfxSource.clip = playerDeathSFX;
        sfxSource.Play();
    }

    public void PlaySlimeJumpSound()
    {
        enemySFX.clip = slimeJumpSFX;
        enemySFX.Play();
    }

    public void PlayMonsterDeathSFX()
    {
        AudioSource.PlayClipAtPoint(MonsterDeathSFX, Camera.main.transform.position);
    }

    public void PlayHitSound()
    {
        // Check if the enemy was not hit recently
        if (!isHitRecently)
        {
            enemySFX.clip = MonsterHitSFX;
            enemySFX.Play();
            isHitRecently = true; // Set the flag to true to indicate the enemy was hit
            StartCoroutine(ResetHitFlag()); // Start coroutine to reset the hit flag
        }
    }

    private IEnumerator ResetHitFlag()
    {
        // Wait for a short duration before allowing the hit sound to play again
        yield return new WaitForSeconds(0.5f); // Adjust the duration as needed
        isHitRecently = false; // Reset the hit flag
    }

    void OnDestroy()
    {
        if (dialogue != null)
        {
            dialogue.OnDialogueStart -= PlayMessageSound;
        }

        if (playerMovement != null)
        {
            playerMovement.OnJump -= PlayJumpSound; // Unsubscribe from OnJump event
        }
    }
}
