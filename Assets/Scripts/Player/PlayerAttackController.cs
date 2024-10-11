using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Physics))]
public class PlayerAttackController : MonoBehaviour
{
    [SerializeField] PlayerAttack up;
    [SerializeField] PlayerAttack upRight;
    [SerializeField] PlayerAttack right;
    [SerializeField] PlayerAttack downRight;
    [SerializeField] PlayerAttack down;

    [SerializeField] float groundedMinRefreshTime = 0.3f;
    [SerializeField] float minTimeBetweenAttacks = 0.3f;

    // enabled and disable attacking
    public bool attackingEnabled = true;

    bool currentlyAttacking = false;
    float timeOfLastAirAttack = Mathf.NegativeInfinity;
    bool airAttackReady = false;

    InputManager input;
    Physics physics;

    private void Awake()
    {
        // TODO: Assert that an Input Manager must always exists for this to work.
        input = GameObject.Find("InputManager").GetComponent<InputManager>();
        physics = GetComponent<Physics>();
    }

    public void AttackEnded()
    {
        currentlyAttacking = false;
    }

    public bool AirAttackReady()
    {
        return airAttackReady;
    }

    public void CheckAirRefresh()
    {
        if (Time.time - timeOfLastAirAttack < groundedMinRefreshTime) return;
        if (physics.IsGrounded())
        {
            airAttackReady = true;
        };
    }

    public void AirAttackRefresh()
    {
        airAttackReady = true;
    }

    public void Update()
    {
        CheckAirRefresh();

        if (input.AttackInputPressed())
        {
            CallAttack();
        }
    }

    // Called whenever the player attacks
    public void CallAttack()
	{
        if (currentlyAttacking || Time.time - timeOfLastAirAttack < minTimeBetweenAttacks) return;
        if (!attackingEnabled) return;

        var attackDirection = GetAttackAndDirection(input.MoveInput());
        var attack = attackDirection.Item1;
        var direction = attackDirection.Item2;

        if (attack.IsAirAttack() && !AirAttackReady()) return;
        if (attack.TryAttack(direction))
        {
            currentlyAttacking = true;
            if (attack.IsAirAttack())
            {
                timeOfLastAirAttack = Time.time;
                airAttackReady = false;
            }
        }
    }

	(PlayerAttack, Vector2) GetAttackAndDirection(Vector2 direction)
    {
        if (direction == Vector2.zero)
        {
            // Get the direction the player is facing
            if (Mathf.Abs(transform.rotation.y - 0) < 0.001f)
            {
                direction = Vector2.right;
            }
            else
            {
                direction = Vector2.left;
            }
        }
        float xSign = Mathf.Sign(direction.x);
        direction.x = Mathf.Abs(direction.x);
        direction.Normalize();


        Vector2[] directions = { Vector2.up, new Vector2(1,1), Vector2.right, new Vector2(1, -1), Vector2.down };
        for (int i = 0; i < directions.Length; i++)
        {
            directions[i].Normalize();
        }

        PlayerAttack[] attacks = { up, upRight, right, downRight, down };

        for (int i = 0; i < attacks.Length; i++)
        {
            if (Vector2.Dot(direction, directions[i]) >= 0.923) // 0.923 is approximately cos(pi/8)
            {

                directions[i].x *= xSign;
                return (attacks[i], directions[i]);
            }
        }
        Debug.LogError("Attack input direction invalid");
        return (null, Vector2.zero);
    }
}
