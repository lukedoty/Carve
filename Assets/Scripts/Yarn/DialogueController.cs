using UnityEngine;

public class DialogueController : MonoBehaviour
{
    [SerializeField]
    private GameObject m_player;

    private SkiController m_playerSkiController;

    private static DialogueController m_instance;

    private void Awake()
    {
        if (m_player == null)
        {
            Debug.LogError("The Dialogue Controller is missing a reference to the Player GameObject.");
            return;
        }
        
        m_playerSkiController = m_player.GetComponent<SkiController>();
    }

    private void OnEnable()
    {
        m_instance = this;
    }

    private void OnDisable()
    {
        m_instance = null;
    }

    private void OnDestroy()
    {
        m_instance = null;
    }

    public void OnDialogueStart()
    {
        GameManager.Input.PlayerActions.Disable();
        GameManager.Input.Player.ZeroInputs();
        m_playerSkiController.Freeze();
    }

    public void OnDialogueComplete()
    {
        GameManager.Input.PlayerActions.Enable();
        m_playerSkiController.Unfreeze();
    }
}
