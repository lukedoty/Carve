using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(SkiController))]
public class SkiCamController : MonoBehaviour
{
    [SerializeField] private CinemachineOrbitalFollow m_orbiter;
    private SkiController m_controller;

    [Header("Horizontal")]
    // smaller value is smoother
    [SerializeField] private float directionSmoothSharpness = 4f;
    [SerializeField] private float minSpeedToUpdateDirection = 0.5f;

    [Header("Vertical")]
    [SerializeField] private float baseVerticalAngle = 20f;
    [SerializeField] private float verticalSmoothSharpness = 4f;
    [SerializeField] private float maxVerticalOffsetAngle = 25f;

    private Vector3 _smoothedPlanarForward = Vector3.forward;
    private float _currentAngle;
    private float _smoothedVerticalOffsetAngle;

    private void Awake()
    {
        m_controller = GetComponent<SkiController>();

        Vector3 initialForward = transform.forward;
        initialForward.y = 0f;
        if (initialForward.sqrMagnitude > 0.0001f)
            _smoothedPlanarForward = initialForward.normalized;

        _currentAngle = Mathf.Atan2(_smoothedPlanarForward.x, _smoothedPlanarForward.z) * Mathf.Rad2Deg;
        _smoothedVerticalOffsetAngle = 0f;

        m_orbiter.HorizontalAxis.Value = _currentAngle;
        m_orbiter.VerticalAxis.Value = baseVerticalAngle;
    }

    private void LateUpdate()
    {
        float dt = Time.deltaTime;
        if (dt <= 0f)
            return;

        Vector3 velocity = m_controller.Velocity;

        UpdateHorizontal(velocity, dt);
        UpdateVertical(velocity, dt);
    }

    private void UpdateHorizontal(Vector3 velocity, float dt)
    {
        if (m_controller.IsSkating)
        {
            float angle = m_controller.transform.rotation.eulerAngles.y;
            m_orbiter.HorizontalAxis.Value = angle;
            return;
        }

        Vector3 planarVelocity = velocity;
        planarVelocity.y = 0f;

        float planarSpeedSq = planarVelocity.sqrMagnitude;
        if (planarSpeedSq > minSpeedToUpdateDirection * minSpeedToUpdateDirection)
        {
            Vector3 desiredForward = planarVelocity.normalized;

            float t = 1f - Mathf.Exp(-directionSmoothSharpness * dt);
            _smoothedPlanarForward = Vector3.Slerp(_smoothedPlanarForward, desiredForward, t);
            _smoothedPlanarForward.y = 0f;

            if (_smoothedPlanarForward.sqrMagnitude > 0.0001f)
                _smoothedPlanarForward.Normalize();
        }

        float targetAngle = Mathf.Atan2(_smoothedPlanarForward.x, _smoothedPlanarForward.z) * Mathf.Rad2Deg;
        _currentAngle = targetAngle;
        m_orbiter.HorizontalAxis.Value = _currentAngle;
    }

    private void UpdateVertical(Vector3 velocity, float dt)
    {
        Vector3 planarVelocity = velocity;
        planarVelocity.y = 0f;

        float planarSpeed = planarVelocity.magnitude;
        float targetVerticalOffsetAngle = 0f;

        if (planarSpeed > 0.01f)
        {
            float pitchFromVelocity = -Mathf.Atan2(velocity.y, planarSpeed) * Mathf.Rad2Deg;
            targetVerticalOffsetAngle = Mathf.Clamp(
                pitchFromVelocity,
                -maxVerticalOffsetAngle,
                maxVerticalOffsetAngle
            );
        }

        float t = 1f - Mathf.Exp(-verticalSmoothSharpness * dt);
        _smoothedVerticalOffsetAngle = Mathf.Lerp(
            _smoothedVerticalOffsetAngle,
            targetVerticalOffsetAngle,
            t
        );

        _smoothedVerticalOffsetAngle = Mathf.Clamp(
            _smoothedVerticalOffsetAngle,
            -maxVerticalOffsetAngle,
            maxVerticalOffsetAngle
        );

        m_orbiter.VerticalAxis.Value = baseVerticalAngle + _smoothedVerticalOffsetAngle;
    }
}
