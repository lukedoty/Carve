using UnityEngine;

[RequireComponent (typeof(CharacterController))]
public class SkiController : MonoBehaviour
{
    [Header("Turning")]

    [SerializeField, Range(0, 1)]
    private float m_edgeControlInput = 0.5f;
    public float EdgeControlInput
    { 
        get { return m_edgeControlInput; } 
        set { m_edgeControlInput = Mathf.Clamp(value, 0, 1); }
    }

    [SerializeField, Range(-1, 1)]
    private float m_turnInput = 0;
    public float TurnInput
    {
        get { return m_turnInput; }
        set { m_turnInput = Mathf.Clamp(value, -1, 1); }
    }

    [SerializeField]
    private float m_turnSpeed = 90.0f;

    [Header("Power Stop")]

    [SerializeField]
    private bool m_powerStopInput;
    public bool PowerStopInput
    {
        get { return m_powerStopInput; }
        set { m_powerStopInput = value; }
    }

    [SerializeField]
    private float m_powerStopSpeedRequirement = 2.5f;

    [SerializeField, Range(0, 120)]
    private float m_powerStopMaxAngle = 90.0f;

    private bool m_isPowerStopping;
    public bool IsPowerStopping => m_isPowerStopping;

    private Vector3 m_powerStopVel;

    [Header("Skating")]

    [SerializeField, Range(0, 1)]
    private float m_skateInput;
    public float SkateInput
    {
        get { return m_skateInput; }
        set { m_skateInput = Mathf.Clamp(value, 0, 1); }
    }

    [SerializeField]
    private float m_skateForce = 1.0f;

    [SerializeField]
    private float m_skatingSpeedLimit = 1.5f;

    [SerializeField]
    private float m_skateSlopeLimit = 10.0f;

    [Header("Friction")]

    [SerializeField, Range(0, 1)]
    private float m_plowInput;
    public float PlowInput
    {
        get { return m_plowInput; }
        set { m_plowInput = value; }
    }

    [SerializeField]
    private float m_staticFrictionCutoffSpeed = 0.01f;
    public float StaticFrictionCutoffSpeed => m_staticFrictionCutoffSpeed;

    [SerializeField]
    private Vector2 m_parallelFrictionRange = new(0.1f, 0.6f);

    [SerializeField]
    private Vector2 m_perpendicularFrictionRange = new(0.6f, 1.2f);

    [Header("Misc")]

    [SerializeField]
    private Vector3 m_accelerationGravity = Vector3.down * 9.8f;

    [SerializeField]
    private bool m_isFrozen;


    private CharacterController m_controller;

    private Vector3 m_acc;
    public Vector3 Acceleration => m_acc;

    private Vector3 m_vel;
    public Vector3 Velocity => m_vel;

    private bool m_isGrounded;
    public bool IsGrounded => m_isGrounded;

    private bool m_isSkating;
    public bool IsSkating => m_isSkating;

    private Vector3 m_groundNormal = Vector3.up;
    public Vector3 GroundNormal => m_groundNormal;

    private Vector3 m_lastGroundedGroundNormal = Vector3.up;
    public Vector3 LastGroundedGroundNormal => m_lastGroundedGroundNormal;

    private void Awake()
    {
        m_controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (m_isFrozen) return;

        HandleMovement();
        HandleTurning();
    }

    private void HandleMovement()
    {
        if (m_isGrounded = GroundedCheck(out m_groundNormal, out _))
        {
            m_lastGroundedGroundNormal = m_groundNormal;

            Vector3 gravitationalForce = Vector3.ProjectOnPlane(m_accelerationGravity, m_groundNormal);
            Vector3 skatingForce = SkatingForce(out m_isSkating);
            Vector3 appliedForce = gravitationalForce + skatingForce;

            Vector3 frictionForce = FrictionForce(appliedForce, out _, out _);

            m_acc = appliedForce + frictionForce;
        }
        else
        {
            m_isSkating = false;
            m_acc = m_accelerationGravity;
        }

        m_vel += m_acc * Time.deltaTime;
        m_controller.Move(m_vel * Time.deltaTime);
    }

    private void HandleTurning()
    {
        if (m_powerStopInput && m_isGrounded)
        {
            Vector3 newPowerStopVel = Vector3.ProjectOnPlane(m_vel, m_groundNormal);
            if (!m_isPowerStopping && newPowerStopVel.sqrMagnitude > m_powerStopSpeedRequirement * m_powerStopSpeedRequirement)
            {
                m_isPowerStopping = true;
                m_powerStopVel = newPowerStopVel;
            }
        }
        else
        {
            m_isPowerStopping = false;
        }

        const float k_PowerStopRotSharpness = 14f;
        if (m_isPowerStopping)
        {
            float baseAngle = Vector3.SignedAngle(Vector3.forward, m_powerStopVel, Vector3.up);
            float targetAngle = m_turnInput * m_powerStopMaxAngle + baseAngle;
            Quaternion target = Quaternion.AngleAxis(targetAngle, Vector3.up);
            float t = 1f - Mathf.Exp(-k_PowerStopRotSharpness * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, t);
        }
        else
        {
            float angleStep = m_turnInput * m_turnSpeed * Time.deltaTime;
            transform.rotation = transform.rotation * Quaternion.AngleAxis(angleStep, Vector3.up);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        m_vel = Vector3.ProjectOnPlane(m_vel, hit.normal);
    }

    private Vector3 SkatingForce(out bool isSkating)
    {
        if (!SkatableCheck() || m_isPowerStopping)
        {
            isSkating = false;
            return Vector3.zero;
        }

        isSkating = true;

        if (m_vel.sqrMagnitude <= m_skatingSpeedLimit * m_skatingSpeedLimit)
        {
            Vector3 direction = Vector3.ProjectOnPlane(transform.forward, m_groundNormal).normalized;
            return m_skateInput * m_skateForce * direction;
        }

        return Vector3.zero;
    }

    private bool SkatableCheck() => Vector3.Angle(m_groundNormal, Vector3.up) <= m_skateSlopeLimit;

    private Vector3 FrictionForce(Vector3 appliedForce, out Vector3 parallelFricForce, out Vector3 perpendicularFricForce)
    {
        Vector3 normalForce = appliedForce - m_accelerationGravity;
        float normalForceMag = normalForce.magnitude;

        parallelFricForce = ParallelFrictionForce(appliedForce, normalForceMag);
        perpendicularFricForce = PerpendicularFrictionForce(appliedForce, normalForceMag);

        return parallelFricForce + perpendicularFricForce;
    }

    private Vector3 ParallelFrictionForce(Vector3 appliedForce, float normalForceMag)
    {
        Vector3 parallelUnit = Vector3.ProjectOnPlane(transform.forward, m_groundNormal).normalized;
        Vector3 parallelVel = Vector3.Project(m_vel, parallelUnit);
        if (parallelVel.sqrMagnitude < m_staticFrictionCutoffSpeed * m_staticFrictionCutoffSpeed)
        {
            Vector3 parallelAppliedForce = Vector3.Project(appliedForce, parallelUnit);
            float parallelFricForceMag = Mathf.Min(GetParallelFriction() * normalForceMag, parallelAppliedForce.magnitude);
            return parallelFricForceMag * -parallelAppliedForce.normalized;
        }
        else
        {
            float parallelFricForceMag = GetParallelFriction() * normalForceMag;
            return parallelFricForceMag * -parallelVel.normalized;
        }
    }

    private Vector3 PerpendicularFrictionForce(Vector3 appliedForce, float normalForceMag)
    {
        Vector3 perpendicularUnit = Vector3.ProjectOnPlane(transform.right, m_groundNormal).normalized;
        Vector3 perpendicularVel = Vector3.Project(m_vel, perpendicularUnit);
        if (perpendicularVel.sqrMagnitude < m_staticFrictionCutoffSpeed * m_staticFrictionCutoffSpeed)
        {
            Vector3 perpendicularAppliedForce = Vector3.Project(appliedForce, perpendicularUnit);
            float perpendicularFricForceMag = Mathf.Min(GetPerpendicularFriction() * normalForceMag, perpendicularAppliedForce.magnitude);
            return perpendicularFricForceMag * -perpendicularAppliedForce.normalized;
        }
        else
        {
            float perpendicularFricForceMag = GetPerpendicularFriction() * normalForceMag;
            return perpendicularFricForceMag * -perpendicularVel.normalized;
        }
    }

    private float GetParallelFriction()
    {
        float t = m_plowInput;
        return Mathf.Lerp(m_parallelFrictionRange.x, m_parallelFrictionRange.y, t);
    }

    private float GetPerpendicularFriction()
    {
        float t = m_isPowerStopping ? 1.0f : Mathf.Max(m_edgeControlInput, m_plowInput);
        return Mathf.Lerp(m_perpendicularFrictionRange.x, m_perpendicularFrictionRange.y, t);
    }

    private bool GroundedCheck(out Vector3 groundNormal, out RaycastHit groundHitInfo)
    {
        bool hit = Physics.SphereCast(
            transform.position + Vector3.up * m_controller.radius,
            m_controller.radius,
            Vector3.down,
            out groundHitInfo,
            m_controller.skinWidth * 2
        );
        
        groundNormal = hit ? groundHitInfo.normal : Vector3.up;
        return hit;
    }

    public void ZeroVelocityAndAcceleration()
    {
        m_vel = Vector3.zero;
        m_acc = Vector3.zero;
    }

    public void Freeze() => m_isFrozen = true;

    public void Unfreeze() => m_isFrozen = false;

    public void MovePlayer(Vector3 newPos)
    {
        gameObject.transform.position = newPos;
    }
}
