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

    [Header("Look Input")]
    [SerializeField] private float lookVerticalRange = 15f;
    [SerializeField] private float lookSmoothSharpness = 6f;

    [Header("Collision")]
    [SerializeField] private LayerMask collisionLayers = ~0;
    [SerializeField] private float cameraRadius = 0.3f;

    private Vector3 _smoothedPlanarForward = Vector3.forward;
    private float _currentAngle;
    private float _smoothedVerticalOffsetAngle;
    private float _smoothedLookOffset;

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

        EnsureCameraCollision();
    }

    private void EnsureCameraCollision()
    {
        GameObject vcamGo = m_orbiter.gameObject;
        if (vcamGo.TryGetComponent<CinemachineDeoccluder>(out _)) return;

        var d = vcamGo.AddComponent<CinemachineDeoccluder>();
        d.CollideAgainst = collisionLayers;
        d.IgnoreTag = "Player";
        d.MinimumDistanceFromTarget = 0.3f;
        d.AvoidObstacles = new CinemachineDeoccluder.ObstacleAvoidance
        {
            Enabled = true,
            CameraRadius = cameraRadius,
            Strategy = CinemachineDeoccluder.ObstacleAvoidance.ResolutionStrategy.PullCameraForward,
            MaximumEffort = 4,
            Damping = 0.4f,
            DampingWhenOccluded = 0.2f,
        };
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
        Vector3 desiredForward;
        bool haveDesired = false;

        if (m_controller.IsSkating)
        {
            Vector3 facing = m_controller.transform.forward;
            facing.y = 0f;
            if (facing.sqrMagnitude > 0.0001f)
            {
                desiredForward = facing.normalized;
                haveDesired = true;
            }
            else desiredForward = _smoothedPlanarForward;
        }
        else
        {
            Vector3 planarVelocity = velocity;
            planarVelocity.y = 0f;
            if (planarVelocity.sqrMagnitude > minSpeedToUpdateDirection * minSpeedToUpdateDirection)
            {
                desiredForward = planarVelocity.normalized;
                haveDesired = true;
            }
            else desiredForward = _smoothedPlanarForward;
        }

        if (haveDesired)
        {
            float t = 1f - Mathf.Exp(-directionSmoothSharpness * dt);
            _smoothedPlanarForward = Vector3.Slerp(_smoothedPlanarForward, desiredForward, t);
            _smoothedPlanarForward.y = 0f;
            if (_smoothedPlanarForward.sqrMagnitude > 0.0001f)
                _smoothedPlanarForward.Normalize();
        }

        _currentAngle = Mathf.Atan2(_smoothedPlanarForward.x, _smoothedPlanarForward.z) * Mathf.Rad2Deg;
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

        // Right-stick adds a small additive offset; negative so pushing up = look up.
        float lookY = GameManager.Input.Player.Look.y;
        float targetLookOffset = -lookY * lookVerticalRange;
        float lookT = 1f - Mathf.Exp(-lookSmoothSharpness * dt);
        _smoothedLookOffset = Mathf.Lerp(_smoothedLookOffset, targetLookOffset, lookT);

        m_orbiter.VerticalAxis.Value = baseVerticalAngle + _smoothedVerticalOffsetAngle + _smoothedLookOffset;
    }

    public void ResetSmoothing()
    {
        Vector3 facing = m_controller.transform.forward;
        facing.y = 0f;
        if (facing.sqrMagnitude > 0.0001f) _smoothedPlanarForward = facing.normalized;
        _currentAngle = Mathf.Atan2(_smoothedPlanarForward.x, _smoothedPlanarForward.z) * Mathf.Rad2Deg;
        _smoothedVerticalOffsetAngle = 0f;
        _smoothedLookOffset = 0f;
        m_orbiter.HorizontalAxis.Value = _currentAngle;
        m_orbiter.VerticalAxis.Value = baseVerticalAngle;
        if (m_orbiter.TryGetComponent<CinemachineCamera>(out var cam))
            cam.PreviousStateIsValid = false;
    }
}
