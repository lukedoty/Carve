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
    private Vector3 m_defaultScale;

    void Start()
    {
        m_defaultScale = gameObject.transform.localScale;
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x * 1.1f, gameObject.transform.localScale.y * 1.1f, gameObject.transform.localScale.z * 1.1f);
        m_journalName.GetComponent<TextMeshProUGUI>().text = m_name;
        m_journalIcon.GetComponent<Image>().sprite = m_slopeIcon;
        m_journalImage.GetComponent<Image>().sprite = m_slopeImage;
        //m_uiController.GetComponent<UIController>().CurrentSlope = something;
    }

    void OnTriggerExit2D()
    {
        gameObject.transform.localScale = m_defaultScale;
    }
}