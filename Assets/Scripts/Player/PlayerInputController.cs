using UnityEngine;
using UnityEngine.InputSystem;

//PROTOTYPE ONLY: NEEDS RE-WRITTING
public class PlayerInputController : MonoBehaviour
{
    private Vector2 moveDir;
    private bool isJumpDown = false;
    private bool isAttackDown = false;
	public void OnMoveCalled(InputAction.CallbackContext ctx)
	{
		moveDir = ctx.ReadValue<Vector2>();
	}

    public void OnJumpCalled(InputAction.CallbackContext ctx)
    {
        isJumpDown = AxisEmulateBool(ctx); 
    }

    public void OnAttackCalled(InputAction.CallbackContext ctx)
    {
        isAttackDown = AxisEmulateBool(ctx);
	}


	public float MoveInputX()
    {
        return moveDir.x;
    }
    

    public bool JumpInputPressed()
    {
        return Input.GetKeyDown(KeyCode.Z);
    }

    public bool JumpInputHeld()
    {
        return isJumpDown;
    }

    public bool AttackInputHeld()
    {
        return isAttackDown;    // do we need this?
    }

    public Vector2 AttackDirection()
    {
        return moveDir;
    }

    bool AxisEmulateBool(InputAction.CallbackContext ctx)
    {
        return ctx.ReadValue<float>() > 0.9;     // allows axis controls to work as buttons

	}
}