using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyTowardTargetBehavior : AIBehavior {
    [SerializeReference] FlyBehavior flyingBehavior;

    protected override List<AITreeNode> GetChildren()
    {
        return new List<AITreeNode>() { flyingBehavior };
    }

    protected override void ActivationAction() {
        SetChildBehavior(flyingBehavior);
    }
    protected override void UpdateAction() {
        if (!controller.Target()) return;
        flyingBehavior.SetTarget(controller.Target().transform.position);
    }
}
