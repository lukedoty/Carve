using System.Collections.Generic;
using UnityEngine;

public class StickerManager : MonoBehaviour
{
    [SerializeField]
    private List<Sticker> m_stickerRegistry;
    private Dictionary<string, Sticker> m_stickerRegistryDict;
    public Dictionary<string, Sticker> StickerRegistry => m_stickerRegistryDict;

    public List<string> ObtainedStickerIDs => GameManager.ActiveState.ObtainedStickerIDs;

    public void OnValidate()
    {
        if (m_stickerRegistry == null) return;
        foreach (Sticker s in m_stickerRegistry)
        {
            if (s == null) continue;
            if (m_stickerRegistry.FindAll(x => x != null && x.StickerID == s.StickerID).Count > 1)
            {
                Debug.LogError($"A sticker with the same ID \"{s.StickerID}\" has already been added to the StickerManager's Sticker list.");
            }
        }
    }

    private void Awake()
    {
        m_stickerRegistryDict = new Dictionary<string, Sticker>();

        foreach (Sticker s in m_stickerRegistry)
        {
            if (m_stickerRegistryDict.ContainsKey(s.StickerID)) continue;
            m_stickerRegistryDict.Add(s.StickerID, s);
        }
    }

    public bool HasSticker(string stickerID)
    {
        return ObtainedStickerIDs.Contains(stickerID);
    }

    public bool AwardSticker(string stickerID)
    {
        if (!m_stickerRegistryDict.ContainsKey(stickerID)) return false;
        if (HasSticker(stickerID)) return false;

        ObtainedStickerIDs.Add(stickerID);
        return true;
    }
}
