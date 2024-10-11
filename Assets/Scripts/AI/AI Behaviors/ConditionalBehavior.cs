using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.OnScreen.OnScreenStick;
using static UnityEngine.RuleTile.TilingRuleOutput;
using TriInspector;


[System.Serializable]
public class ConditionalBehavior : AIBehavior {
    [SerializeReference] Condition swapCondition;
    [SerializeReference] AIBehavior defaultBehavior;
    [SerializeReference] AIBehavior swappedBehavior;

    protected override List<AITreeNode> GetChildren()
    {
        return new List<AITreeNode>() { defaultBehavior, swappedBehavior, swapCondition };
    }

    protected override void ActivationAction() {
        SwapIfNeeded();
    }

    private void SwapIfNeeded() {
        if (swapCondition.Satisfied()) {
            SetChildBehavior(swappedBehavior);
        }
        else {
            SetChildBehavior(defaultBehavior);
        }
    }

    protected override void WhenChildInterruptible() {
        SwapIfNeeded();
    }

    public override void ChildDeactivated(DeactivationReason reason = null) {
        SwapIfNeeded();
    }
}