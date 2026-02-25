using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sticker Asset", menuName = "Scriptable Objects/Sticker Asset")]
public class Sticker : ScriptableObject, IRegisterable
{
    [SerializeField]
    private string m_stickerID;
    public string ID => m_stickerID;

    [SerializeField]
    private Texture2D m_stickerImage;
    public Texture2D StickerImage => m_stickerImage;
}
