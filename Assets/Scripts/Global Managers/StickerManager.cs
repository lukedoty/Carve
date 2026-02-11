using System.Collections.Generic;
using UnityEngine;

public class StickerManager : MonoBehaviour
{
    [SerializeField]
    private List<Sticker> stickers;
    public List<Sticker> Stickers => stickers;
}
