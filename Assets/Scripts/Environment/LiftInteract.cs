using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class LiftInteract : Interactable
{
    [SerializeField]
    private string m_name;
    [SerializeField]
    private Vector3 m_tpLocation;
    [SerializeField]
    private CinemachineCamera m_vcam;
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

        Vector3 delta = m_tpLocation - m_player.transform.position;
        m_player.transform.position = m_tpLocation;

        if (m_vcam != null)
            m_vcam.OnTargetObjectWarped(m_player.transform, delta);

        if (m_player.TryGetComponent<SkiCamController>(out var camCtrl))
            camCtrl.ResetSmoothing();

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