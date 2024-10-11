using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public struct PossibleBehavior {
    [SerializeReference] public AIBehavior behavior;
    [SerializeField] public float weight;
    [SerializeReference] public Condition usableCondition;
}

[System.Serializable]
public class RandomizeBehavior : AIBehavior {
    [SerializeField] bool repeat = false;
    [SerializeField] PossibleBehavior[] behaviors;

    protected override List<AITreeNode> GetChildren()
    {
        var children = new List<AITreeNode>();
        foreach (var behavior in behaviors)
        {
            children.Add(behavior.behavior);
            if (behavior.usableCondition != null)
            {
                children.Add(behavior.usableCondition);
            }
        }
        return children;
    }
    private int RandomBehaviorIndex() {
        float totalWeight = 0f;
        List<int> eligibleBehaviorIndices = new List<int>();
        for (int i = 0; i < behaviors.Length; i++) {
            if (behaviors[i].usableCondition == null || behaviors[i].usableCondition.Satisfied()) {
                totalWeight += behaviors[i].weight;
                eligibleBehaviorIndices.Add(i);
            }
        }

        float randomValue = UnityEngine.Random.value * totalWeight;
        for (int i = 0; i < eligibleBehaviorIndices.Count; i++) {
            int behaviorIndex = eligibleBehaviorIndices[i];
            randomValue -= behaviors[behaviorIndex].weight;
            if (randomValue <= 0f) {
                return behaviorIndex;
            }
        }

        Debug.LogError("RandomizeBehavior: Failed to choose a behavior");
        return 0;
    }

    private void TakeRandomBehavior() {
        SetChildBehavior(behaviors[RandomBehaviorIndex()].behavior);
    }
    protected override void ActivationAction() {
        TakeRandomBehavior();
    }

    public override void ChildDeactivated(DeactivationReason reason = null) {
        if (repeat) {
            GiveParentChanceToInterrupt(); if (!IsActive()) return;
            TakeRandomBehavior();
        }
        else {
            Deactivate(new ChildrenFinishedReason());
        }
    }
}
