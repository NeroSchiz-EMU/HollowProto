using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//PROTOTYPE ONLY: from a previous project... needs some serious work
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Physics : MonoBehaviour {
    [SerializeField] PhysicsProperties phys;
    PhysicsProperties originalPhysicsProperties;
    [SerializeField] Optional<LayerMask> customSolidGroundLayers;
    LayerMask solidGroundLayers;


    Collider2D col;
    Rigidbody2D rb;
    const float epsilon = 0.1f;

    bool movementSetThisFrame = false;
    Vector2 queuedMovement = Vector2.zero;

    public Vector2 GetVelocity() {
        return rb.velocity;
    }

    private void Awake() {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        if (customSolidGroundLayers.Enabled) solidGroundLayers = customSolidGroundLayers.Value;
        else solidGroundLayers = Physics2D.GetLayerCollisionMask(gameObject.layer);
        originalPhysicsProperties = phys;
    }


    public void SetPhysicsProperties(PhysicsProperties phys)
    {
        this.phys = phys;
    }

    public void ResetPhysicsProperties()
    {
        SetPhysicsProperties(originalPhysicsProperties);
    }

    public void Move(Vector2 goalVelocity) {
        movementSetThisFrame = true;
        queuedMovement = goalVelocity;
    }

    private float AccelerationAmount(Vector2 vel, Vector2 goalVel, bool grounded) {
        float currentSpeed = phys.Flying ? vel.magnitude : Mathf.Abs(vel.x);
        bool belowGoalSpeed = currentSpeed <= goalVel.magnitude || Vector2.Dot(vel, goalVel) < 0;

        float acceleration = (grounded ? phys.GroundDefaultAcceleration : phys.AirDefaultAcceleration);
        float decelerationTowardDefaultSpeed = (grounded ? phys.GroundDecelerationTowardDefaultSpeed : phys.AirDecelerationTowardDefaultSpeed);
        float decelerationTowardZero = (grounded ? phys.GroundDecelerationTowardZero : phys.AirDecelerationTowardZero);

        float amount;
        if (belowGoalSpeed && goalVel != Vector2.zero) {
            amount = acceleration;
        }
        else if (goalVel == Vector2.zero) {
            amount = decelerationTowardZero;
        }
        else {
            amount = decelerationTowardDefaultSpeed;
        }

        amount *= Time.fixedDeltaTime;

        return amount;
    }

    private void FixedUpdate() {
        bool grounded = IsGrounded();
        Vector2 vel = rb.velocity;

        vel.y -= phys.Gravity * Time.fixedDeltaTime;

        float amount = AccelerationAmount(vel, queuedMovement, grounded);
        if (phys.Flying) {
            vel = Vector2.MoveTowards(vel, queuedMovement, amount);
        }
        else {
            vel.x = Mathf.MoveTowards(vel.x, queuedMovement.x, amount);
        }

        vel.y = Mathf.Max(vel.y, -phys.MaxFallSpeed);

        rb.velocity = vel;
    }

    private void LateUpdate() {
        if (!movementSetThisFrame) {
            queuedMovement = Vector2.zero;
        }
        movementSetThisFrame = false;
    }

    public bool IsGrounded() {
        Bounds bounds = col.bounds;
        bounds.Expand(-epsilon);
        RaycastHit2D hit = Physics2D.BoxCast(bounds.center, bounds.size, 0f, Vector2.down, 2 * epsilon, solidGroundLayers);

        return hit.collider != null;
    }

    public bool IsTouchingLayerMask(LayerMask mask)
    {
        Bounds bounds = col.bounds;
        bounds.Expand(-epsilon);
        Collider2D hit = Physics2D.OverlapBox(bounds.center, bounds.size, 0f, mask);

        return hit != null;
    }
}