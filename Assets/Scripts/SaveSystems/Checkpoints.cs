using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoints : MonoBehaviour
{
    //public Transform LevelStart;
    private Vector2 respawnPoint;

    //SpriteRenderer spriteRenderer;
    //public Sprite passive, active;


    private void Start()
    {
        respawnPoint = transform.position;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "RespawnPoint")
        {
            //spriteRenderer.sprite = active;
            transform.position = respawnPoint;
        }
        else if (collision.tag == "Checkpoint")
        {
            //spriteRenderer.sprite = active;
            respawnPoint = transform.position;
        }
    }
}
