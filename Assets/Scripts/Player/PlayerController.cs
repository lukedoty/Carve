using UnityEngine;

[RequireComponent (typeof(SkiController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField, Range(0, 0.9f)]
    private float m_skateDeadzone = 0.1f;

    [SerializeField, Range(0, 0.9f)]
    private float m_ploughDeadzone = 0.1f;

    private SkiController m_controller;

    private void Awake()
    {
        m_controller = GetComponent<SkiController>();
    }

    private void Update()
    {
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
