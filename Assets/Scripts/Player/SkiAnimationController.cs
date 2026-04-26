using UnityEngine;

[RequireComponent (typeof(SkiController))]
public class SkiAnimationController : MonoBehaviour
{
    [SerializeField]
    private Animator m_animator;
    private static readonly int s_turnHash = Animator.StringToHash("Turn");
    private static readonly int s_yHash = Animator.StringToHash("Y");

    [Header("Transforms")]
    [SerializeField]
    private Transform m_model;
    [SerializeField]
    private Transform m_skis;
    [SerializeField]
    private Transform m_leftSki;
    [SerializeField]
    private Transform m_rightSki;

    [Header("Settings")]
    [SerializeField]
    private float m_maxEdgeControlAngle = 45;
    //[SerializeField]
    //private float m_redirectionCeiling = 100;

    private SkiController m_controller;

    private void Awake()
    {
        m_controller = GetComponent<SkiController>();
    }

    private void Update()
    {
        m_animator.SetFloat(s_turnHash, m_controller.TurnInput);
        m_animator.SetFloat(s_yHash, m_controller.SkateInput);


        // SKIS-TO-GROUND ALIGNMENT

        Vector3 groundNormal = m_controller.LastGroundedGroundNormal;

        Vector3 skisForward = Vector3.ProjectOnPlane(m_model.forward, groundNormal).normalized;
        Quaternion targetAlignmentRotation = Quaternion.LookRotation(skisForward, groundNormal);
        m_skis.rotation = Quaternion.Lerp(m_skis.rotation, targetAlignmentRotation, 0.1f); // TODO: replace with robust animation and interpolation

        // EDGE ALIGNMENT

        //float redirectionStep;
        //if (m_controller.Velocity.sqrMagnitude > m_controller.StaticFrictionCutoffSpeed * m_controller.StaticFrictionCutoffSpeed)
        //{
        //    redirectionStep = Vector3.SignedAngle(Vector3.ProjectOnPlane(m_controller.AngularAccelerationStep * skisForward, groundNormal), skisForward, groundNormal);
        //}
        //else
        //{
        //    redirectionStep = 0;
        //}
        //float redirection = redirectionStep / Time.deltaTime;
        //float redirectionComponent = Mathf.Clamp(redirection / m_redirectionCeiling, -1, 1);

        Vector3 downhillOrthogonal = Vector3.Cross(groundNormal, Vector3.up).normalized;
        float downhillComponent = Vector3.Dot(skisForward, downhillOrthogonal);

        float edgeControlComponent = m_controller.IsPowerStopping ? 1.0f : m_controller.EdgeControlInput;

        float groundedComponent = m_controller.IsGrounded ? 1 : 0;

        float edgeControlAngle = downhillComponent * edgeControlComponent * groundedComponent * m_maxEdgeControlAngle;

        Quaternion edgeControlRotation = Quaternion.AngleAxis(edgeControlAngle, Vector3.forward);
        m_leftSki.localRotation = edgeControlRotation;
        m_rightSki.localRotation = edgeControlRotation;
    }
}
