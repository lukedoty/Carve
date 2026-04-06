using UnityEngine;
using Yarn.Unity;

[RequireComponent(typeof(DialogueRunner))]
public class DialogueController : MonoBehaviour
{
    [SerializeField]
    private GameObject m_player;

    private SkiController m_playerSkiController;

    private DialogueRunner m_dialogueRunner;

    private void Awake()
    {
        if (m_player == null)
        {
            Debug.LogError("The Dialogue Controller is missing a reference to the Player GameObject.");
            return;
        }
        
        m_playerSkiController = m_player.GetComponent<SkiController>();
        m_dialogueRunner = GetComponent<DialogueRunner>();
    }

    public void OnDialogueStart()
    {
        GameManager.Input.PlayerActions.Disable();
        GameManager.Input.Player.ZeroInputs();
        m_playerSkiController.Freeze();
        m_playerSkiController.ZeroVelocityAndAcceleration();
    }

    public void OnDialogueComplete()
    {
        GameManager.Input.PlayerActions.Enable();
        m_playerSkiController.Unfreeze();
    }

    public void StartDialogue(string nodeName)
    {
        m_dialogueRunner.StartDialogue(nodeName);
    }
}
