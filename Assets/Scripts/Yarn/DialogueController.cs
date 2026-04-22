using UnityEngine;
using Yarn.Unity;

[RequireComponent(typeof(DialogueRunner))]
public class DialogueController : MonoBehaviour
{
    [SerializeField]
    private GameObject m_player;

    [SerializeField]
    private float m_maxDialogueDistance = 5f;

    private SkiController m_playerSkiController;

    private DialogueRunner m_dialogueRunner;

    private Transform m_dialogueSpeaker;

    private void Awake()
    {
        m_playerSkiController = m_player.GetComponent<SkiController>();
        m_dialogueRunner = GetComponent<DialogueRunner>();
    }

    private void Update()
    {
        if (m_dialogueSpeaker == null || !m_dialogueRunner.IsDialogueRunning) return;

        float distance = Vector3.Distance(m_player.transform.position, m_dialogueSpeaker.position);
        if (distance > m_maxDialogueDistance)
        {
            m_dialogueRunner.Stop().Forget();
        }
    }

    public void OnDialogueStart()
    {
        GameManager.Input.PlayerActions.Disable();
        GameManager.Input.Player.ZeroInputs();
        //m_playerSkiController.Freeze();
        m_playerSkiController.ZeroVelocityAndAcceleration();
    }

    public void OnDialogueComplete()
    {
        GameManager.Input.PlayerActions.Enable();
        //m_playerSkiController.Unfreeze();
        m_dialogueSpeaker = null;
    }

    public void StartDialogue(string nodeName, Transform speaker)
    {
        m_dialogueSpeaker = speaker;
        m_dialogueRunner.StartDialogue(nodeName);
    }
}
