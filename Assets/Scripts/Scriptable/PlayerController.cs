using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform skisVisual;

    [Header("General")]
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float skiSpeed = 15f;
    [SerializeField] private float yawSpeedDeg = 90f;

    [Header("Skating (flat ground)")]
    [SerializeField] private float skateAccel = 10f;
    [SerializeField] private float skateSideDamp = 3f;

    [Header("Skiing (carving)")]
    [SerializeField] private float snowMu = 0.25f;        // friction coefficient
    [SerializeField] private float maxSkiYawDeg = 35f;    // steer angle applied to ski heading
    [SerializeField] private float maxEdgeDeg = 45f;      // visual tilt
    [SerializeField] private float sidecutRadius = 14f;   // meters; typical ~10-20m

    private InputManager inputManager;
    private Rigidbody rb;
    private CapsuleCollider capsule;
    private bool grounded;

    private void Awake()
    {
        inputManager = FindFirstObjectByType<InputManager>();
        rb = GetComponent<Rigidbody>();
        capsule = GetComponentInChildren<CapsuleCollider>();
    }

    private void FixedUpdate()
    {
        grounded = Grounded(out var groundNormal);

        if (!grounded)
            return;

        var input = inputManager.Player;

        ApplyLookYaw(input.Look.x);

        Vector3 vPlane = GetVelocityOnPlane(groundNormal);

        if (vPlane.magnitude < skiSpeed)
            MovementFlatGround(groundNormal);
        else
            MovementSkiing(groundNormal);
    }

    #region Flat Ground Logic

    private void MovementFlatGround(Vector3 n)
    {
        var input = inputManager.Player;

        Vector3 f = GetBodyForwardOnSlope(n);
        Vector3 r = Vector3.Cross(n, f).normalized;

        ApplyFlatForwardAccel(f, input.Move.y);
        ApplyFlatSideDamping(n, r);
    }

    private void ApplyFlatForwardAccel(Vector3 f, float moveY)
    {
        rb.AddForce(f * (moveY * skateAccel), ForceMode.Acceleration);
    }

    private void ApplyFlatSideDamping(Vector3 n, Vector3 r)
    {
        Vector3 vPlane = GetVelocityOnPlane(n);
        float sideSpeed = Vector3.Dot(vPlane, r);
        rb.AddForce(-r * (sideSpeed * skateSideDamp), ForceMode.Acceleration);
    }
    #endregion
    #region Skiing Logic
    private void MovementSkiing(Vector3 groundNormal)
    {
        var input = inputManager.Player;

        SlopeBasis basis = BuildSlopeBasis(groundNormal);

        float steer = Mathf.Clamp(input.Move.x, -1f, 1f);
        if (Mathf.Abs(steer) < 1e-3f)
            return;

        SkiFrame frame = BuildSkiFrame(basis, steer);
        EdgeInfo edge = ComputeEdgeInfo(basis, frame, steer);

        UpdateSkiVisuals(frame, basis.n, edge);

        Vector3 vPlane = GetVelocityOnPlane(basis.n);
        float speed = vPlane.magnitude;
        if (speed < 0.25f)
            return;

        // Gravity component along ground normal (baseline normal acceleration requirement)
        float gN = ComputeGravityNormalAccelMag(basis.n);

        // Turn radius from sidecut + edge
        float turnRadius = ComputeTurnRadius(edge.edgeMagDeg);

        // Uncapped centripetal carve demand
        float aDemand = (speed * speed) / turnRadius;
        aDemand *= edge.edge01; // no edge -> no carve

        // Compute maximum lateral accel from friction, accounting for increased normal load under lateral g
        float aMax = ComputeMaxLateralAccelFromMu(gN, snowMu);
        float aLat = Mathf.Min(aDemand, aMax);

        // Compute required normal acceleration magnitude
        float nReq = ComputeRequiredNormalAccel(gN, aLat);
        float normalForceN = rb.mass * nReq;

        // Available friction acceleration given this normal load: mu * N / m = mu * nReq
        float aFricMax = snowMu * nReq;

        // Apply carve + frictional damping
        ApplyCarveAcceleration(frame, steer, aLat);
        ApplySkiFriction(frame, vPlane, edge.edge01, aFricMax);

        // Rotate the character to follow the carve
        ApplyCarveRotation(speed, turnRadius, edge.edge01, steer);

        // can use normalForceN for ski spray or other stuff
        _ = normalForceN;
    }
    #endregion

    #region G-Force Calculations
    private static float ComputeGravityNormalAccelMag(Vector3 n)
    {
        return Mathf.Abs(Vector3.Dot(Physics.gravity, n));
    }

    private static float ComputeRequiredNormalAccel(float gN, float aLat)
    {
        // normal accel magnitude needed to sustain gravity-normal + lateral turn accel
        return Mathf.Sqrt(gN * gN + aLat * aLat); // m/s^2
    }

    private static float ComputeMaxLateralAccelFromMu(float gN, float mu)
    {
        // Solve aLat <= mu * sqrt(gN^2 + aLat^2) for aLat (mu < 1)
        mu = Mathf.Clamp(mu, 0f, 0.999f);
        float denom = Mathf.Sqrt(Mathf.Max(1f - mu * mu, 1e-6f));
        return mu * gN / denom;
    }
    #endregion


    #region Math/Helpers
    private Vector3 GetVelocity()
    {
        return rb.linearVelocity;
        // return rb.velocity;
    }

    private Vector3 GetVelocityOnPlane(Vector3 n)
    {
        if (n.sqrMagnitude < 1e-8f) return GetVelocity();
        return Vector3.ProjectOnPlane(GetVelocity(), n);
    }

    private Vector3 GetBodyForwardOnSlope(Vector3 n)
    {
        Vector3 fBody = Vector3.ProjectOnPlane(transform.forward, n);
        if (fBody.sqrMagnitude < 1e-6f)
            fBody = Vector3.ProjectOnPlane(Vector3.forward, n);
        return fBody.normalized;
    }

    private readonly struct SlopeBasis
    {
        public readonly Vector3 n;
        public readonly Vector3 fBody;
        public readonly Vector3 upslope;
        public readonly float slopeAngleDeg;

        public SlopeBasis(Vector3 n, Vector3 fBody, Vector3 upslope, float slopeAngleDeg)
        {
            this.n = n;
            this.fBody = fBody;
            this.upslope = upslope;
            this.slopeAngleDeg = slopeAngleDeg;
        }
    }

    private readonly struct SkiFrame
    {
        public readonly Vector3 fSki;
        public readonly Vector3 rSki;

        public SkiFrame(Vector3 fSki, Vector3 rSki)
        {
            this.fSki = fSki;
            this.rSki = rSki;
        }
    }

    private readonly struct EdgeInfo
    {
        public readonly float edge01;
        public readonly float edgeMagDeg;
        public readonly float edgeDegSigned;

        public EdgeInfo(float edge01, float edgeMagDeg, float edgeDegSigned)
        {
            this.edge01 = edge01;
            this.edgeMagDeg = edgeMagDeg;
            this.edgeDegSigned = edgeDegSigned;
        }
    }

    private SlopeBasis BuildSlopeBasis(Vector3 groundNormal)
    {
        Vector3 n = groundNormal.normalized;

        Vector3 fBody = GetBodyForwardOnSlope(n);

        Vector3 upslope = Vector3.ProjectOnPlane(-Physics.gravity, n);
        if (upslope.sqrMagnitude < 1e-6f)
            upslope = Vector3.ProjectOnPlane(Vector3.forward, n);
        upslope.Normalize();

        float slopeAngleDeg = Vector3.Angle(n, Vector3.up);

        return new SlopeBasis(n, fBody, upslope, slopeAngleDeg);
    }

    private SkiFrame BuildSkiFrame(SlopeBasis basis, float steer)
    {
        float skiYawDeg = steer * maxSkiYawDeg;
        Vector3 fSki = (Quaternion.AngleAxis(skiYawDeg, basis.n) * basis.fBody).normalized;
        Vector3 rSki = Vector3.Cross(basis.n, fSki).normalized;
        return new SkiFrame(fSki, rSki);
    }

    private EdgeInfo ComputeEdgeInfo(SlopeBasis basis, SkiFrame frame, float steer)
    {
        float edge01 = Mathf.Abs(steer);

        float misalign01 = 1f - Mathf.Abs(Vector3.Dot(frame.fSki, basis.upslope));
        float edgeMagDeg = edge01 * maxEdgeDeg + misalign01 * basis.slopeAngleDeg;

        float edgeDegSigned = edgeMagDeg * -Mathf.Sign(steer);

        return new EdgeInfo(edge01, edgeMagDeg, edgeDegSigned);
    }

    private void UpdateSkiVisuals(SkiFrame frame, Vector3 n, EdgeInfo edge)
    {
        if (skisVisual == null) return;
        skisVisual.rotation = Quaternion.LookRotation(frame.fSki, n);
        skisVisual.Rotate(frame.fSki, edge.edgeDegSigned, Space.World);
    }

    private float ComputeTurnRadius(float edgeMagDeg)
    {
        float edgeRad = edgeMagDeg * Mathf.Deg2Rad;
        float cosE = Mathf.Clamp(Mathf.Cos(edgeRad), 0.15f, 1f);

        float turnRadius = sidecutRadius * cosE;
        return Mathf.Max(2.0f, turnRadius);
    }

    private void ApplyCarveAcceleration(SkiFrame frame, float steer, float aLat)
    {
        Vector3 a = frame.rSki * (aLat * Mathf.Sign(steer));
        rb.AddForce(a, ForceMode.Acceleration);
    }

    private void ApplySkiFriction(SkiFrame frame, Vector3 vPlane, float edge01, float aFricMax)
    {
        // Sideways damping (cap by current friction capability)
        float vSide = Vector3.Dot(vPlane, frame.rSki);
        float sideDamp = Mathf.Lerp(1.0f, 8.0f, edge01);
        float aSide = Mathf.Clamp(-vSide * sideDamp, -aFricMax, aFricMax);
        rb.AddForce(frame.rSki * aSide, ForceMode.Acceleration);

        // Small forward drag
        float vFwd = Vector3.Dot(vPlane, frame.fSki);
        float aFwd = -vFwd * 0.02f;
        rb.AddForce(frame.fSki * aFwd, ForceMode.Acceleration);
    }

    private void ApplyCarveRotation(float speed, float turnRadius, float edge01, float steer)
    {
        float omega = speed / turnRadius; // rad/s
        omega *= edge01;
        omega *= Mathf.Sign(steer);

        float omegaMax = 3.5f;
        omega = Mathf.Clamp(omega, -omegaMax, omegaMax);

        float yawDeltaDeg = omega * Mathf.Rad2Deg * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, yawDeltaDeg, 0f));
    }

    private void ApplyLookYaw(float lookX)
    {
        float yawDeltaDeg = lookX * yawSpeedDeg * Time.fixedDeltaTime;
        rb.MoveRotation(
            rb.rotation * Quaternion.Euler(0f, yawDeltaDeg, 0f)
        );
    }
    #endregion


    #region Grounded Checks
    private bool Grounded(out Vector3 groundNormal)
    {
        groundNormal = Vector3.up;
        if (capsule == null) return false;

        Transform t = capsule.transform;
        Vector3 s = t.lossyScale;
        float radiusWorld = capsule.radius * Mathf.Max(Mathf.Abs(s.x), Mathf.Abs(s.z));
        float heightWorld = capsule.height * Mathf.Abs(s.y);
        float halfHeightNoCaps = Mathf.Max(heightWorld * 0.5f - radiusWorld, 0f);

        Vector3 worldCenter = t.TransformPoint(capsule.center);
        Vector3 bottomSphereCenter = worldCenter - Vector3.up * halfHeightNoCaps;

        float skin = 0.02f;
        Vector3 origin = bottomSphereCenter + Vector3.up * skin;
        float castDist = radiusWorld + 0.25f;

        bool hit = Physics.SphereCast(
            origin,
            radiusWorld * 0.95f,
            -Vector3.up,
            out RaycastHit hitInfo,
            castDist,
            ~0,
            QueryTriggerInteraction.Ignore
        );

        groundNormal = hit ? hitInfo.normal : Vector3.up;
        return hit;
    }
    #endregion
}