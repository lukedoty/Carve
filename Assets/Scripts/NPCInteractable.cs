using UnityEngine;

public class NPCInteractable : Interactable
{
    [SerializeField]
    private string m_name;
    [SerializeField]
    private string m_nodeName;
    [SerializeField]
    private DialogueController m_dialogueController;

    private void Awake()
    {
        m_prompt = $"Talk to {m_name}";
    }

    public override bool IsInteractable(PlayerInteract player)
    {
        return true;
    }

    public override void Interact()
    {
        m_dialogueController.StartDialogue(m_nodeName);
    }
}
