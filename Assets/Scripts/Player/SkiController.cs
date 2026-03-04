using UnityEngine;

[RequireComponent (typeof(CharacterController))]
public class SkiController : MonoBehaviour
{
    private CharacterController m_controller;

    [SerializeField]
    private Vector3 m_accelerationGravity = Vector3.down * 9.8f;

    [SerializeField]
    private float m_staticFrictionCutoffSpeed = 0.01f;
    public float StaticFrictionCutoffSpeed => m_staticFrictionCutoffSpeed;

    [SerializeField]
    private float m_parallelFriction = 0.1f;

    [SerializeField]
    private Vector2 m_perpendicularFrictionRange = new(0.6f, 1.2f);

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
    private float m_momentumBoostFactor = 0.75f;

    private Vector3 m_vel;
    public Vector3 Velocity => m_vel;

    private Vector3 m_acc;
    public Vector3 Acceleration => m_acc;

    private Quaternion m_angularAccStep;
    public Quaternion AngularAccelerationStep => m_angularAccStep;

    private bool m_isGrounded;
    public bool IsGrounded => m_isGrounded;

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
        if (m_isGrounded = Grounded(out m_groundNormal, out _))
        {
            m_lastGroundedGroundNormal = m_groundNormal;

            // For both gravitational acceleration and friction, mass seems to fully cancel out.

            Vector3 appliedGravitationalForce = Vector3.ProjectOnPlane(m_accelerationGravity, m_groundNormal);
            Vector3 frictionForce = FrictionForce(m_groundNormal, appliedGravitationalForce, out _, out _);
            //Vector3 momentumBoostForce = MomentumBoostForce(appliedGravitationalForce, perpendicularFricForce);

            m_acc = appliedGravitationalForce + frictionForce;
            Debug.DrawRay(transform.position, appliedGravitationalForce, Color.blue);
            Debug.DrawRay(transform.position, frictionForce, Color.red);
            Debug.DrawRay(transform.position + Vector3.up, appliedGravitationalForce + frictionForce, Color.green);
        }
        else
        {
            m_acc = m_accelerationGravity;
        }

        Vector3 lastVel = m_vel;

        m_vel += m_acc * Time.deltaTime;
        m_controller.Move(m_vel * Time.deltaTime);

        m_angularAccStep = Quaternion.FromToRotation(lastVel, m_vel);

        Debug.DrawRay(transform.position + Vector3.up * 2, m_vel);


        float t = m_turn / 2 + 0.5f;
        float angle = Mathf.Lerp(0, 180, t);
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
    }

    private Vector3 FrictionForce(Vector3 groundNormal, Vector3 appliedForce, out Vector3 parallelFricForce, out Vector3 perpendicularFricForce)
    {
        Vector3 normalForce = appliedForce - m_accelerationGravity;
        float normalForceMag = normalForce.magnitude;

        // PARELLEL
        Vector3 parallelUnit = Vector3.ProjectOnPlane(transform.forward, groundNormal).normalized;
        Vector3 parallelVel = Vector3.Project(m_vel, parallelUnit);
        if (parallelVel.sqrMagnitude < m_staticFrictionCutoffSpeed * m_staticFrictionCutoffSpeed)
        {
            Vector3 parallelAppliedForce = Vector3.Project(appliedForce, parallelUnit);
            float parallelFricForceMag = Mathf.Min(m_parallelFriction * normalForceMag, parallelAppliedForce.magnitude);
            parallelFricForce = parallelFricForceMag * -parallelAppliedForce.normalized;
        }
        else
        {
            float parallelFricForceMag = m_parallelFriction * normalForceMag;
            parallelFricForce = parallelFricForceMag * -parallelVel.normalized;
        }

        //PERPENDICULAR
        Vector3 perpendicularUnit = Vector3.ProjectOnPlane(transform.right, groundNormal).normalized;
        Vector3 perpendicularVel = Vector3.Project(m_vel, perpendicularUnit);
        if (perpendicularVel.sqrMagnitude < m_staticFrictionCutoffSpeed * m_staticFrictionCutoffSpeed)
        {
            Vector3 perpendicularAppliedForce = Vector3.Project(appliedForce, perpendicularUnit);
            float perpendicularFricForceMag = Mathf.Min(GetPerpendicularFriction() * normalForceMag, perpendicularAppliedForce.magnitude);
            perpendicularFricForce = perpendicularFricForceMag * -perpendicularAppliedForce.normalized;
        }
        else
        {
            float perpendicularFricForceMag = GetPerpendicularFriction() * normalForceMag;
            perpendicularFricForce = perpendicularFricForceMag * -perpendicularVel.normalized;
        }


        return parallelFricForce + perpendicularFricForce;
    }

    private float GetPerpendicularFriction() => Mathf.Lerp(m_perpendicularFrictionRange.x, m_perpendicularFrictionRange.y, m_edgeControl);

    private bool Grounded(out Vector3 groundNormal, out RaycastHit groundHitInfo)
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

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        m_vel = Vector3.ProjectOnPlane(m_vel, hit.normal);
    }


}
