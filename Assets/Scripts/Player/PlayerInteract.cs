using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField]
    private float m_interactionRadius = 5;

    private Interactable m_target;
    public Interactable Target => m_target;
    public bool HasTarget => m_target != null;
    private float m_targetDistance;

    private void Update()
    {
        m_target = null;
        m_targetDistance = m_interactionRadius;

        Collider[] colliders = Physics.OverlapSphere(transform.position, m_interactionRadius, ~0, QueryTriggerInteraction.Collide);

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent<Interactable>(out Interactable interactable)
                && interactable.IsInteractable(this)
                && Vector3.Distance(transform.position, interactable.transform.position) <= m_targetDistance)
            {
                m_target = interactable;
                m_targetDistance = Vector3.Distance(transform.position, interactable.transform.position);
            }
        }
    }

    public void Interact()
    {
        if (m_target == null) return;

        m_target.Interact();
    }
}
