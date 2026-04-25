using System.Collections;
using UnityEngine;

public class LiftInteract : Interactable
{
    [SerializeField]
    private string m_name;
    [SerializeField]
    private Vector3 m_tpLocation;
    private GameObject m_player;

    private void Awake()
    {
        m_prompt = $"Take lift {m_name}";
    }

    public override bool IsInteractable(PlayerInteract player)
    {
        return true;
    }

    public override void Interact()
    {
        if (m_player != null)
        {
            StartCoroutine(Teleport());
        }
    }

    public IEnumerator Teleport()
    {
        SkiController controller = m_player.GetComponent<SkiController>();
        controller.ZeroVelocityAndAcceleration();
        controller.Freeze();
        m_player.transform.position = m_tpLocation;

        yield return new WaitForSeconds(0.5f);
        
        controller.Unfreeze();
    }

    public void setTPLocation(Vector3 location)
    {
        m_tpLocation = location;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_player = other.gameObject;
        }
    }
}