using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCloseCondition : Condition {
    [SerializeField] float radius;
    public override bool Satisfied() {
        if (!controller.Target()) return false;
        Vector2 vectorToTarget = (controller.Target().transform.position - controller.transform.position);
        return vectorToTarget.magnitude <= radius;
    }
}
