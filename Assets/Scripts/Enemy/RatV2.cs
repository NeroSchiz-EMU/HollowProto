using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatV2 : MonoBehaviour
{
    public float detectionRadius = 10f; // Radius to detect the player
    public float speed = 2f; // Speed of the rat
    public LayerMask playerLayer; // Layer for the player

    private Transform playerTransform;
    private bool playerNearby = false;

    private void Update()
    {
        DetectPlayer();
        if (playerNearby)
        {
            ChasePlayer();
        }
    }

    private void DetectPlayer()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
        if (player != null)
        {
            playerTransform = player.transform;
            playerNearby = true;
        }
        else
        {
            playerNearby = false;
        }
    }

    private void ChasePlayer()
    {
        if (playerTransform != null)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            Vector2 newPosition = Vector2.MoveTowards(transform.position, new Vector2(playerTransform.position.x, transform.position.y), speed * Time.deltaTime);
            transform.position = newPosition;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
