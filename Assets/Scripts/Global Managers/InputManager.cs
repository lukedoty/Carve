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

    public PlayerActions(InputActionMap playerMap)
    {
        PlayerMap = playerMap;
        Move = playerMap.FindAction("Move", true);
        Look = playerMap.FindAction("Look", true);
        Jump = playerMap.FindAction("Jump", true);
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

    public void Update(PlayerActions playerActions)
    {
        if (!playerActions.PlayerMap.enabled) return;

        Move = playerActions.Move.ReadValue<Vector2>();
        Look = playerActions.Look.ReadValue<Vector2>();
        Jump = playerActions.Jump.IsPressed();
    }
}
