using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DashState { None, Dash, Bounce }


[RequireComponent(typeof(Physics))]
public class DashAttack : PlayerAttack, AttackCollisionHandler
{
    [SerializeField] float dashPower = 2;
    [SerializeField] PhysicsProperties dashPhysicsProperties;
    [SerializeField] PhysicsProperties bouncePhysicsProperties;
    [SerializeField] LayerMask bounceLayers;
    Rigidbody2D rb;
    Physics physics;
    [SerializeField] float bounceVerticalVelocity = 1.0f;
    [SerializeField] float bounceHorizontalVelocity = -1.0f;
    [SerializeField] float bounceHorizontalVelocityPreserved = 0.0f;

    [SerializeField] float dashPhysicsTime = 0.1f;
    [SerializeField] float dashTime = 0.4f;
    [SerializeField] float bouncePhysicsTime = 0.1f;
    [SerializeField] float bounceTime = 0.4f;

    [SerializeField] GameObject hurtbox;
    [SerializeField] Hitbox hitbox;

    Coroutine waitForEndOfAttack;

    DashState state = DashState.None;

    float timeRemaining = 0f;
    public override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        physics = GetComponent<Physics>();
    }

    public override void Update()
    {
        timeRemaining -= Time.deltaTime;
        base.Update();
    }

    public override bool CanStop()
    {
        return timeRemaining <= 0;
    }
    public override void AttackStart(Vector2 direction)
    {
        timeRemaining = dashTime;
        rb.velocity = direction * dashPower;
        state = DashState.Dash;
        waitForEndOfAttack = StartCoroutine(DashTimeline());
        hurtbox.SetActive(true);
        hitbox.SetDashing(true);
    }

    public void AttackStay(Collider2D other)
    {
        if (!CurrentlyInUse()) return;
        if (bounceLayers != (bounceLayers | (1 << other.gameObject.layer))) return; // if not in bounce layer, return
        if (state != DashState.Dash) return;

        Hitbox hitbox = other.GetComponent<Hitbox>();
        hitbox.Hit(rb.velocity);


        StopCoroutine(waitForEndOfAttack);
        waitForEndOfAttack = StartCoroutine(BounceTimeline());
        state = DashState.Bounce;
        pac.AirAttackRefresh();
    }

    public void Bounce()
    {
        physics.SetPhysicsProperties(bouncePhysicsProperties);
        Vector2 vel = rb.velocity;


        float dir = Mathf.Sign(vel.x);
        vel = new Vector2(vel.x * bounceHorizontalVelocityPreserved + dir * bounceHorizontalVelocity, bounceVerticalVelocity);


        rb.velocity = vel;
    }

    public override void AttackEnd()
    {
        physics.ResetPhysicsProperties();
        state = DashState.None;
        hurtbox.SetActive(false);
        hitbox.SetDashing(false);
    }

    IEnumerator DashTimeline()
    {
        physics.SetPhysicsProperties(dashPhysicsProperties);
        yield return new WaitForSeconds(dashPhysicsTime);
        physics.ResetPhysicsProperties();
        yield return new WaitForSeconds(dashTime - dashPhysicsTime);
        EndAttack();
    }

    IEnumerator BounceTimeline()
    {
        hurtbox.SetActive(false);
        Bounce();
        yield return new WaitForSeconds(bouncePhysicsTime);
        physics.ResetPhysicsProperties();
        yield return new WaitForSeconds(bounceTime - bouncePhysicsTime);
        EndAttack();
    }
}
