using System.Collections;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;


public class DeactivationReason { }
public class ParentWasDeactivatedReason : DeactivationReason { }
public class ParentChangedChildReason : DeactivationReason { }
public class ActionCompletedReason : DeactivationReason { } //General reason for whenever the behavior immediately deactivates itself
public class TimedOutReason : DeactivationReason { }

public abstract class AITreeNode
{
    protected AITreeNode parent = null;
    protected AIBehaviorController controller = null;
    public static readonly bool DEBUG = false;
    public void SetParent(AITreeNode parent)
    {
        this.parent = parent;
    }
    public void SetController(AIBehaviorController controller)
    {
        this.controller = controller;
    }

    public List<AITreeNode> Subtree()
    {
        List<AITreeNode> subtree = new List<AITreeNode> { this };
        if (DEBUG) Debug.Log($"Node type: {this.GetType().Name} explored with children:");
        foreach (AITreeNode child in GetChildren())
        {
            if (DEBUG) Debug.Log(this.GetType().Name);
            child.SetParent(this);
            subtree.AddRange(child.Subtree());
        }
        return subtree;
    }

    protected virtual List<AITreeNode> GetChildren()
    {
        return new List<AITreeNode>();
    }
}

[System.Serializable]
public abstract class AIBehavior : AITreeNode
{
    bool active = false;
    float timeOfLastActivation;

    AIBehavior activeChild = null;

    float deactivateAfterSeconds = Mathf.Infinity;

    protected AIBehavior Parent()
    {
        if (parent == null) return null;
        if (parent is AIBehavior AIBehaviorParent)
        {
            return AIBehaviorParent;
        }
        Debug.LogError($"AIBehavior {this.GetType().Name} does not have an AIBehavior as parent");
        return null;
    }

    protected virtual bool InterruptOK()
    {
        if (activeChild != null) return activeChild.InterruptOK();
        return true;
    }
    protected virtual void GiveParentChanceToInterrupt()
    {
        if (Parent() != null) Parent().WhenChildInterruptible();
    }
    protected virtual void WhenChildInterruptible() { }


    public void ChildDeactivatedPossiblyByParent(DeactivationReason reason = null)
    {
        if (!active) return;
        if (reason is ParentWasDeactivatedReason) return;
        if (reason is ParentChangedChildReason) return;
        ChildDeactivated(reason);
    }

    public virtual void ChildDeactivated(DeactivationReason reason = null) { }

    protected void SetChildBehavior(AIBehavior newBehavior, float seconds)
    {
        SetChildBehavior(newBehavior);
        newBehavior.DeactivateAfterSeconds(seconds);
    }

    protected void SetChildBehavior(AIBehavior newBehavior)
    {
        if (activeChild == newBehavior && activeChild.IsActive()) return;
        if (activeChild != null) activeChild.Deactivate(new ParentChangedChildReason());
        activeChild = newBehavior;
        newBehavior.Activate();
    }
    public bool IsActive()
    {
        return active;
    }

    public void DeactivateAfterSeconds(float seconds)
    {
        deactivateAfterSeconds = Mathf.Min(deactivateAfterSeconds, seconds);
    }

    public float SecondsActive()
    {
        return Time.time - timeOfLastActivation;
    }

    public void Activate()
    {
        if (active) return;
        active = true;
        timeOfLastActivation = Time.time;
        ActivationAction();
    }

    public void Deactivate(DeactivationReason reason = null)
    {
        if (AITreeNode.DEBUG) Debug.Log($"AIBehavior {this.GetType().Name} deactivated for reason {reason.GetType().Name}");
        if (!active) return;
        DeactivationAction();
        deactivateAfterSeconds = Mathf.Infinity;
        if (activeChild != null) activeChild.Deactivate(new ParentWasDeactivatedReason());
        active = false;
        if (Parent() != null) Parent().ChildDeactivatedPossiblyByParent(reason);
    }

    protected Transform Transform()
    {
        return controller.transform;
    }

    protected Collider2D Collider()
    {
        return controller.GetComponent<Collider2D>();
    }

    protected virtual void ActivationAction() { }

    protected virtual void UpdateAction() { }

    protected virtual void DeactivationAction() { }


    public void Update()
    {
        if (!active) return;

        if (SecondsActive() >= deactivateAfterSeconds)
        {
            Deactivate(new TimedOutReason());
            return;
        }

        if (InterruptOK())
        {
            GiveParentChanceToInterrupt();
            if (!active) return;
        }

        if (activeChild != null) activeChild.Update();
        UpdateAction();
    }
}