using System;
using System.Collections.Generic;
<<<<<<< Updated upstream
using Microsoft.Unity.VisualStudio.Editor;
=======
using System.Linq;
>>>>>>> Stashed changes
using UnityEngine;
using UnityEditor;
using Mono.Cecil;
using System;
using System.Data.Common;

[RequireComponent(typeof(Collider))]
public class OverworldSticker : MonoBehaviour
{
    private string m_stickerID;

    public void Start()
    {
        if (GameManager.Sticker.HasSticker(m_stickerID))
        {
            Destroy(gameObject);
        }
        SpriteRenderer stickerRenderer = GetComponentInChildren<SpriteRenderer>();

        if (stickerRenderer == null)
        {
            Debug.LogError($"Overworld sticker of ID \"{m_stickerID}\" has no child sprite.");
            return;
        }

        Sprite stickerSprite = GameManager.Sticker.Registry[m_stickerID].StickerImage;
        stickerRenderer.sprite = stickerSprite;
    }

    public void setID(string ID)
    {
        m_stickerID = ID;
    }

    public void OnValidate()
    {
        StickerManager stickerManager = FindFirstObjectByType<StickerManager>();
        if (stickerManager.Registry.ContainsKey(m_stickerID)) return;

        Debug.LogError($"Registry does not contain sticker \"{m_stickerID}\".");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Sticker.ObtainSticker(m_stickerID);
            Destroy(gameObject);
        }
    }
}

[CustomEditor(typeof(OverworldSticker))]
public class OverworldStickerEditor : Editor
{

    
    string[] IDs = {};
    int i = 0;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        StickerManager stickerManager = FindFirstObjectByType<StickerManager>();
        List<Sticker> stickerList = stickerManager.RegistryList;

        int j = 0;
        foreach (Sticker sticker in stickerList)
        {
            if (sticker != null)
            {
                IDs[j] = sticker.ID;
                j++;
            }
        }
        OverworldSticker overworldSticker = target as OverworldSticker;
        if (IDs.Length > 0)
        {
            i = EditorGUILayout.Popup(i, IDs);
            overworldSticker.setID(IDs[i]);
            EditorUtility.SetDirty(target);
        } 
        else
        {
            overworldSticker.setID("LittleSteppe");
            EditorGUILayout.LabelField("ERROR: Sticker List not found or no stickers contained in the registry.");
        }
    }
}