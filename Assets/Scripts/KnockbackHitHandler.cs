using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class KnockbackHitHandler : MonoBehaviour, HitHandler
{
    [SerializeField] float knockbackFactor = 1.0f;
    Rigidbody2D rb;

    public void Hit(Vector2 force, float damage = 1)
    {
        rb.velocity = knockbackFactor * force.normalized;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
}
