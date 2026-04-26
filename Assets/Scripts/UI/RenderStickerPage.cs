using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenderStickerPage : MonoBehaviour
{
    public bool leftPage;
    public Button advanceButton;
    public Button returnButton;
    private int pageNum;
    

    public void Start()
    {
        pageNum = leftPage ? 0 : 1;
        RenderPage();
    }

    public void FlipForward()
    {
        if (pageNum + 2 <= GameManager.Sticker.StickerBookPages.Count + 1)
        {
            pageNum += 2;
        }

        RenderPage();
    }

    public void FlipBack()
    {
        if (pageNum - 2 >= 0)
        {
            pageNum -= 2;
        }
        RenderPage();
    }

    public void RenderPage()
    {
        if (pageNum >= GameManager.Sticker.StickerBookPages.Count)
        {
            Image[] hiddenImages = GetComponentsInChildren<Image>();
            foreach (Image image in hiddenImages)
            {
                image.enabled = false;
            }
            return;
        }

        CheckButtons();
        int i = 0;

        Image[] images = GetComponentsInChildren<Image>();
        foreach (Image image in images)
        {

            if (i >= GameManager.Sticker.StickerBookPages[pageNum].stickers.Count)
            {
                image.enabled = false;
                continue;
            }

            image.enabled = true;
            Sticker sticker = GameManager.Sticker.StickerBookPages[pageNum].stickers[i];

            // Blacks out unobtained stickers
            image.color = GameManager.Sticker.HasSticker(sticker.ID) ? Color.white : Color.black;

            // Sets image to sticker sprite
            image.sprite = sticker.StickerImage;
            i++;
        }
    }

    public void CheckButtons()
    {
        if (leftPage && pageNum + 2 >= GameManager.Sticker.StickerBookPages.Count)
        {
            advanceButton.interactable = false;
        } else if (leftPage)
        {
            advanceButton.interactable = true;
        }

        if (leftPage && pageNum <= 1)
        {
            returnButton.interactable = false;
        } else if (leftPage)
        {
            returnButton.interactable = true;
        }
    }
}
