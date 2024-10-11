using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class IsFloatingBox : MonoBehaviour
{
    [SerializeField] float buoyantForce = 10f;
    [Tooltip("Depth at which the maximum force should be applied to this object. Force will fade at lower depths")]
    [SerializeField] float maxForceDepth = 1f;
    [SerializeField] float velocityDampeningFactor = 0.9f;

    // Tracks if this box is touching water
    bool touchingWater = false;
    BoxCollider2D waterCollider;

    // Store reference to rigid body
    Rigidbody2D rigidbody;
    BoxCollider2D boxCollider;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if other object is water
        if (collision.gameObject.GetComponent<Water>())
        {
            touchingWater = true;
            waterCollider = collision as BoxCollider2D;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Check if other object is water
        if (other.gameObject.GetComponent<Water>())
        {
            touchingWater = false;
        }
    }

    private void FixedUpdate()
    {
        if (touchingWater)
        {
            // Find the coordinates of the surface of the water
            float surfaceY = waterCollider.bounds.center.y + waterCollider.bounds.extents.y;

            // Find the coordinates of the bottom of the box
            float bottomOfBoxY = boxCollider.bounds.center.y - boxCollider.bounds.extents.y;

            // Calculate how far below the surface the box is
            float distanceBelowSurface = surfaceY - bottomOfBoxY;

            // Apply force based on depth
            // If below the maximum force depth, apply the maximum force
            if (distanceBelowSurface >= maxForceDepth)
            {
                rigidbody.AddForce(Vector2.up * buoyantForce);
            }
            // If below the surface but not below the maximum depth, apply partial force
            else if (distanceBelowSurface > 0)
            {
                rigidbody.AddForce(Vector2.up * buoyantForce * (distanceBelowSurface / maxForceDepth));
            }
        }

        // Dampen velocity
        Vector2 velocity = rigidbody.velocity;
        if(velocity.y > 0)
        {
            velocity *= velocityDampeningFactor;
                    rigidbody.velocity = velocity;
        }
    }
}
