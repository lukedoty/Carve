using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class OverworldSticker : MonoBehaviour
{
    [SerializeField]
    private string m_stickerID;

    public void Start()
    {
        if (GameManager.Sticker.HasSticker(m_stickerID))
        {
            Destroy(gameObject);
        }
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