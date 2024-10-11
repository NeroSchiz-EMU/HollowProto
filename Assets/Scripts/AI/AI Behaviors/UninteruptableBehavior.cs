using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UninteruptableBehavior : AIBehavior
{
    [SerializeField] Optional<AIBehavior> behaviorMadeUninteruptable;

    protected override List<AITreeNode> GetChildren()
    {
        var children = new List<AITreeNode>();
        if (behaviorMadeUninteruptable.Enabled)
        {
            children.Add(behaviorMadeUninteruptable.Value);
        }
        return children;
    }
    protected override void ActivationAction()
    {
        if (behaviorMadeUninteruptable.Enabled)
        {
            SetChildBehavior(behaviorMadeUninteruptable.Value);
        }
    }

    public override void ChildDeactivated(DeactivationReason reason = null)
    {
        Deactivate(reason);
    }

    protected override bool InterruptOK()
    {
        return false;
    }
}