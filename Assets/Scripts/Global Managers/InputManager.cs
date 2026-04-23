using MessagePack.Resolvers;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private InputActionAsset m_inputActions;

    private PlayerActions m_playerActions;
    public PlayerActions PlayerActions => m_playerActions;

    private PlayerInput m_playerInput;
    public PlayerInput Player => m_playerInput;

    private UIActions m_uiActions;
    public UIActions UIActions => m_uiActions;
    private UIInput m_uiInput;
    public UIInput UI => m_uiInput;


    private void Awake()
    {
        InputActionMap m_playerMap = m_inputActions.FindActionMap("Player", true);
        m_playerActions = new(m_playerMap);
        InputActionMap m_uiMap = m_inputActions.FindActionMap("UI", true);
        m_uiActions = new(m_uiMap);
    }

    private void Update()
    {
        m_playerInput.Update(m_playerActions);
        m_uiInput.Update(m_uiActions);
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
    public readonly InputAction Interact;
    public readonly InputAction ToggleJournal;

    public PlayerActions(InputActionMap playerMap)
    {
        PlayerMap = playerMap;
        Move = playerMap.FindAction("Move", true);
        Look = playerMap.FindAction("Look", true);
        Jump = playerMap.FindAction("Jump", true);
        EdgeControl = playerMap.FindAction("Edge Control", true);
        PowerStop = playerMap.FindAction("Power Stop", true);
        Interact = playerMap.FindAction("Interact", true);
        ToggleJournal = playerMap.FindAction("Toggle Journal", true);
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
    public bool Interact { get; private set; }
    public bool ToggleJournal { get; private set; }

    public void Update(PlayerActions playerActions)
    {
        if (!playerActions.Enabled) return;

        Move = playerActions.Move.ReadValue<Vector2>();
        Look = playerActions.Look.ReadValue<Vector2>();
        Jump = playerActions.Jump.IsPressed();
        EdgeControl = playerActions.EdgeControl.ReadValue<float>();
        PowerStop = playerActions.PowerStop.IsPressed();
        Interact = playerActions.Interact.IsPressed();
        ToggleJournal = playerActions.ToggleJournal.WasPressedThisFrame();
    }

    public void ZeroInputs()
    {
        Move = Vector2.zero;
        Look = Vector2.zero;
        Jump = false;
        EdgeControl = 0;
        PowerStop = false;
        Interact = false;
    }
}

public readonly struct UIActions
{
    public readonly InputActionMap UIMap;
    public readonly InputAction ToggleLog;
    public readonly InputAction Scroll;
    public readonly InputAction Navigate;
    public readonly InputAction Select;
    public readonly InputAction ToggleJournal;
    public UIActions(InputActionMap uiMap)
    {
        UIMap = uiMap;
        ToggleLog = uiMap.FindAction("Toggle Log", true);
        Scroll = uiMap.FindAction("Scroll", true);
        Navigate = uiMap.FindAction("Navigate", true);
        Select = uiMap.FindAction("Select", true);
        ToggleJournal = uiMap.FindAction("Toggle Journal", true);
    }

    public readonly bool Enabled => UIMap.enabled;
    public readonly void Enable() => UIMap.Enable();
    public readonly void Disable() => UIMap.Disable();
}

public struct UIInput
{
    public bool ToggleLog { get; private set; }
    public float Scroll { get; private set; }
    public Vector2 Navigate { get; private set; }
    public bool Select { get; private set; }
    public bool ToggleJournal { get; private set; }
    public void Update(UIActions uiActions)
    {
        if (!uiActions.UIMap.enabled) return;

        ToggleLog = uiActions.ToggleLog.WasPressedThisFrame();
        Scroll = uiActions.Scroll.ReadValue<float>();
        Navigate = uiActions.Navigate.ReadValue<Vector2>();
        Select = uiActions.Navigate.WasPressedThisFrame();
    }
}
