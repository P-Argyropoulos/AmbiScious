using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static PlayerInputActions;


[CreateAssetMenu (menuName = "InputReader")]
public class InputReader : ScriptableObject, IPlayerActions
{
    public event UnityAction<Vector2> Move = delegate { };
    public event UnityAction<bool> Jump = delegate { };
    public event UnityAction<bool> Dash = delegate { };
    public event UnityAction<bool> Shoot = delegate { };

    public PlayerInputActions playerInputActions;

    public void EnablePlayerActions()
    {
        if (playerInputActions == null)
        {
            playerInputActions = new PlayerInputActions();
            playerInputActions.Player.SetCallbacks(this);            
        }
        playerInputActions.Enable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Move.Invoke(context.ReadValue<Vector2>());
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                    Jump.Invoke(true);
                    break;
            case InputActionPhase.Canceled:
                    Jump.Invoke(false);
                    break;
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                    Dash.Invoke(true);
                    break;
            case InputActionPhase.Canceled:
                    Dash.Invoke(false);
                    break;
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                    Shoot.Invoke(true);
                    break;
            case InputActionPhase.Canceled:
                    Shoot.Invoke(false);
                    break;
        }
    }
}

   

