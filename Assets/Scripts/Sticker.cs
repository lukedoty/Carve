using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sticker Asset", menuName = "Scriptable Objects/Sticker Asset")]
public class Sticker : ScriptableObject
{
    [SerializeField]
    private string m_stickerId;
    public string StickerId => m_stickerId;
    [SerializeField]
    private Image m_stickerImage;
    public Image StickerImage => m_stickerImage;
    private bool m_unlocked = false;
    public bool Unlocked {
        get => m_unlocked; 
        set => m_unlocked = value;
    }
}
