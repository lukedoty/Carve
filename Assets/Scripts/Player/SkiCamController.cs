using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SkiCamController : MonoBehaviour
{
    [SerializeField]
    private SkiController m_controller;

    [SerializeField]
    private Vector3 m_lookOffset;

    private Camera m_cam;

    private Vector3 m_initialLocalPos;

    private float m_yAngle;

    private void Awake()
    {
        if (m_controller == null) m_controller = GetComponentInParent<SkiController>();

        m_cam = GetComponent<Camera>();
        m_initialLocalPos = transform.localPosition;
    }

    private void LateUpdate()
    {
        if (m_controller == null) return;

        Vector3 skiPos = m_controller.transform.position;

        if (m_controller.IsSkatable)
        {
            m_yAngle = m_controller.transform.rotation.eulerAngles.y;
            // TODO: When going fast over a skatable area, the camera should keep its position and not snap to player direction.
        }
        else
        {
            Vector3 groundForward = Vector3.ProjectOnPlane(m_controller.LastGroundedGroundNormal, Vector3.up);
            m_yAngle = Vector3.SignedAngle(Vector3.forward, groundForward, Vector3.up);
        }

        Vector3 m_boomOffset = m_initialLocalPos - m_lookOffset;
        Quaternion boomRotation = Quaternion.AngleAxis(m_yAngle, m_controller.LastGroundedGroundNormal);
        
        Vector3 targetPos = skiPos + m_lookOffset + boomRotation * m_boomOffset;
        transform.position = targetPos; // TODO: replace with robust animation and interpolation

        Quaternion rotation = Quaternion.LookRotation(skiPos + m_lookOffset - transform.position, Vector3.up);
        transform.rotation = rotation;
    }
}
