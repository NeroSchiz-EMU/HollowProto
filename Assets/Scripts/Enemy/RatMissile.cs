using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatMissile : MonoBehaviour
{

    private CircleCollider2D collider;

    [SerializeField] float rotateSpeed = 2;
    [SerializeField] float moveSpeed = 10f;

    [SerializeField] Transform target;

    private Rigidbody2D rb;

    [SerializeField] float Homingoffset = 3f;

    private Vector3 lastPos;
    // Start is called before the first frame update
    void Start()
    {
        collider = gameObject.GetComponent<CircleCollider2D>();
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //move towards target
        if(target){
            rb.angularVelocity = rotateSpeed;
            Vector2 distance = target.position - transform.position;
            if(distance.magnitude >= Homingoffset){
                rb.velocity = distance.normalized * moveSpeed;
                lastPos = target.position;
            }else{
                rb.velocity = (lastPos - transform.position).normalized * moveSpeed;
                target = null;
            }
        }
    }
        
    
    public void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject otherObject = collision.gameObject;

        Debug.Log(collision.gameObject.name);
        //move past enemy
        if(otherObject.tag == "Enemy"){
            return;
        }

        //hit player
        else if(otherObject.tag == "Player"){

            //find hitbox
            Hitbox hitbox = null;
            foreach (Transform child in otherObject.transform){
                if (child.name == "Hitbox"){
                    hitbox = child.GetComponent<Hitbox>();
                }
            } 
            //hit
            if (hitbox != null)
                hitbox.Hit(collision.gameObject.transform.position - transform.position, transform.GetChild(0).GetComponent<Hurtbox>());
        }

        Destroy(gameObject);
    }
}
