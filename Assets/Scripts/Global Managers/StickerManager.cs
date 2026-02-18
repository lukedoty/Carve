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

    public bool AwardSticker(string id)
    {
        if (!m_stickerRegistryDict.ContainsKey(id)) return false;
        if (HasObtainedSticker(id)) return false;

        ObtainedStickerIDs.Add(id);
        return true;
    }

    public bool HasObtainedSticker(string id)
    {
        return ObtainedStickerIDs.Contains(id);
    }
}
