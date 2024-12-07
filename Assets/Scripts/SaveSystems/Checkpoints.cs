using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Checkpoints : MonoBehaviour
{
    [SerializeField] AudioClip lightTurnOn;
    [SerializeField] Light2D checkpointLight;  // Reference to the 2D light
    private Vector2 respawnPoint;
    private bool lightActivated = false; // Flag to track if the light has been activated

   

    private void Start()
    {
        respawnPoint = transform.position;
        if (checkpointLight != null)
        {
            checkpointLight.enabled = false; // Ensure the light is off at the start
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "RespawnPoint")
        {
            transform.position = respawnPoint;
        }
        else if (collision.tag == "Checkpoint")
        {
            respawnPoint = transform.position;

            if (checkpointLight != null && !lightActivated)
            {
                SoundFXManager.instance.PlaySoundFXClip(lightTurnOn, transform, 1f);
                checkpointLight.enabled = true; // Turn on the light when the player reaches the checkpoint
                lightActivated = true; // Set the flag to true
            }
        }
        else if (collision.tag == "Player")
        {
            if (checkpointLight != null && !lightActivated)
            {
                SoundFXManager.instance.PlaySoundFXClip(lightTurnOn, transform, 1f);
                checkpointLight.enabled = true; // Turn on the light when the player enters the light
                lightActivated = true; // Set the flag to true
            }
        }
    }
}


//  ----OLD CODE----
//  code that i may need if something breaks
//
//public UnityEngine.Rendering.Universal.Light2D checkpointLight; 