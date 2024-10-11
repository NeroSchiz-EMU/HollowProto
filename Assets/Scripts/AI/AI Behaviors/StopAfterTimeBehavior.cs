using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StopAfterTimeBehavior : AIBehavior {
    [SerializeField] Optional<AIBehavior> behavior;
    [SerializeField] float seconds;

    protected override List<AITreeNode> GetChildren()
    {
        var children = new List<AITreeNode>();
        if (behavior.Enabled)
        {
            children.Add(behavior.Value);
        }
        return children;
    }
    protected override void ActivationAction() {
        if (behavior.Enabled) {
            SetChildBehavior(behavior.Value, seconds);
        }
        else {
            DeactivateAfterSeconds(seconds);
        }
    }

    public override void ChildDeactivated(DeactivationReason reason = null) {
        Deactivate(reason);
    }
}
