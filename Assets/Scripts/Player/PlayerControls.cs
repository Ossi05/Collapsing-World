using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour {

    public static PlayerControls Instance;

    public event EventHandler OnPauseAction;

    void Awake()
    {
        Instance = this;
    }

    public Vector2 moveInput { get; private set; }
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();
        this.moveInput = moveInput;
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnPauseAction?.Invoke(this, EventArgs.Empty);
        }
    }

}
