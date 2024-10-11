using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class TargetCenterInColliderCondition : Condition {
    [SerializeField] Collider2D collider;
    public override bool Satisfied() {
        if (!controller.Target()) return false;
        return collider.OverlapPoint(controller.Target().transform.position);
    }
}

