using System.Collections.Generic;
using UnityEngine;

public class StickerManager : MonoBehaviour
{
    [SerializeField]
    private List<Sticker> m_stickers;
    private Dictionary<string, Sticker> m_stickersDict;
    public Dictionary<string, Sticker> Stickers => m_stickersDict;

    public void OnValidate()
    {
        foreach (Sticker s in m_stickers)
        {
            if (s == null) continue;
            if (m_stickers.FindAll(x => x != null && x.StickerId == s.StickerId).Count > 1)
            {
                Debug.LogError($"A sticker with the same ID \"{s.StickerId}\" has already been added to the StickerManager's Sticker list.");
            }
        }
    }
    
    public Sticker GetStickerById(string id) => m_stickers.Find(s => s.StickerId == id);
    public bool AwardSticker(string id)
    {
        List<string> stickerIds = GameManager.ActiveState.ObtainedStickerIDs;
        if (HasSticker(id))
        {
            return false;
        }
        stickerIds.Add(id);
        return true;
    }

    public bool HasSticker(string id)
    {
        return GameManager.ActiveState.ObtainedStickerIDs.Contains(id);
    }
}
