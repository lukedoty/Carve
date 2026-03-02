using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class NotificationController : MonoBehaviour
{
    [SerializeField]
    private GameObject m_notificationPrefab;

    [SerializeField]
    private Vector2 m_notificationRoot = new(-225, -75);
    [SerializeField]
    private float m_notificationSpacing;
    [SerializeField]
    private float m_notificationLifetime = 5f;

    [SerializeField]
    private string m_obtainStickerHeader;
    [SerializeField]
    private string m_assignQuestHeader;
    [SerializeField]
    private string m_completeQuestHeader;

    private List<Notification> m_notifications;

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
        for (int i = m_notifications.Count - 1; i >= 0; i--)
        {
            Notification notification = m_notifications[i];

            RectTransform rt = notification.Obj.GetComponent<RectTransform>();
            rt.anchoredPosition = m_notificationRoot + (i * m_notificationSpacing * Vector2.down);

            notification.Duration -= Time.deltaTime;

            if (notification.Duration <= 0)
            {
                m_notifications.RemoveAt(i);
                Destroy(notification.Obj);
            } else
            {
                m_notifications[i] = notification;
            }
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

        m_notifications.Insert(0, new Notification(notification, m_notificationLifetime));
    }

    private enum NotificationType
    {
        ObtainSticker,
        AssignQuest,
        CompleteQuest
    }

    private struct Notification
    {
        public Notification(GameObject obj, float duration)
        {
            Obj = obj;
            Duration = duration;
        }

        public GameObject Obj { get; set; }
        public float Duration { get; set; }
    }
}
