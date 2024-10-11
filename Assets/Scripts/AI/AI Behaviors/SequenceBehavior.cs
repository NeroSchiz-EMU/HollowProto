using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChildrenFinishedReason : DeactivationReason { }

[System.Serializable]
public class SequenceBehavior : AIBehavior {
    [SerializeReference] AIBehavior[] behaviors;
    [SerializeField] bool loopAfterFinishingSequence = true;

    int index = 0;

    protected override List<AITreeNode> GetChildren()
    {
        return new List<AITreeNode>(behaviors);
    }
    protected override void ActivationAction() {
        index = 0;
        SetChildBehavior(behaviors[index]);
    }

    public override void ChildDeactivated(DeactivationReason reason = null) {
        GiveParentChanceToInterrupt(); if (!IsActive()) return;
        index++;
        if (index >= behaviors.Length) {
            if (loopAfterFinishingSequence) {
                index = 0;
            }
            else {
                Deactivate(new ChildrenFinishedReason());
                return;
            }
        }

        SetChildBehavior(behaviors[index]);
    }
}
