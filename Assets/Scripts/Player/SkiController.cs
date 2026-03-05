using UnityEngine;

[RequireComponent (typeof(CharacterController))]
public class SkiController : MonoBehaviour
{
    [Header("Turning")]

    [SerializeField, Range(0, 1)]
    private float m_edgeControl = 0.5f;
    public float EdgeControl
    { 
        get { return m_edgeControl; } 
        set { m_edgeControl = Mathf.Clamp(value, 0, 1); }
    }

    [SerializeField, Range(-1, 1)]
    private float m_turn = 0;
    public float Turn
    {
        get { return m_turn; }
        set { m_turn = Mathf.Clamp(value, -1, 1); }
    }

    [SerializeField]
    private float m_turnSpeed = 90.0f;

    [SerializeField]
    private bool m_edgeControlOverride;
    public bool EdgeControlOverride
    {
        get { return m_edgeControlOverride; }
        set { m_edgeControlOverride = value; }
    }

    [SerializeField, Range(0, 120)]
    private float m_turnOverrideMaxAngle = 90.0f;

    [Header("Skating")]

    [SerializeField, Range(0, 1)]
    private float m_skate;
    public float Skate
    {
        get { return m_skate; }
        set { m_skate = Mathf.Clamp(value, 0, 1); }
    }

    [SerializeField]
    private float m_skateForce = 1.0f;

    [SerializeField]
    private float m_skateSlopeLimit = 10.0f;

    [Header("Friction")]

    [SerializeField]
    private float m_staticFrictionCutoffSpeed = 0.01f;
    public float StaticFrictionCutoffSpeed => m_staticFrictionCutoffSpeed;

    [SerializeField]
    private float m_parallelFriction = 0.1f;

    [SerializeField]
    private Vector2 m_perpendicularFrictionRange = new(0.6f, 1.2f);

    [Header("Gravity")]

    [SerializeField]
    private Vector3 m_accelerationGravity = Vector3.down * 9.8f;


    //[SerializeField]
    //private float m_momentumBoostFactor = 0.75f;

    private CharacterController m_controller;

    private Vector3 m_vel;
    public Vector3 Velocity => m_vel;

    private Vector3 m_acc;
    public Vector3 Acceleration => m_acc;

    //private Quaternion m_angularAccStep;
    //public Quaternion AngularAccelerationStep => m_angularAccStep;

    private bool m_isGrounded;
    public bool IsGrounded => m_isGrounded;

    private bool m_isSkatable;
    public bool IsSkatable => m_isSkatable;

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
        if (m_isGrounded = GroundedCheck(out m_groundNormal, out _))
        {
            m_lastGroundedGroundNormal = m_groundNormal;

            Vector3 gravitationalForce = Vector3.ProjectOnPlane(m_accelerationGravity, m_groundNormal);
            Vector3 skatingForce = SkatingForce(out m_isSkatable);
            Vector3 appliedForce = gravitationalForce + skatingForce;

            Vector3 frictionForce = FrictionForce(appliedForce, out _, out _);
            //Vector3 momentumBoostForce = MomentumBoostForce(appliedGravitationalForce, perpendicularFricForce);

            m_acc = appliedForce + frictionForce;
        }
        else
        {
            m_isSkatable = false;
            m_acc = m_accelerationGravity;
        }

        //Vector3 lastVel = m_vel;
        m_vel += m_acc * Time.deltaTime;
        m_controller.Move(m_vel * Time.deltaTime);
        //m_angularAccStep = Quaternion.FromToRotation(lastVel, m_vel);
        
        if (m_isGrounded && !m_isSkatable && m_edgeControlOverride)
        {
            float t = m_turn / 2 + 0.5f;
            Vector3 groundForward = Vector3.ProjectOnPlane(m_groundNormal, Vector3.up);

            float baseAngle = Vector3.SignedAngle(Vector3.forward, groundForward, Vector3.up);
            float minAngle = baseAngle - m_turnOverrideMaxAngle;
            float maxAngle = baseAngle + m_turnOverrideMaxAngle;

            float angle = Mathf.Lerp(minAngle, maxAngle, t);

            transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
        }
        else
        {
            float angleStep = m_turn * m_turnSpeed * Time.deltaTime;
            transform.rotation = transform.rotation * Quaternion.AngleAxis(angleStep, Vector3.up);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        m_vel = Vector3.ProjectOnPlane(m_vel, hit.normal);
    }

    private Vector3 SkatingForce(out bool isSkatable)
    {
        if (isSkatable = SkatableCheck())
        {
            Vector3 direction = Vector3.ProjectOnPlane(transform.forward, m_groundNormal).normalized;
            return m_skate * m_skateForce * direction;
        }

        return Vector3.zero;
    }

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
            float parallelFricForceMag = Mathf.Min(m_parallelFriction * normalForceMag, parallelAppliedForce.magnitude);
            return parallelFricForceMag * -parallelAppliedForce.normalized;
        }
        else
        {
            float parallelFricForceMag = m_parallelFriction * normalForceMag;
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

    private float GetPerpendicularFriction()
    {
        float t = m_edgeControlOverride ? 1.0f : m_edgeControl;
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

    private bool SkatableCheck() => Vector3.Angle(m_groundNormal, Vector3.up) <= m_skateSlopeLimit;

    public void ZeroVelocityAndAcceleration()
    {
        m_vel = Vector3.zero;
        m_acc = Vector3.zero;
    }
}
