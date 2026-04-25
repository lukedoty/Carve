using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class NotificationView : MonoBehaviour
{
    private const string k_showParam = "Show";

    [SerializeField]
    private TMP_Text m_header;

    [SerializeField]
    private TMP_Text m_body;

    [SerializeField]
    private Image m_icon;

    [SerializeField]
    private float m_hideDuration = 0.5f;

    private Animator m_animator;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    public void Show(string header, string body, Sprite icon)
    {
        m_header.text = header;
        m_body.text = body;

        if (icon != null)
        {
            m_icon.sprite = icon;
            m_icon.gameObject.SetActive(true);
        }
        else
        {
            m_icon.gameObject.SetActive(false);
        }

        m_animator.SetBool(k_showParam, true);
    }

    public void Hide()
    {
        m_animator.SetBool(k_showParam, false);
        Destroy(gameObject, m_hideDuration);
    }
}
