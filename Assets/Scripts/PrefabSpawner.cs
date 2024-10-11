using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] float waitTime = 3f;
    float timeOfLastSpawn = Mathf.NegativeInfinity;
    private void Update()
    {
        if (Time.time - timeOfLastSpawn >= waitTime)
        {
            timeOfLastSpawn = Time.time;
            GameObject enemy = Instantiate(prefab);
            enemy.transform.position = transform.position;
        }
    }
}
