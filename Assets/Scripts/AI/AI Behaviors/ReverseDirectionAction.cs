using UnityEngine;

public class ReverseDirectionAction : AIBehavior {
    [SerializeField] Direction direction;

    protected override void ActivationAction() {
        direction.Reverse();
        Deactivate(new ActionCompletedReason());
    }
}
