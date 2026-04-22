using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

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

    private UnityAction<string> m_assignQuestListener;
    private UnityAction<string> m_completeQuestListener;
    private UnityAction<string> m_obtainStickerListener;

    private List<Notification> m_notifications;

    private void Awake()
    {
        m_assignQuestListener = (id) => InstantiateNotification(NotificationType.AssignQuest, id);
        m_completeQuestListener = (id) => InstantiateNotification(NotificationType.CompleteQuest, id);
        m_obtainStickerListener = (id) => InstantiateNotification(NotificationType.ObtainSticker, id);

        m_notifications = new();
    }

    private void Start()
    {
        GameManager.Quest.AssignQuestEvent.AddListener(m_assignQuestListener);
        GameManager.Quest.CompleteQuestEvent.AddListener(m_completeQuestListener);
        GameManager.Sticker.ObtainStickerEvent.AddListener(m_obtainStickerListener);
    }

    private void OnDestroy()
    {
        GameManager.Quest.AssignQuestEvent.RemoveListener(m_assignQuestListener);
        GameManager.Quest.CompleteQuestEvent.RemoveListener(m_completeQuestListener);
        GameManager.Sticker.ObtainStickerEvent.RemoveListener(m_obtainStickerListener);
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
                notification.Obj.GetComponent<NotificationView>().Hide();
            } else
            {
                m_notifications[i] = notification;
            }
        }
    }

    private void InstantiateNotification(NotificationType type, string id)
    {
        GameObject notification = Instantiate(m_notificationPrefab, transform);
        NotificationView view = notification.GetComponent<NotificationView>();

        string header = "";
        string body = "";
        Sprite icon = null;

        switch (type)
        {
            case NotificationType.ObtainSticker:
                Sticker sticker = GameManager.Sticker.Registry[id];
                header = m_obtainStickerHeader;
                body = sticker.Name;
                icon = sticker.StickerImage;
                break;
            case NotificationType.AssignQuest:
                header = m_assignQuestHeader;
                body = GameManager.Quest.Registry[id].Name;
                break;
            case NotificationType.CompleteQuest:
                Quest quest = GameManager.Quest.Registry[id];
                header = m_completeQuestHeader;
                body = quest.Name;
                icon = ResolveQuestRewardIcon(quest);
                break;
        }

        view.Show(header, body, icon);

        m_notifications.Insert(0, new Notification(notification, m_notificationLifetime));
    }

    private Sprite ResolveQuestRewardIcon(Quest quest)
    {
        foreach (QuestReward reward in quest.Rewards)
        {
            if (reward.RewardType == QuestRewardType.Sticker)
            {
                return GameManager.Sticker.Registry[reward.ID].StickerImage;
            }
        }
        return null;
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
