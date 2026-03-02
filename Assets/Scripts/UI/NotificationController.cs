using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using TMPro;

public class NotificationController : MonoBehaviour
{
    [SerializeField]
    private string m_obtainStickerHeader;
    [SerializeField]
    private string m_assignQuestHeader;
    [SerializeField]
    private string m_completeQuestHeader;

    [SerializeField]
    private GameObject m_notificationPrefab;

    [SerializeField]
    private float m_notificationRoot = -75;
    [SerializeField]
    private float m_notificationSpacing;

    private List<GameObject> m_notifications;

    private enum NotificationType
    {
        ObtainSticker,
        AssignQuest,
        CompleteQuest
    }

    private void Awake()
    {
        m_notifications = new();
    }

    private void Start()
    {
        GameManager.Quest.AssignQuestEvent.AddListener((id) => InstantiateNotification(NotificationType.AssignQuest, id));
        GameManager.Quest.CompleteQuestEvent.AddListener((id) => InstantiateNotification(NotificationType.CompleteQuest, id));
        GameManager.Sticker.ObtainStickerEvent.AddListener((id) => InstantiateNotification(NotificationType.ObtainSticker, id));
    }

    private void Update()
    {
        for (int i = 0; i < m_notifications.Count; i++ )
        {
            GameObject notification = m_notifications[i];
            RectTransform rt = notification.GetComponent<RectTransform>();
            //rt.anchoredPosition.y = m_notificationRoot - m_notificationSpacing * i;
        }
    }

    private void InstantiateNotification(NotificationType type, string id)
    {
        GameObject notification = Instantiate(m_notificationPrefab, transform);

        TMP_Text[] text = notification.GetComponentsInChildren<TMP_Text>();
        
        switch (type)
        {
            case NotificationType.ObtainSticker:
                text[0].text = m_obtainStickerHeader;
                text[1].text = GameManager.Sticker.Registry[id].Name;
                break;
            case NotificationType.AssignQuest:
                text[0].text = m_assignQuestHeader;
                text[1].text = GameManager.Quest.Registry[id].Name;
                break;
            case NotificationType.CompleteQuest:
                text[0].text = m_completeQuestHeader;
                text[1].text = GameManager.Quest.Registry[id].Name;
                break;
        }

        m_notifications.Insert(0, notification);
    }


}
