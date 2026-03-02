using UnityEngine;

public class CorrectRotation : MonoBehaviour
{
    private Quaternion m_baselineRotation;

    void Start()
    {
        m_baselineRotation = transform.rotation;
    }

    void Update()
    {
        float angle = transform.parent.rotation.eulerAngles.y;
        transform.rotation = m_baselineRotation * Quaternion.AngleAxis(angle, Vector3.up);
    }
}
