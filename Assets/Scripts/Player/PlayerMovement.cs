using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

//PROTOTYPE ONLY: from a previous project... probably needs some re-working
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Physics))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float runSpeed = 9f;
    [SerializeField] PlayerAnimationController animator;
    [SerializeField] float jumpForce = 15f;
    [SerializeField] float variableJumpYSpeedConserved = 0.5f;
    [SerializeField] float coyoteDelay = 0.03f;
    [SerializeField] float jumpBufferSeconds = 0.03f;

    [SerializeField] AudioClip jumpSound, landSound;


    Physics physics;
    Rigidbody2D rb;
    InputManager input;

    // enable and disable movement
    public bool movementEnabled = true;

    private bool isJumping = false;
    private bool previouslyJumping = false;

    bool prevGroundedState = true;


    float coyoteTimeLeftGround = Mathf.NegativeInfinity;


    float lastJumpTime = Mathf.NegativeInfinity;
    float lastJumpInputTime = Mathf.NegativeInfinity;

    // animation stuff
    AnimationPlayable
        runclipArms = new AnimationPlayable("enid_run_arms", 0),
        runclipLegs = new AnimationPlayable("enid_run_legs", 0),
        idleclip = new AnimationPlayable("enid_idle", 0),
        jumpclipArms = new AnimationPlayable("enid_jump_arms", 0),
        jumpclipLegs = new AnimationPlayable("enid_jump_legs", 0),
        fallclipArms = new AnimationPlayable("enid_fall_arms", 0),
        fallclipLegs = new AnimationPlayable("enid_fall_legs", 0),
        landclip = new AnimationPlayable("enid_land", 1);

    private void Awake()
    {
        physics = GetComponent<Physics>();
        rb = GetComponent<Rigidbody2D>();
        // TODO: Assert that an Input Manager must always exists for this to work.
        input = GameObject.Find("InputManager").GetComponent<InputManager>();
    }

    public void SetSpeed(float value)
    {
        runSpeed = value;
    }

    public float GetSpeed()
    {
        return runSpeed;
    }

    public void InterruptJump(bool slowUpwardAcceleration = false)
    {
        isJumping = false;
        previouslyJumping = slowUpwardAcceleration ? previouslyJumping : false;
    }

    void TryPlayJumpSound()
    {
        if (physics.IsGrounded())
        {
            SoundFXManager.instance.PlaySoundFXClip(jumpSound, transform, 1f);
        }
	}

	private void Movement()
    {
        // Don't do movement if movement is disabled
        if (!movementEnabled)
            return;

        Vector2 vel = rb.velocity;

        bool grounded = physics.IsGrounded();
        if (vel.y <= 0) isJumping = false;

        if (grounded && vel.y <= 0)
        {
            coyoteTimeLeftGround = Time.time;
        }

        if (input.JumpInputPressed())
        {
            lastJumpInputTime = Time.time;
        }


        bool coyoteValid = Time.time - coyoteTimeLeftGround < coyoteDelay;
        bool jumpQueued = (Time.time - lastJumpInputTime < jumpBufferSeconds) &&
            (lastJumpTime < lastJumpInputTime);

        if ((grounded || coyoteValid) && (input.JumpInputPressed() || jumpQueued))
        {
            
            isJumping = true;
            coyoteTimeLeftGround = -Mathf.Infinity;
			TryPlayJumpSound();

			vel.y = jumpForce;
            lastJumpTime = Time.time;
        }
        else if (input.JumpInputReleased() && isJumping)
        {
            isJumping = false;
        }
        if (previouslyJumping && !isJumping)
        {
            vel.y *= variableJumpYSpeedConserved;
        }
        previouslyJumping = isJumping;

        rb.velocity = vel;

        physics.Move(input.MoveInput().x * runSpeed * Vector2.right);

        if (vel.y > 0.1 && !grounded)
        {
            animator.SetActiveAnimation(jumpclipArms, PlayerAnimationController.AnimationType.Top);
            animator.SetActiveAnimation(jumpclipLegs, PlayerAnimationController.AnimationType.Bottom);
		}      
        else if ((vel.x > 0.1 || vel.x < -0.1) && grounded)
        {
            // Debug.Log("RUNNING");
			animator.SetActiveAnimation(runclipArms, PlayerAnimationController.AnimationType.Top);
            animator.SetActiveAnimation(runclipLegs, PlayerAnimationController.AnimationType.Bottom);
		}
        else if (vel.y < -0.1 && !grounded)
        {
            // Debug.Log("FALLING");
            animator.SetActiveAnimation(fallclipArms, PlayerAnimationController.AnimationType.Top);
            animator.SetActiveAnimation(fallclipLegs, PlayerAnimationController.AnimationType.Bottom);
        }
        else
        {
            animator.SetActiveAnimation(idleclip, PlayerAnimationController.AnimationType.Single);  
        }

        
        

        if (grounded && prevGroundedState == false)
        {
            animator.SetActiveAnimation(landclip, PlayerAnimationController.AnimationType.Single);
			SoundFXManager.instance.PlaySoundFXClip(landSound, transform, 0.2f);
			StartCoroutine(fallToIdle());
        }

        prevGroundedState = grounded;
    }

    IEnumerator fallToIdle()
    {
		yield return new WaitForSeconds(0.2f);
		animator.SetActiveAnimationAndResetPriority(idleclip, PlayerAnimationController.AnimationType.Single);

	}


	private void Update()
    {
        Movement();
        
	}
}
