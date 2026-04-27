using UnityEngine;

[RequireComponent (typeof(SkiController))]
[RequireComponent (typeof(PlayerInteract))]
public class PlayerController : MonoBehaviour
{
    [SerializeField, Range(0, 0.9f)]
    private float m_skateDeadzone = 0.1f;

    [SerializeField, Range(0, 0.9f)]
    private float m_ploughDeadzone = 0.1f;

    [SerializeField]
    private GameObject m_journal;
    [SerializeField]
    private GameObject m_pauseMenu;

    private SkiController m_controller;
    private PlayerInteract m_interact;

    private void Awake()
    {
        m_controller = GetComponent<SkiController>();
        m_interact = GetComponent<PlayerInteract>();
    }

    private void Update()
    {
        HandleMovementInput();
        
        if (GameManager.Input.PlayerActions.Interact.WasPerformedThisFrame())
        {
            m_interact.Interact();
        }

        //BUG: Player movement is not properly zeroed
        if (GameManager.Input.Player.ToggleJournal)
        {
            GameManager.Input.PlayerActions.Disable();
            GameManager.Input.Player.ZeroInputs();
            m_journal.SetActive(true);
        }

        if (GameManager.Input.Player.Pause)
        {
            Time.timeScale = 0;
            m_pauseMenu.SetActive(true);
        }
    }

    private void HandleMovementInput()
    {
        if (!GameManager.Input.PlayerActions.Enabled) return;

        float edgeControlRaw = GameManager.Input.Player.EdgeControl;
        m_controller.EdgeControlInput = edgeControlRaw * edgeControlRaw;

        m_controller.PowerStopInput = GameManager.Input.Player.PowerStop;

        float turnRaw = GameManager.Input.Player.Move.x;
        m_controller.TurnInput = turnRaw * turnRaw * turnRaw;

        float yTopHalf = Mathf.Max(GameManager.Input.Player.Move.y, 0);
        m_controller.SkateInput = 1 / (1 - m_skateDeadzone) * (yTopHalf - m_skateDeadzone);

        float yBottomHalf = Mathf.Max(-GameManager.Input.Player.Move.y, 0);
        m_controller.PlowInput = 1 / (1 - m_ploughDeadzone) * (yBottomHalf - m_ploughDeadzone);

        
    }
}
