using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    
    public Transform LevelStart;
    private Vector2 respawnPoint;

    private void Start()
    {
        respawnPoint = transform.position;
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
        }
    }
}
