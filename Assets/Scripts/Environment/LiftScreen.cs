using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LiftScreen : MonoBehaviour
{
    [Header("Advertisement Screen")]
    [SerializeField]
    private RegistryIDSelector<Quest> goodScreenQuest;
    [SerializeField]
    private List<Sprite> evilAds;
    [SerializeField]
    private List<Sprite> goodAds;
    [SerializeField]
    private float imageSwapDelay;

    private Image screen;
    private bool updateImage = true;
    private int currImageIndex = -1;

    public void Start()
    {
        Image screenImage = GetComponentInChildren<Image>();
        if (screenImage != null)
        {
            screen = screenImage;
        } else
        {
            Debug.LogError($"Screen image not found.");
        }
    }

    public void Update()
    {
        if (screen != null && updateImage)
        {
            StartCoroutine(SwapImage());
        }
    }

    private IEnumerator SwapImage()
    {
        updateImage = false;
        screen.color = Color.white;

        List<Sprite> renderList = GameManager.Quest.IsQuestComplete(goodScreenQuest.ID) ? goodAds : evilAds;

        if (renderList.Count > 0)
        {
            currImageIndex++;
            if (currImageIndex >= renderList.Count)
            {
                currImageIndex = 0;
            }
            screen.sprite = renderList[currImageIndex];
        }

        yield return new WaitForSeconds(imageSwapDelay);
        updateImage = true;
    }
    
}