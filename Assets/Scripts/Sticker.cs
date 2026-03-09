using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "New Sticker Asset", menuName = "Scriptable Objects/Sticker Asset")]
public class Sticker : ScriptableObject, IRegisterable
{
    [SerializeField]
    private string m_stickerID;
    public string ID => m_stickerID;

    [SerializeField]
    private string m_name;
    public string Name => m_name;

    [SerializeField]
    private Texture2D m_stickerImage;
    public Texture2D StickerImage => m_stickerImage;
}

#if UNITY_EDITOR
[CustomEditor(typeof(Sticker))]
public class StickerEditor : RegisterableEditor<Sticker, StickerManager> { }
#endif