using UnityEngine;
using TMPro;

public class InteractionPrompt : MonoBehaviour
{
    [SerializeField]
    private PlayerInteract m_playerInteract;
    [SerializeField]
    private TMP_Text m_textComponent;

    private void Awake()
    {
        m_textComponent.enabled = false;
    }

    private void Update()
    {
        if (GameManager.Input.PlayerActions.Enabled && m_playerInteract.HasTarget)
        {
            m_textComponent.text = m_playerInteract.Target.Prompt;
            m_textComponent.enabled = true;
        }
        else
        {
            m_textComponent.enabled = false;
        }
    }
}
