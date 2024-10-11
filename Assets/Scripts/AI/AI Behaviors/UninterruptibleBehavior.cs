using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UninterruptibleBehavior : AIBehavior
{
    [SerializeField] Optional<AIBehavior> behaviorMadeUninterruptible;

    protected override List<AITreeNode> GetChildren()
    {
        var children = new List<AITreeNode>();
        if (behaviorMadeUninterruptible.Enabled)
        {
            children.Add(behaviorMadeUninterruptible.Value);
        }
        return children;
    }
    protected override void ActivationAction()
    {
        if (behaviorMadeUninterruptible.Enabled)
        {
            SetChildBehavior(behaviorMadeUninterruptible.Value);
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
    protected override void GiveParentChanceToInterrupt()
    {
        return;
    }
}