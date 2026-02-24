using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameManager))]
public class QuestManager : MonoBehaviour
{
    [SerializeField]
    private bool m_questEvaluationEnabled = false;
    public bool QuestEvaluationEnabled => m_questEvaluationEnabled;

    [SerializeField]
    private List<Quest> m_questRegistry;
    private Dictionary<string, Quest> m_questRegistryDict;
    public Dictionary<string, Quest> QuestRegistry => m_questRegistryDict;

    public List<QuestState> ActiveQuestStates => GameManager.ActiveState.ActiveQuests;
    public List<QuestState> CompletedQuestStates => GameManager.ActiveState.CompletedQuests;

    private List<QuestState> m_completedQuestBuffer = new();

    private void OnValidate()
    {
        foreach (Quest q in m_questRegistry)
        {
            if (q == null) continue;
            if (m_questRegistry.FindAll(x => x != null && x.QuestID == q.QuestID).Count <= 1) continue;

            Debug.LogError($"A quest with the same ID \"{q.QuestID}\" has already been added to the QuestManager's Quests list.");
            break;
        }
    }

    private void Awake()
    {
        m_questRegistryDict = new Dictionary<string, Quest>();

        foreach (Quest q in m_questRegistry)
        {
            if (m_questRegistryDict.ContainsKey(q.QuestID)) continue;
            m_questRegistryDict.Add(q.QuestID, q);
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
        if (!m_questRegistryDict.ContainsKey(q.QuestID)) return false;
        Criterion[] criteria = m_questRegistryDict[q.QuestID].Criteria;

        bool questComplete = true;

        for (int i = 0; i < criteria.Length; i++)
        {
            if (q.PassedCriteriaIndices.Contains(i)) continue;

            if (criteria[i].Check()) q.PassedCriteriaIndices.Add(i);
            else questComplete = false;
        }

        Debug.Log("Quest " + q.QuestID + " evaluates to " + questComplete);
        return questComplete;
    }

    public bool IsQuestActive(string questID) => ActiveQuestStates.Exists(x => x.QuestID == questID);

    public bool IsQuestComplete(string questID) => CompletedQuestStates.Exists(x => x.QuestID == questID);

    public QuestState GetActiveQuestState(string questID) => ActiveQuestStates.Find(x => x.QuestID == questID);

    public QuestState GetCompletedQuestState(string questID) => CompletedQuestStates.Find(x => x.QuestID == questID);

    public bool AssignQuest(string questID)
    {
        if (!QuestRegistry.ContainsKey(questID)) return false;

        ActiveQuestStates.Add(new QuestState(QuestRegistry[questID]));
        return true;
    }

    public bool AssignQuest(Quest q) => AssignQuest(q.QuestID);

    public bool CompleteQuest(string questID)
    {
        if (!IsQuestActive(questID)) return false;

        QuestState q = GetActiveQuestState(questID);
        CompletedQuestStates.Add(q);
        ActiveQuestStates.Remove(q);
        return true;
    }

    public bool CompleteQuest(Quest q) => CompleteQuest(q.QuestID);

    public bool CompleteQuest(QuestState q) => CompleteQuest(q.QuestID);
}
