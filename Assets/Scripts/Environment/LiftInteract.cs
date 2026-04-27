using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LiftInteract : Interactable
{
    [SerializeField]
    private string m_name;
    [SerializeField]
    private Vector3 m_tpLocation;
    [SerializeField]
    private Image m_blackPanel;
    [SerializeField]
    private float m_fadeTime = 2;
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
            SkiController controller = m_player.GetComponent<SkiController>();
            controller.Freeze();
            controller.ZeroVelocityAndAcceleration();
            StartCoroutine(Teleport());
        }
    }

    public IEnumerator Teleport()
    {
        yield return Fade();
        
        m_blackPanel.color = new Color(0, 0, 0, 0);
        yield return Unfade();
    }

    public void SetTPLocation(Vector3 location)
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

    private IEnumerator Fade()
    {
        yield return new WaitForEndOfFrame();
        float m_timer = 0;
        float m_percentFaded;
        
        while (m_timer < m_fadeTime)
        {
            m_timer += Time.deltaTime;
            m_percentFaded = m_timer/m_fadeTime;
            m_blackPanel.color = new Color(0, 0, 0, m_percentFaded);
            yield return null;
        }
        m_player.GetComponent<SkiController>().MovePlayer(m_tpLocation);
        Debug.Log($"target: {m_tpLocation}, reality: {m_player.transform.position}");
        yield return new WaitForSeconds(0.2f);
        m_player.GetComponent<SkiController>().Unfreeze();
        yield return new WaitForSeconds(0.8f);
    }

    private IEnumerator Unfade()
    {
        float m_timer = 0;
        float m_percentFaded;
        
        while (m_timer < m_fadeTime)
        {
            m_timer += Time.deltaTime;
            m_percentFaded = m_timer/m_fadeTime;
            m_blackPanel.color = new Color(0, 0, 0, 1 - m_percentFaded);
            yield return null;
        }
    }
}