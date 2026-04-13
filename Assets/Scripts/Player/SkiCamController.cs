using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(SkiController))]
public class SkiCamController : MonoBehaviour
{
    [SerializeField]
    private CinemachineOrbitalFollow m_orbiter;

    private SkiController m_controller;

    private void Awake()
    {
        m_controller = GetComponent<SkiController>();
    }

    private void LateUpdate()
    {
        float angle = m_controller.transform.rotation.eulerAngles.y;
        m_orbiter.HorizontalAxis.Value = angle;
    }
}
