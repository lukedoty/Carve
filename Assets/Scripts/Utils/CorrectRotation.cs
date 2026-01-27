using UnityEngine;

public class CorrectRotation : MonoBehaviour
{
    private Quaternion m_baselineRotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_baselineRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        float angle = transform.parent.rotation.eulerAngles.y;
        transform.rotation = m_baselineRotation * Quaternion.AngleAxis(angle, Vector3.up);
    }
}
