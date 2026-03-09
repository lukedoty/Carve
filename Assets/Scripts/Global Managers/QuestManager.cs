using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(GameManager))]
public class QuestManager : RegistryController<Quest>
{
    [SerializeField]
    private bool m_questEvaluationEnabled = false;
    public bool QuestEvaluationEnabled => m_questEvaluationEnabled;

    private UnityEvent<string> m_assignQuestEvent;
    public UnityEvent<string> AssignQuestEvent => m_assignQuestEvent;

    private UnityEvent<string> m_completeQuestEvent;
    public UnityEvent<string> CompleteQuestEvent => m_completeQuestEvent;

    public List<QuestState> ActiveQuestStates => GameManager.ActiveState.ActiveQuests;
    public List<QuestState> CompletedQuestStates => GameManager.ActiveState.CompletedQuests;

    private List<QuestState> m_completedQuestBuffer = new();

    protected override void Awake()
    {
        base.Awake();
        m_assignQuestEvent = new();
        m_completeQuestEvent = new();
    }

    private void Update()
    {
        if (m_questEvaluationEnabled)
        {
            EvaluateActiveQuests();
        }
    }

    private void EvaluateActiveQuests()
    {
        m_completedQuestBuffer.Clear();

        foreach (QuestState q in ActiveQuestStates)
        {
            if (EvaluateQuest(q)) m_completedQuestBuffer.Add(q);
        }

        foreach (QuestState q in m_completedQuestBuffer)
        {
            CompleteQuest(q);
        }
    }

    public bool EvaluateQuest(QuestState q)
    {
        if (!IsIdRegistered(q.QuestID)) return false;
            
        Criterion[] criteria = m_registry[q.QuestID].Criteria;

        bool questComplete = true;

        for (int i = 0; i < criteria.Length; i++)
        {
            if (q.PassedCriteria[i]) continue;

            if (criteria[i].Check()) q.PassedCriteria[i] = true;
            else questComplete = false;
        }

        return questComplete;
    }

    public bool IsQuestActive(string questID)
    {
        if (!IsIdRegistered(questID)) return false;
        return ActiveQuestStates.Exists(x => x.QuestID == questID);
    }

    public bool IsQuestComplete(string questID)
    {
        if (!IsIdRegistered(questID)) return false;
        return CompletedQuestStates.Exists(x => x.QuestID == questID);
    }

    public QuestState GetActiveQuestState(string questID)
    {
        if (!IsIdRegistered(questID)) return default;
        
        QuestState value = ActiveQuestStates.Find(x => x.QuestID == questID);
        if (value == default(QuestState)) Debug.LogError($"There is no active quest with ID \"{questID}\"");
        return value;
    }

    public QuestState GetCompletedQuestState(string questID)
    {
        if (!IsIdRegistered(questID)) return default;

        QuestState value = CompletedQuestStates.Find(x => x.QuestID == questID);
        if (value == default(QuestState)) Debug.LogError($"There is no completed quest with ID \"{questID}\"");
        return value;
    }

    public bool AssignQuest(string questID)
    {
        if (!IsIdRegistered(questID)) return false;

        ActiveQuestStates.Add(new QuestState(m_registry[questID]));
        m_assignQuestEvent.Invoke(questID);
        return true;
    }

    public bool AssignQuest(Quest q) => AssignQuest(q.ID);

    public bool CompleteQuest(string questID)
    {
        QuestState qs = GetActiveQuestState(questID);
        CompletedQuestStates.Add(qs);
        ActiveQuestStates.Remove(qs);

        ProcessRewards(m_registry[questID]);
        m_completeQuestEvent.Invoke(questID);
        return true;
    }

    public bool CompleteQuest(Quest q) => CompleteQuest(q.ID);

    public bool CompleteQuest(QuestState q) => CompleteQuest(q.QuestID);

    private void ProcessRewards(Quest quest)
    {
        foreach (QuestReward r in quest.Rewards)
        {
        switch (r.RewardType)
        {
            case QuestRewardType.Sticker:
                GameManager.Sticker.ObtainSticker(r.ID);
                break;
            case QuestRewardType.Quest:
                GameManager.Quest.AssignQuest(r.ID);
                break;
        }
        }
    }
}
