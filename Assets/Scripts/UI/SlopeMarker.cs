using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SlopeMarker : MonoBehaviour
{
    [SerializeField]
    private string m_name;
    [SerializeField]
    private Sprite m_slopeIcon;
    [SerializeField]
    private Sprite m_slopeImage;
    [SerializeField]
    private GameObject m_journalName;
    [SerializeField]
    private Image m_journalIcon;
    [SerializeField]
    private Image m_journalImage;
    [SerializeField]
    private GameObject m_uiController;

    void OnTriggerEnter2D(Collider2D collider)
    {
        m_journalName.GetComponent<TextMeshProUGUI>().text = m_name;
        m_journalIcon.GetComponent<Image>().sprite = m_slopeIcon;
        m_journalImage.GetComponent<Image>().sprite = m_slopeImage;
        //m_uiController.GetComponent<UIController>().CurrentSlope = something;
    }
}