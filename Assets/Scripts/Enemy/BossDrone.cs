using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BossDrone : MonoBehaviour
{
    public Win winScript;
    public GameObject enemyPrefab; // The enemy prefab to spawn
    public Transform[] spawnPoints; // Array of spawn points
    public float spawnRate = 5f; // Time interval between spawns
    public float detectionRadius = 10f; // Radius to detect the player
    public LayerMask playerLayer; // Layer for the player

    private bool playerNearby = false;

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    private void Update()
    {
        DetectPlayer();
    }

    private void DetectPlayer()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
        playerNearby = player != null;
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnRate);
            if (playerNearby)
            {
                SpawnEnemy();
            }
        }
    }

    private void SpawnEnemy()
    {
        if (spawnPoints.Length > 0)
        {
            int spawnIndex = Random.Range(0, spawnPoints.Length);
            Instantiate(enemyPrefab, spawnPoints[spawnIndex].position, Quaternion.identity);
        }
    }

    private void OnDestroy()
    {
        if (winScript != null)
        {
            winScript.OnBossDroneDestroyed();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}



//public class BossDrone : MonoBehaviour
//{
//    public Win winScript;
//    public GameObject enemyPrefab; // The enemy prefab to spawn
//    public Transform[] spawnPoints; // Array of spawn points
//    public float spawnRate = 5f; // Time interval between spawns

//    private void Start()
//    {
//        StartCoroutine(SpawnEnemies());
//    }

//    private IEnumerator SpawnEnemies()
//    {
//        while (true)
//        {
//            yield return new WaitForSeconds(spawnRate);
//            SpawnEnemy();
//        }
//    }

//    private void SpawnEnemy()
//    {
//        if (spawnPoints.Length > 0)
//        {
//            int spawnIndex = Random.Range(0, spawnPoints.Length);
//            Instantiate(enemyPrefab, spawnPoints[spawnIndex].position, Quaternion.identity);
//        }
//    }

//    private void OnDestroy()
//    {
//        if (winScript != null)
//        {
//            winScript.OnBossDroneDestroyed();
//        }
//    }
//}

//OLD CODE 1
//public class BossDrone : MonoBehaviour
//{
//    public Win winScript;

//    private void OnDestroy()
//    {
//        if (winScript != null)
//        {
//            winScript.OnBossDroneDestroyed();
//        }
//    }
//}
