using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Written by Jacob Robinson, 5/14/24
public class InputManager : MonoBehaviour
{
    private static InputManager instance;
    public static InputManager Instance { get { return instance; } }

    public bool gamepad_enabled = false;
    private bool movement_enabled = true;
    private InputActions controls;

    private void Awake()
    {
        // Establishes InputManager as a singleton
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        // Initialize the controls
        if (controls == null)
        {
            controls = new InputActions();

            // If there are no controllers connected only use keyboard and mouse controls
            if (Gamepad.all.Count == 0)
            {
                controls.devices = new InputDevice[] { InputSystem.GetDevice<Keyboard>(), InputSystem.GetDevice<Mouse>() };
            }
            // Else use gamepad controls
            else
            {
                controls.devices = new InputDevice[] { InputSystem.GetDevice<Keyboard>(), InputSystem.GetDevice<Mouse>(), Gamepad.all[0] };
                gamepad_enabled = true;
            }
        }

        movement_enabled = true;
        controls.Player.Enable();
    }

    public Vector2 MoveInput()
    {
        if (movement_enabled)
        {
            return controls.Player.Move.ReadValue<Vector2>();
        }

        return new Vector2(0.0f, 0.0f);
    }

    public bool JumpInputPressed()
    {
        if (movement_enabled)
        {
            return controls.Player.Jump.WasPressedThisFrame();
        }

        return false;
    }

    public bool JumpInputReleased()
    {
        if (movement_enabled)
        {
            return controls.Player.Jump.WasReleasedThisFrame();
        }

        return false;
    }

    public bool AttackInputPressed()
    {
        if (movement_enabled)
        {
            return controls.Player.Attack.WasPressedThisFrame();
        }

        return false;
    }

    // Toggles the players ability to move
    // Jacob Robinson, 5/14/24
    public void ToggleMovement()
    {
        if (movement_enabled) { movement_enabled = false; }
        else { movement_enabled = true; }
    }
}
