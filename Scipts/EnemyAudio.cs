using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAudio : MonoBehaviour
{
     [SerializeField] AudioSource musicSource;
     public AudioClip background;
    void Start()
    {
          musicSource.clip = background;
        // Loop the background music
        musicSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
