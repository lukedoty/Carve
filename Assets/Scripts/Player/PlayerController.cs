using UnityEngine;

[RequireComponent (typeof(SkiController))]
public class PlayerController : MonoBehaviour
{
    private SkiController m_controller;

    private void Awake()
    {
        m_controller = GetComponent<SkiController>();
    }

    private void Update()
    {
        float edgeControlRaw = GameManager.Input.Player.EdgeControl;
        m_controller.EdgeControl = edgeControlRaw * edgeControlRaw;

        float turnRaw = GameManager.Input.Player.Move.x;
        m_controller.Turn = turnRaw * turnRaw * turnRaw;
    }
}
