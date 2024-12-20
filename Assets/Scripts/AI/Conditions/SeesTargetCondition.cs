using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SeesTargetCondition : Condition
{
    [SerializeField] LayerMask visionBlockMask;
    [SerializeField] float maxTrackingDistance = 10f; // Maximum distance to track the target

    public override bool Satisfied()
    {
        if (!controller.Target()) return false;
        Vector2 vectorToTarget = (controller.Target().transform.position - controller.transform.position);
        if (vectorToTarget.magnitude > maxTrackingDistance) return false;
        return !Physics2D.Raycast(controller.transform.position, vectorToTarget.normalized, vectorToTarget.magnitude, visionBlockMask);
    }
}


// Old code
//[System.Serializable]
//public class SeesTargetCondition : Condition {
//    [SerializeField] LayerMask visionBlockMask;
//    public override bool Satisfied() {
//        if (!controller.Target()) return false;
//        Vector2 vectorToTarget = (controller.Target().transform.position - controller.transform.position);
//        return !Physics2D.Raycast(controller.transform.position, vectorToTarget.normalized, vectorToTarget.magnitude, visionBlockMask);
//    }
//}