using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Physics))]
public class MeleeAttack : PlayerAttack, AttackCollisionHandler
{
    //[SerializeField] float dashPower = 2;
    //[SerializeField] PhysicsProperties dashPhysicsProperties;
    //[SerializeField] PhysicsProperties bouncePhysicsProperties;
    //[SerializeField] LayerMask bounceLayers;
    Rigidbody2D rb;
    Physics physics;
    PlayerAnimationController animationController;
    //[SerializeField] float bounceVerticalVelocity = 1.0f;
    //[SerializeField] float bounceHorizontalVelocity = -1.0f;
    //[SerializeField] float bounceHorizontalVelocityPreserved = 0.0f;

    //[SerializeField] float dashPhysicsTime = 0.1f;
    [SerializeField] float attackTime = 0.4f;
    //[SerializeField] float bouncePhysicsTime = 0.1f;
    //[SerializeField] float bounceTime = 0.4f;

    [SerializeField] GameObject hurtbox;
    [SerializeField] Hitbox hitbox;
    [SerializeField] AnimationPlayer slashAnimationController;
    [SerializeField] AudioClip attackHitClip;
    [SerializeField] AudioClip wooshSwordClip;
    

    //Coroutine waitForEndOfAttack;

    //DashState state = DashState.None;

    // Animation
    AnimationPlayable
        attackClipArms = new AnimationPlayable("enid_slash_arms_1", 2),
        attackClipLegs = new AnimationPlayable("enid_slash_legs_1", 0),
        attackClipArms2 = new AnimationPlayable("enid_slash_2_arms", 2),
        attackClipLegs2 = new AnimationPlayable("enid_slash_2_legs", 0),
        attackClipUpArms = new AnimationPlayable("enid_slash_up_arms", 2),
        attackClipUpLegs = new AnimationPlayable("enid_slash_up_legs", 0),
        attackClipDownArms = new AnimationPlayable("slash_down_arms", 2),
        attackClipDownLegs = new AnimationPlayable("slash_down_legs", 1),
        slashClip = new AnimationPlayable("slash_1", 0),
        slashClip2 = new AnimationPlayable("slash_2", 0),
        slashUpClip = new AnimationPlayable("slash_up", 0),
        slashDownClip = new AnimationPlayable("slash_down", 0);

    float timeRemaining = 0f;

    bool doSecondSlash = false;
    float secondSlashTime = 2f;
    float lastFirstSlashTime = 0f;

    public override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        physics = GetComponent<Physics>();
        hurtbox.gameObject.transform.position += new Vector3(1, 0, 0);
        animationController = GetComponent<PlayerAnimationController>();
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
        SoundFXManager.instance.PlaySoundFXClip(wooshSwordClip, transform, 0.2f);
        timeRemaining = attackTime;
 
        if (direction.x > 0 || direction.x < 0)
        {
            hurtbox.gameObject.transform.position = gameObject.transform.position;
            hurtbox.gameObject.transform.position += new Vector3(direction.x, 0, 0);
        }
        if (direction.y > 0 || direction.y < 0)
        {
            hurtbox.gameObject.transform.position = gameObject.transform.position;
            hurtbox.gameObject.transform.position += new Vector3(0, direction.y, 0);
        }

        // Start animation
        // Up slash
        if (direction.y > 0)
        {
            animationController.SetActiveAnimation(attackClipUpArms, PlayerAnimationController.AnimationType.Top);
            animationController.SetActiveAnimation(attackClipUpLegs, PlayerAnimationController.AnimationType.Bottom);
            slashAnimationController.gameObject.SetActive(true);
            slashAnimationController.SetActiveAnimation(slashUpClip);
        }
        // Down slash
        else if (direction.y < 0 && !physics.IsGrounded())
        {
            animationController.SetActiveAnimation(attackClipDownArms, PlayerAnimationController.AnimationType.Top);
            animationController.SetActiveAnimation(attackClipDownLegs, PlayerAnimationController.AnimationType.Bottom);
            slashAnimationController.gameObject.SetActive(true);
            slashAnimationController.SetActiveAnimation(slashDownClip);
        }
        // Horizontal Slash
        else
        {
            if (!doSecondSlash || Time.time - lastFirstSlashTime > secondSlashTime)
            {
                animationController.SetActiveAnimation(attackClipArms, PlayerAnimationController.AnimationType.Top);
                animationController.SetActiveAnimation(attackClipLegs, PlayerAnimationController.AnimationType.Bottom);
                slashAnimationController.gameObject.SetActive(true);
                slashAnimationController.SetActiveAnimation(slashClip);
                doSecondSlash = true;
                lastFirstSlashTime = Time.time;
            }
            else
            {
                animationController.SetActiveAnimation(attackClipArms2, PlayerAnimationController.AnimationType.Top);
                animationController.SetActiveAnimation(attackClipLegs2, PlayerAnimationController.AnimationType.Bottom);
                slashAnimationController.gameObject.SetActive(true);
                slashAnimationController.SetActiveAnimation(slashClip2);
                doSecondSlash = false;
            }
        }
        

        hurtbox.SetActive(true);
        hitbox.SetDashing(true);
    }

    public void AttackEnter(Collider2D other)
    {
        if (!CurrentlyInUse()) return;
     

        Hitbox hitbox = other.GetComponent<Hitbox>();
        hitbox.Hit(rb.velocity);

        SoundFXManager.instance.PlaySoundFXClip(attackHitClip, transform, 0.2f);

      
        pac.AirAttackRefresh();
    }

  

    public override void AttackEnd()
    {
        timeRemaining = 0f;

        physics.ResetPhysicsProperties();

        animationController.ResetActiveAnimation(PlayerAnimationController.AnimationType.Top);
        animationController.ResetActiveAnimation(PlayerAnimationController.AnimationType.Bottom);
        slashAnimationController.gameObject.SetActive(false);

        hurtbox.SetActive(false);
        hitbox.SetDashing(false);
    }

}
