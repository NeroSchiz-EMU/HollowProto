using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderBot : MonoBehaviour
{
    
    [Header("Speed Info")] 
    [SerializeField] private float moveSpeed;
    [SerializeField] private float playerSeenMultiplier;
    
    [SerializeField] protected Transform playerCheck;
    [SerializeField] protected float playerCheckDistance;
    [SerializeField] protected LayerMask whatIsPlayer;
    
    protected Rigidbody2D rb;
    protected Animator anim;
    
    protected int facingDir = 1;
    protected bool facingRight = true;
    
    private bool isAttacking;
    private RaycastHit2D isPlayerDetected;
    
    public void Update()
    {
        if (isPlayerDetected)
        {
            if (isPlayerDetected.distance > 1)
            {
                rb.velocity = new Vector2(moveSpeed * playerSeenMultiplier, rb.velocity.y);
                Debug.Log("I see the player!");
                isAttacking = false;
            }
            else
            {
                Debug.Log("ATTACK!! " + isPlayerDetected.collider.gameObject.name);
                isAttacking = true;
            }
        }
    }
    



    public void CollisionChecks()
    {
        isPlayerDetected = Physics2D.Raycast(playerCheck.position,
            Vector2.right, playerCheckDistance * facingDir, whatIsPlayer);
    }
    
    void OnTriggerEnter2D (Collider2D col)
    {
        switch (col.tag) 
        { 
            case "Level": rb.AddForce (Vector2.up * 642f); break;
        }	
    }
    
    protected virtual void Flip()
    {
        facingDir = -facingDir;
        facingRight = !facingRight;
        anim.transform.Rotate(0f, 180f, 0f);
    }
    
    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(playerCheck.position,
            new Vector3(playerCheck.position.x + playerCheckDistance * facingDir, playerCheck.position.y));
    }
    
}

