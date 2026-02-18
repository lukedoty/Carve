using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class OverworldSticker : MonoBehaviour
{
    [SerializeField]
    public string stickerId;
    List<string> stickerIds;

    public void Start()
    {
        if (GameManager.Stickers.HasSticker(stickerId))
        {
            Destroy(this);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Stickers.AwardSticker(stickerId);
            Destroy(this);
        }
    }
}