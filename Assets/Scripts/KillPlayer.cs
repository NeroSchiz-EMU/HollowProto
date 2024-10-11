using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KillPlayer : MonoBehaviour
{
   [SerializeField] AudioClip deathSoundClip;
   public GameObject player;
   public Transform respawnPoint;
   public void OnCollisionEnter2D(Collision2D other)
   {
        if (other.gameObject.CompareTag("Player"))
        {
            SoundFXManager.instance.PlaySoundFXClip(deathSoundClip, transform, 1f);
            player.GetComponent<Health>().SetHealth(0);
        }
   }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SoundFXManager.instance.PlaySoundFXClip(deathSoundClip, transform, 1f);
            player.GetComponent<Health>().SetHealth(0);
        }
    }
}
