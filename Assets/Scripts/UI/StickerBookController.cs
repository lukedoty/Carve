using System.Collections;
using UnityEngine;

public class StickerBookController : MonoBehaviour
{

    [SerializeField]
    private RenderStickerPage m_leftPage;
    [SerializeField]
    private RenderStickerPage m_rightPage;
    [SerializeField]
    private GameObject book;

    public void Start()
    {
        book.SetActive(false);
    }

    void Update()
    {
        if (GameManager.Input.UIActions.ToggleStickerBook.WasPressedThisFrame() && !book.activeSelf)
        {
            EnableBook();
        } else if (GameManager.Input.UIActions.ToggleStickerBook.WasPressedThisFrame() && book.activeSelf)
        {
            DisableBook();
        }

        if (GameManager.Input.UIActions.ToggleStickerBook.WasPressedThisFrame())
        {
            Debug.Log("Hello!");
        }
    }

    public void EnableBook()
    {
        Debug.Log("Enabling!");
        GameManager.Input.PlayerActions.Disable();
        if (m_leftPage)
        {
            m_leftPage.RenderPage();
        }
        if (m_rightPage)
        {
            m_rightPage.RenderPage();
        }
        book.SetActive(true);
    }

    public void DisableBook()
    {
        book.SetActive(false);
        GameManager.Input.PlayerActions.Enable();
    }

}
