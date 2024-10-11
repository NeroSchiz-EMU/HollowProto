using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering.Universal;

public class ScanBeam : MonoBehaviour
{
    // Light Settings
    [Header("Light Info")]
    public Light2D detectionLight; 
    public float detectionRange; // Use this directly for distance checks 
    public LayerMask playerLayer; // Only detect the player layer
    // Rotation Settings
    public float rotationSpeed = 180f; 
    public float maxFollowDistance = 15f; 
    public float returnToDefaultDistance = 8f;

    // Attack Settings
    [Header("Attack Info")]
    public Transform player; 
    public float attackRange = 10f;
    public float attackCooldown = 1f; 
    public float damageAmount = 1; 
    public bool canAttack = true;
    public Health playerHealth;
    
    //Bullet Settings
    // [Header("Bullet Info")]
    // public GameObject bullet;
    // public Transform bulletPos;
    
    //public float timer;
    //public GameObject target;
    
    private Quaternion initialRotation = Quaternion.Euler(0, 0, -90); // Default down
    private bool isFollowingPlayer = false; 

    void Start()
    {
        //target = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<Health>();
        initialRotation = Quaternion.Euler(0, 0, 0); // Default down
        detectionLight.transform.rotation = initialRotation;  // Apply the initial rotation immediately
    }

    void Update()
    {
        if (isFollowingPlayer)
        {
            FollowPlayer();
            float distance = Vector2.Distance(transform.position, player.transform.position);
            
            // timer += Time.deltaTime;
            // if (timer > .8)
            // {
            //     timer = 0;
            //     //shoot();
            // }
            CheckFollowDistance();
        }
        else
        {
            DetectPlayer();
            // Add this line to return to default when not following:
            ReturnToDefaultIfPlayerOutOfRange();
        }
    }

    void ReturnToDefaultIfPlayerOutOfRange()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer > maxFollowDistance) 
        {
            detectionLight.transform.rotation = Quaternion.Slerp(
                detectionLight.transform.rotation, initialRotation, rotationSpeed * Time.deltaTime
            );
        }
    }
    
    // void shoot()
    // {
    //     Instantiate(bullet, bulletPos.position, Quaternion.identity);
    // }
    
    void DetectPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer > detectionRange)
        {
            return; // Exit early if the player is out of range
        }
        
        // Light Beam Angle Check
        Vector2 directionToPlayer = (player.position - detectionLight.transform.position).normalized;
        float angleToPlayer = Vector2.Angle(-detectionLight.transform.up, directionToPlayer);
        if (angleToPlayer > detectionLight.pointLightOuterAngle / 2)
        {
            return; // Exit if player isn't within the light beam
        }

        // Raycast (Only if in range and in light beam)
        RaycastHit2D hit = Physics2D.Raycast(detectionLight.transform.position, directionToPlayer, 
            detectionRange, playerLayer); // Use playerLayer
        if (hit.collider != null && hit.collider.transform == player) 
        {
            AttackPlayer();
        }
        
        if (hit.collider != null && hit.collider.transform == player) 
        {
            isFollowingPlayer = true;
            AttackPlayer();
        }
    }
    
    void FollowPlayer()
    {
        Vector2 directionToPlayer = (player.position - detectionLight.transform.position).normalized;
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        angle += 90f;

        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        detectionLight.transform.rotation = Quaternion.Slerp(
            detectionLight.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime
        );
    }

    void CheckFollowDistance()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > maxFollowDistance || !PlayerIsInLight())
        {
            isFollowingPlayer = false;
        }

        // Rotate to default ONLY if the beam isn't currently following the player
        if (!isFollowingPlayer)
        {
            detectionLight.transform.rotation = Quaternion.Slerp(
                detectionLight.transform.rotation, initialRotation, rotationSpeed * Time.deltaTime
            );
        }
    }
    
    bool PlayerIsInLight()
    {
        Vector2 directionToPlayer = (player.position - detectionLight.transform.position).normalized;
        float angleToPlayer = Vector2.Angle(detectionLight.transform.up, directionToPlayer);
        if (angleToPlayer > detectionLight.pointLightOuterAngle / 2)
        {
            return false;
        }

        RaycastHit2D hit = Physics2D.Raycast(detectionLight.transform.position, directionToPlayer,
            detectionRange, playerLayer);
        return hit.collider != null && hit.collider.transform == player;
    }

    void AttackPlayer()
    {
        if (canAttack && Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            Debug.Log("Attacking the player!");
            Hitbox hitbox = playerHealth.GetComponentInChildren<Hitbox>();
            hitbox?.Hit(playerHealth.transform.position - transform.position, this.GetComponent<Hurtbox>());
            StartCoroutine(AttackCooldown());
        }
    }

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
