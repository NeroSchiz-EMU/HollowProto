using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface AttackCollisionHandler
{
    public virtual void AttackEnter(Collider2D other) { }
    public virtual void AttackStay(Collider2D other) { }
    public virtual void AttackLeave(Collider2D other) { }
}

public class AttackCollisionController : MonoBehaviour
{
    [SerializeField] bool onChildOfObject = true;
    public AttackCollisionHandler[] attackCollisionHandlers;

    public void OnTriggerEnter2D(Collider2D other)
    {
        foreach (AttackCollisionHandler handler in attackCollisionHandlers)
        {
            handler.AttackEnter(other);
        }
    }
    public void OnTriggerStay2D(Collider2D other)
    {
        foreach (AttackCollisionHandler handler in attackCollisionHandlers)
        {
            handler.AttackStay(other);
        }
    }
    public void OnTriggerLeave2D(Collider2D other)
    {
        foreach (AttackCollisionHandler handler in attackCollisionHandlers)
        {
            handler.AttackLeave(other);
        }
    }

    public void Awake()
    {
        GameObject obj;
        if (onChildOfObject)
        {
            obj = transform.parent.gameObject;
        }
        else
        {
            obj = gameObject;
        }

        attackCollisionHandlers = obj.GetComponentsInChildren<AttackCollisionHandler>();
    }
}