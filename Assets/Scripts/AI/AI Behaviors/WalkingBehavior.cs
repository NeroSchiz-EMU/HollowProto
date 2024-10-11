using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WalkingBehavior : AIBehavior {
    [SerializeField] Direction direction;
    [SerializeField] Physics physics;

    [SerializeField] float walkSpeed;

    [SerializeField] Transform wallDetectionRaycast;
    [SerializeField] Transform gapDetectionRaycast;

    [SerializeField] LayerMask tileLayerMask;

    const float gapCheckDistance = 1f;

    protected override void ActivationAction() {
        //if (animator != null) animator.SetAnimationState(walkAnimation);
        physics.Move(direction.FowardVector() * walkSpeed);
    }

    public bool CanWalkForward() {
        float distanceAboutToWalk = physics.GetVelocity().x * Time.fixedDeltaTime;
        bool aboutToHitWall = Physics2D.Raycast(wallDetectionRaycast.position, wallDetectionRaycast.up, distanceAboutToWalk, tileLayerMask);
        bool aboutToWalkOverGap = !Physics2D.Raycast(gapDetectionRaycast.position, gapDetectionRaycast.up, gapCheckDistance, tileLayerMask);

        if (aboutToHitWall) {
            return false;
        }
        else if (aboutToWalkOverGap) {
            return false;
        }

        return true;
    }
    protected override void UpdateAction() {
        //if (animator != null) animator.SetAnimationState(walkAnimation);
        physics.Move(direction.FowardVector() * walkSpeed);
        if (!CanWalkForward()) {
            Deactivate();
        }
    }
}

/*public class WalkingBehavior : AIBehavior {
    [SerializeField] Direction direction;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] AnimationController animator;


    [SerializeField] string walkAnimation;
    [SerializeField] float walkSpeed;

    [SerializeField] Transform wallDetectionRaycast;
    [SerializeField] Transform gapDetectionRaycast;

    [SerializeField] LayerMask tileLayerMask;

    float gapCheckDistance = 1f;

    protected override void StartAction() {
        animator?.SetAnimationState(walkAnimation);
        rb.velocity = direction.FowardVector() * walkSpeed;
    }

    public bool CanWalkForward() {
        float distanceAboutToWalk = walkSpeed * Time.fixedDeltaTime;
        bool aboutToHitWall = Physics2D.Raycast(wallDetectionRaycast.position, wallDetectionRaycast.up, distanceAboutToWalk, tileLayerMask);
        bool aboutToWalkOverGap = !Physics2D.Raycast(gapDetectionRaycast.position, gapDetectionRaycast.up, gapCheckDistance, tileLayerMask);

        if (aboutToHitWall) {
            return false;
        }
        else if (aboutToWalkOverGap) {
            return false;
        }

        return true;
    }
    protected override void ActionUpdate() {
        rb.velocity = direction.FowardVector() * walkSpeed;
        if (!CanWalkForward()) {
            Deactivate();
        }
    }

    protected override void StopAction() {
        rb.velocity = Vector2.zero;
    }
}*/

/*public class WalkingAction : MonoBehaviour {
    [SerializeField] Direction direction;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] AnimationController animator;


    [SerializeField] string walkAnimation;
    [SerializeField] float walkSpeed;

    [SerializeField] Transform wallDetectionRaycast;
    [SerializeField] Transform gapDetectionRaycast;

    [SerializeField] LayerMask tileLayerMask;

    float gapCheckDistance = 1f;

    bool active = false;

    public bool IsActive() {
        return active;
    }

    public void SetActive(bool value) {
        if (value == active) return;
        if (value) StartWalkingFoward();
        StopWalkingForwad();
    }

    public void StartWalkingFoward() {
        active = true;
        animator?.SetAnimationState(walkAnimation);
        rb.velocity = direction.FowardVector() * walkSpeed;
    }
    private void WalkFoward() {
        rb.velocity = direction.FowardVector() * walkSpeed;
        float distanceAboutToWalk = walkSpeed * Time.fixedDeltaTime;
        bool aboutToHitWall = Physics2D.Raycast(wallDetectionRaycast.position, wallDetectionRaycast.up, distanceAboutToWalk, tileLayerMask);
        bool aboutToWalkOverGap = !Physics2D.Raycast(gapDetectionRaycast.position, gapDetectionRaycast.up, gapCheckDistance, tileLayerMask);

        if (aboutToHitWall) {
            StopWalkingForwad();
            //return StopReason.Wall;
        }
        else if (aboutToWalkOverGap) {
            StopWalkingForwad();
            //return StopReason.Gap;
        }
        //return StopReason.None;
    }

    public void StopWalkingForwad() {
        active = false ;
        rb.velocity = Vector2.zero;
    }

    private void Update() {
        if (active) WalkFoward();
    }
}*/



/*public class WalkingAction : MonoBehaviour {
    [SerializeField] string walkAnimation;
    [SerializeField] float walkSpeed;

    [SerializeField] Transform wallDetectionRaycast;
    [SerializeField] Transform gapDetectionRaycast;

    [SerializeField] LayerMask tileLayerMask;

    float gapCheckDistance = 1f;


    bool interrupt = false;

    public enum StopReason { Gap, Wall, TimeElapsed, Interrupted }
    StopReason stopReason;

    public StopReason GetStopReason() {
        return stopReason;
    }
    public void Interrupt() {
        interrupt = true;
    }
    public IEnumerator WalkForward(Direction direction, AnimationController animator, Rigidbody2D rb, float seconds = Mathf.Infinity) {
        float startTime = Time.time;

        animator?.SetAnimationState(walkAnimation);
        while (Time.time - startTime < seconds && !interrupt) {
            rb.velocity = direction.FowardVector() * walkSpeed;
            float distanceAboutToWalk = walkSpeed * Time.fixedDeltaTime;
            bool aboutToHitWall = Physics2D.Raycast(wallDetectionRaycast.position, wallDetectionRaycast.up, distanceAboutToWalk, tileLayerMask);
            bool aboutToWalkOverGap = !Physics2D.Raycast(gapDetectionRaycast.position, gapDetectionRaycast.up, gapCheckDistance, tileLayerMask);

            if (aboutToHitWall) {
                stopReason = StopReason.Wall;
                break;
            }
            else if (aboutToWalkOverGap) {
                stopReason = StopReason.Gap;
                break;
            }
            yield return null;
        }

        
        if (Time.time - startTime >= seconds) {
            stopReason = StopReason.TimeElapsed;
        }
        else if (interrupt) {
            stopReason = StopReason.Interrupted;
        }
        rb.velocity = Vector2.zero;
        interrupt = false;
    }
}*/
