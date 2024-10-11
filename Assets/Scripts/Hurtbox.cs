using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    [SerializeField] bool dashable = true;
    public bool Dashable()
    {
        return dashable;
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        Hitbox hitbox = collision.gameObject.GetComponent<Hitbox>();
        hitbox?.Hit(collision.gameObject.transform.position - transform.position, this);
    }
}
