using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.OnScreen.OnScreenStick;

public enum BooleanOperator { AND, OR }
public class CompositeCondition : Condition {
    [SerializeReference] Condition[] conditions;
    [SerializeField] BooleanOperator booleanOperator = BooleanOperator.AND;
    [SerializeField] bool invert = false;

    protected override List<AITreeNode> GetChildren()
    {
        return new List<AITreeNode>(conditions);
    }

    public override bool Satisfied() {
        bool satisfiedBeforeInverstion = OtherConditionsSatisfied();
        if (!invert) {
            return satisfiedBeforeInverstion;
        }
        else {
            return !satisfiedBeforeInverstion;
        }
    }

    private bool OtherConditionsSatisfied() {
        if (booleanOperator == BooleanOperator.AND) {
            foreach (Condition c in conditions) {
                if (!c.Satisfied()) return false;
            }
            return true;
        }
        else if (booleanOperator == BooleanOperator.OR) {
            foreach (Condition c in conditions) {
                if (c.Satisfied()) return true;
            }
            return false;
        }
        Debug.Assert(false);
        return false;
    }
}
