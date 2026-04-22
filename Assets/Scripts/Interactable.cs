using UnityEngine;

[RequireComponent (typeof(Collider))]
public abstract class Interactable : MonoBehaviour
{
    protected string m_prompt = "Interact";
    public string Prompt => m_prompt;
    public abstract bool IsInteractable(PlayerInteract player);
    public abstract void Interact();
}
