using System.Collections.Generic;
using UnityEngine;

public class StickerManager : MonoBehaviour
{
    [SerializeField]
    private List<Sticker> m_stickers;
    private Dictionary<string, Sticker> m_stickerDict;
    public Dictionary<string, Sticker> Stickers => m_stickerDict;

    public List<string> ObtainedStickerIDs => GameManager.ActiveState.ObtainedStickerIDs;

    public void OnValidate()
    {
        foreach (Sticker s in m_stickers)
        {
            if (s == null) continue;
            if (m_stickers.FindAll(x => x != null && x.StickerID == s.StickerID).Count > 1)
            {
                Debug.LogError($"A sticker with the same ID \"{s.StickerID}\" has already been added to the StickerManager's Sticker list.");
            }
        }
    }

    private void Awake()
    {
        m_stickerDict = new Dictionary<string, Sticker>();

        foreach (Sticker s in m_stickers)
        {
            if (m_stickerDict.ContainsKey(s.StickerID)) continue;
            m_stickerDict.Add(s.StickerID, s);
        }
    }

    public bool AwardSticker(string id)
    {
        if (!m_stickerDict.ContainsKey(id)) return false;
        if (HasObtainedSticker(id)) return false;

        ObtainedStickerIDs.Add(id);
        return true;
    }

    public bool HasObtainedSticker(string id)
    {
        return ObtainedStickerIDs.Contains(id);
    }
}
