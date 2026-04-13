using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Mono.Cecil;
using System;
using System.Data.Common;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class OverworldSticker : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private string m_stickerID;
    private bool m_collected = false;

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
        Debug.Log(ID);
        m_stickerID = ID;
        Debug.Log(m_stickerID);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!m_collected)
            {
                StartCoroutine(Collect());
            }
        }
    }

    private IEnumerator Collect()
    {
        GameManager.Sticker.ObtainSticker(m_stickerID);
        Animator animator = GetComponentInChildren<Animator>();
        if (animator != null)
        {
            animator.SetBool("Collected", true);
            transform.rotation = Camera.main.transform.rotation;
        } else
        {
            Debug.LogError("Sticker animator not found");
            Destroy(gameObject);
        }
        yield return new WaitForSeconds(1);
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}



[CustomEditor(typeof(OverworldSticker))]
public class OverworldStickerEditor : Editor
{   
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RegistryController<Sticker> stickerRegistry = Resources.Load<RegistryController<Sticker>>("Game Manager");
        List<Sticker> stickerList = stickerRegistry.RegistryList;
        String[] IDs = new string[stickerList.Count];

        int j = 0;
        foreach (Sticker sticker in stickerList)
        {
            if (sticker != null)
            {
                IDs[j] = sticker.ID;
                j++;
            }
        }

        SerializedProperty overworldSticker = serializedObject.FindProperty("m_stickerID");

        if (IDs.Length > 0)
        {
            int i = EditorGUILayout.Popup(Array.IndexOf(IDs, overworldSticker.stringValue), IDs);
            overworldSticker.stringValue = IDs[i];
        } 
        else
        {
            overworldSticker.stringValue = "ERROR";
            EditorGUILayout.LabelField("ERROR: Sticker List not found or no stickers contained in the registry.");
        }

        serializedObject.ApplyModifiedProperties();
    }
}