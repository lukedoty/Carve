using MessagePack.Resolvers;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private InputActionAsset m_inputActions;

    private PlayerActions m_playerActions;
    public PlayerActions PlayerActions => m_playerActions;

    private PlayerInput m_playerInput;
    public PlayerInput Player => m_playerInput;

    private void Awake()
    {
        InputActionMap m_playerMap = m_inputActions.FindActionMap("Player", true);
        m_playerActions = new(m_playerMap);
    }

    private void Update()
    {
        m_playerInput.Update(m_playerActions);
    }
}

public readonly struct PlayerActions
{
    public readonly InputActionMap PlayerMap;
    public readonly InputAction Move;
    public readonly InputAction Look;
    public readonly InputAction Jump;
    public readonly InputAction EdgeControl;
    public readonly InputAction PowerStop;

    public PlayerActions(InputActionMap playerMap)
    {
        PlayerMap = playerMap;
        Move = playerMap.FindAction("Move", true);
        Look = playerMap.FindAction("Look", true);
        Jump = playerMap.FindAction("Jump", true);
        EdgeControl = playerMap.FindAction("Edge Control", true);
        PowerStop = playerMap.FindAction("Power Stop", true);
    }

    public readonly bool Enabled => PlayerMap.enabled;
    public readonly void Enable() => PlayerMap.Enable();
    public readonly void Disable() => PlayerMap.Disable();
}

public struct PlayerInput
{
    public Vector2 Move { get; private set; }
    public Vector2 Look { get; private set; }
    public bool Jump { get; private set; }
    public float EdgeControl { get; private set; }
    public bool PowerStop { get; private set; }

    public void Update(PlayerActions playerActions)
    {
        if (!playerActions.Enabled) return;

        Move = playerActions.Move.ReadValue<Vector2>();
        Look = playerActions.Look.ReadValue<Vector2>();
        Jump = playerActions.Jump.IsPressed();
        EdgeControl = playerActions.EdgeControl.ReadValue<float>();
        PowerStop = playerActions.PowerStop.IsPressed();
    }

    public void ZeroInputs()
    {
        Move = Vector2.zero;
        Look = Vector2.zero;
        Jump = false;
        EdgeControl = 0;
        PowerStop = false;
    }
}
