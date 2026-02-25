using System.Collections.Generic;
using UnityEngine;

public class StickerManager : RegistryController<Sticker>
{
    public List<string> ObtainedStickerIDs => GameManager.ActiveState.ObtainedStickerIDs;

    public bool HasSticker(string stickerID)
    {
        if (!IsIdRegistered(stickerID)) return false;
        return ObtainedStickerIDs.Contains(stickerID);
    }

    public bool ObtainSticker(string stickerID)
    {
        if (!IsIdRegistered(stickerID)) return false;
        if (HasSticker(stickerID))
        {
            Debug.LogError($"The sticker with ID \"{stickerID}\" has already been obtained.");
            return false;
        }

        ObtainedStickerIDs.Add(stickerID);
        return true;
    }
}
