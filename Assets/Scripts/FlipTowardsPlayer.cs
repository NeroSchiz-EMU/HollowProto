using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipTowardsPlayer : MonoBehaviour
{
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (player != null)
        {
            Vector3 direction = player.position - transform.position;
            if (direction.x >= 0.1f)
            {
                transform.localScale = new Vector3(1, 1, 1); // Face right
            }
            else if (direction.x <= -0.1f)
            {
                transform.localScale = new Vector3(-1, 1, 1); // Face left
            }
        }
    }
}
