using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface HitHandler
{
    void Hit(Vector2 force, float damage = 1.0f);
}

public class Hitbox : MonoBehaviour
{
    [SerializeField] bool invulnerable = false;
    [SerializeField] bool onChildOfObject = true;
    bool dashingInvulnerable = false;
    public HitHandler[] hitHandlers;
    public void Hit(Vector2 force, Hurtbox hurtbox = null, float damage = 1.0f)
    {
        if (invulnerable) return;
        if (dashingInvulnerable && hurtbox != null && hurtbox.Dashable()) return;
        foreach (HitHandler handler in hitHandlers)
        {
            handler.Hit(force, damage);
        }
    }

    public void SetDashing(bool dashing)
    {
        dashingInvulnerable = dashing;
    }

    public void SetInvulnerable(bool value)
    {
        invulnerable = value;
    }

    public bool Invulnerable()
    {
        return invulnerable;
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

        hitHandlers = obj.GetComponentsInChildren<HitHandler>(true);
    }
}
