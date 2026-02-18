using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sticker Asset", menuName = "Scriptable Objects/Sticker Asset")]
public class Sticker : ScriptableObject
{
    [SerializeField]
    private string m_stickerID;
    public string StickerID => m_stickerID;
    [SerializeField]
    private Image m_stickerImage;
    public Image StickerImage => m_stickerImage;
}
