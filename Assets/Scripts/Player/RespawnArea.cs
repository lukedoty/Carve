using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class RespawnArea : MonoBehaviour
{
    [SerializeField]
    private Vector3 m_respawnPoint;
    [SerializeField]
    private float m_respawnDirection;

    private BoxCollider m_boxCollider;

    private void Awake()
    {
        m_boxCollider = GetComponent<BoxCollider>();
        m_boxCollider.isTrigger = true;
    }

    // ISSUE: Triggering, but not working.
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<SkiController>(out SkiController controller))
        {
            Debug.Log("The respawn area is currently not working. You'll need to stop and restart to ski more, sorry!");
            controller.ZeroVelocityAndAcceleration();
            controller.transform.SetPositionAndRotation(m_respawnPoint, Quaternion.AngleAxis(m_respawnDirection, Vector3.up));
        }
    }
}
