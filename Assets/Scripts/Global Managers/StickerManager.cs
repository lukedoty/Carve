using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class StickerManager : RegistryController<Sticker>
{
    private UnityEvent<string> m_obtainStickerEvent;
    public UnityEvent<string> ObtainStickerEvent => m_obtainStickerEvent;
    public List<string> ObtainedStickerIDs => GameManager.ActiveState.ObtainedStickerIDs;
    
    [Serializable]
    public struct StickerPage
    {
        public string pageTitle;
        public List<Sticker> stickers;
    }

    [SerializeField]
    public List<StickerPage> StickerBookPages;

    protected override void Awake()
    {
        base.Awake();
        m_obtainStickerEvent = new();
    }

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
        m_obtainStickerEvent.Invoke(stickerID);
        return true;
    }
}
