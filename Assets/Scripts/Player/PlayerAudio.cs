using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProximitySound : MonoBehaviour
{
    public float detectionRadius = 10f; // Radius to detect enemies
    public LayerMask enemyLayer; // Layer for the enemies

    private void Update()
    {
        DetectEnemies();
    }

    private void DetectEnemies()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, detectionRadius, enemyLayer);
        foreach (Collider2D enemy in enemies)
        {
            AudioSource enemyAudio = enemy.GetComponent<AudioSource>();
            if (enemyAudio != null)
            {
                enemyAudio.enabled = true;
            }
        }

        Collider2D[] allEnemies = Physics2D.OverlapCircleAll(transform.position, Mathf.Infinity, enemyLayer);
        foreach (Collider2D enemy in allEnemies)
        {
            if (Vector2.Distance(transform.position, enemy.transform.position) > detectionRadius)
            {
                AudioSource enemyAudio = enemy.GetComponent<AudioSource>();
                if (enemyAudio != null)
                {
                    enemyAudio.enabled = false;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
