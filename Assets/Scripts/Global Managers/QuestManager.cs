using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameManager))]
public class QuestManager : MonoBehaviour
{
    [SerializeField]
    private bool m_questEvaluationEnabled = false;
    public bool QuestEvaluationEnabled => m_questEvaluationEnabled;

    [SerializeField]
    private List<Quest> m_quests;
    private Dictionary<string, Quest> m_questDict;
    public Dictionary<string, Quest> Quests => m_questDict;

    public SerializableDictionary<string, QuestState> ActiveQuestStates => GameManager.ActiveState.ActiveQuests;
    public SerializableDictionary<string, QuestState> CompletedQuestStates => GameManager.ActiveState.CompletedQuests;

    private void OnValidate()
    {
        foreach (Quest q in m_quests)
        {
            if (q == null) continue;
            if (m_quests.FindAll(x => x != null && x.QuestID == q.QuestID).Count <= 1) continue;

            Debug.LogError($"A quest with the same ID \"{q.QuestID}\" has already been added to the QuestManager's Quests list.");
            break;
        }
    }

    private void Awake()
    {
        m_questDict = new Dictionary<string, Quest>();

        foreach (Quest q in m_quests)
        {
            if (m_questDict.ContainsKey(q.QuestID)) continue;
            m_questDict.Add(q.QuestID, q);
        }
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
        foreach (QuestState q in ActiveQuestStates.Values)
        {
            if (EvaluateQuest(q)) CompleteQuest(q);
        }
    }

    public bool EvaluateQuest(QuestState q)
    {
        bool questComplete = true;
        foreach (Criterion c in m_questDict[q.QuestID].Criteria)
        {
            if (q.CriteriaPassed[c.CriterionID]) continue;

            if (c.Check()) q.CriteriaPassed[c.CriterionID] = true;
            else
            {
                questComplete = false;
                break;
            }
        }

        return questComplete;
    }

    public bool AssignQuest(string questID)
    {
        if (!Quests.ContainsKey(questID)) return false;
        if (ActiveQuestStates.ContainsKey(questID)) return false;

        ActiveQuestStates.Add(questID, new QuestState(Quests[questID]));
        return true;
    }

    public bool AssignQuest(Quest q) => AssignQuest(q.QuestID);

    public bool CompleteQuest(string questID)
    {
        if (!Quests.ContainsKey(questID)) return false;
        if (!ActiveQuestStates.ContainsKey(questID)) return false;

        CompletedQuestStates.Add(questID, ActiveQuestStates[questID]);
        ActiveQuestStates.Remove(questID);
        return true;
    }

    public bool CompleteQuest(Quest q) => CompleteQuest(q.QuestID);

    public bool CompleteQuest(QuestState q) => CompleteQuest(q.QuestID);
}
