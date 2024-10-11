using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FlyBehavior : AIBehavior
{
    [SerializeField] Physics physics;
    [SerializeField] float flySpeed;
    [SerializeField] bool getsStuckOnWalls = false;
    [ShowWhen("getsStuckOnWalls", false)]
    [SerializeField] float avoidWallsRadius = 1.0f;

    Vector2 target;

    public void SetTarget(Vector2 newTarget)
    {
        target = newTarget;
    }

    protected override void ActivationAction()
    {
        SetTarget(controller.transform.position);
    }

    protected override void UpdateAction()
    {
        Vector2 flyDirection = (target - (Vector2)Transform().position).normalized;

        /*
        if (!getsStuckOnWalls)
        {
            Debug.Log("doesn't get stuck");
            float raycastDistance = avoidWallsRadius + flySpeed * Time.deltaTime;

            int layerMask = LayerMask.GetMask("Level");
            RaycastHit2D hit = Physics2D.Raycast((Vector2)Transform().position, flyDirection, raycastDistance, layerMask);
            if (hit.collider != null)
            {
                // Calculate the perpendicular direction to the hit normal
                Vector2 perpendicularDirection = Vector2.Perpendicular(hit.normal);

                // Choose the perpendicular direction that is closest to the original fly direction
                if (Vector2.Dot(perpendicularDirection, flyDirection) < 0)
                {
                    perpendicularDirection = -perpendicularDirection;
                }
                flyDirection = perpendicularDirection.normalized;
                Debug.Log(flyDirection);
            }
        }*/

        physics.Move(flyDirection * flySpeed);
    }
}