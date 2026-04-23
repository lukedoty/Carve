using UnityEngine;
public class UIController : MonoBehaviour
{
    [SerializeField]
    private GameObject m_scrollContent;
    [SerializeField]
    private GameObject m_cursor;
    [SerializeField]
    private GameObject m_journal;
    public GameObject CurrentSlope { get; set; }
    void Update()
    {
        Vector2 currNavigation = GameManager.Input.UI.Navigate;
        if (currNavigation.x != 0 || currNavigation.y != 0)
        {
            m_cursor.transform.Translate(currNavigation * Time.deltaTime * 300);
        }

        float currScroll = GameManager.Input.UI.Scroll * -1;
        if (currScroll != 0)
        {
            m_scrollContent.transform.Translate(0, currScroll * Time.deltaTime * 1000, 0);
        }

        if (GameManager.Input.UI.Select)
        {
            if (CurrentSlope != null)
            {
                //do something with the current slope declared in SlopeMarker.cs
            }
        }

        if (GameManager.Input.UI.ToggleJournal)
        {
            m_journal.SetActive(false);
            GameManager.Input.PlayerActions.Enable();
        }
    }
}
