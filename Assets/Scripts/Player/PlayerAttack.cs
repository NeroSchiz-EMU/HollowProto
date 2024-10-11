using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(PlayerAttackController))]
public abstract class PlayerAttack : MonoBehaviour
{
    PlayerMovement playerMovement = null;
    bool active = false;
    protected PlayerAttackController pac;
    public virtual void Awake()
    {
        if (InterruptJump())
        {
            playerMovement = GetComponent<PlayerMovement>();
        }
        pac = GetComponent<PlayerAttackController>();
    }

    public virtual bool CanStart()
    {
        return true;
    }

    public virtual bool CanStop()
    {
        return true;
    }

    public void StopIfPossible()
    {
        if (active && CanStop())
        {
            EndAttack();
            active = false;
        }
    }
    public virtual void Update()
    {
        StopIfPossible();
    }

    public bool InterruptJump()
    {
        return IsAirAttack();
    }

    public virtual bool IsAirAttack()
    {
        return true;
    }

    public bool CurrentlyInUse()
    {
        return active;
    }

    public bool TryAttack(Vector2 direction)
    {
        if (!CanStart()) return false;
            
        if (InterruptJump())
        {
            playerMovement.InterruptJump();
        }
        AttackStart(direction);
        active = true;
        StopIfPossible();
        return true;
    }
    public abstract void AttackStart(Vector2 direction);
    public virtual void AttackEnd() { }
    protected void EndAttack()
    {
            AttackEnd();
        pac.AttackEnded();
    }
}
