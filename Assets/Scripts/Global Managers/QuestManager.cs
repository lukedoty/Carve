using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(GameManager))]
public class QuestManager : MonoBehaviour
{
    [SerializeField]
    private List<Quest> m_quests = new();
    public List<Quest> Quests => m_quests;

    private void OnValidate()
    {
        foreach (Quest q in m_quests)
        {
            if (q == null) continue;
            if (m_quests.FindAll(x => x != null && x.QuestID == q.QuestID).Count > 1)
            {
                Debug.LogError($"A quest with the same ID \"{q.QuestID}\" has already been added to the QuestManager's Quests list.");
            }
        }
    }

    public Quest GetQuestFromID(string id) => m_quests.Find(q => q.QuestID == id);

    public QuestState GetActiveQuestStateFromID(string id) => GameManager.ActiveState.ActiveQuests.Find(q => q.QuestID == id);

    public bool AssignQuest(Quest q)
    {
        if (GameManager.ActiveState.ActiveQuests.Find(x => x.QuestID == q.QuestID) != null) return false;

        GameManager.ActiveState.ActiveQuests.Add(new QuestState(q));
        return true;
    }

    public bool AssignQuest(string id)
    {
        Quest q = GetQuestFromID(id);
        if (q == null) return false;
        return AssignQuest(q);
    }

    public bool CompleteQuest(string id)
    {
        QuestState q = GetActiveQuestStateFromID(id);
        if (q == null) return false;
        GameManager.ActiveState.ActiveQuests.Remove(q);
        GameManager.ActiveState.CompletedQuestIDs.Add(q.QuestID);
        return true;
    }

    public bool CompleteQuest(QuestState q) => CompleteQuest(q.QuestID);

    public bool CompleteQuest(Quest q) => CompleteQuest(q.QuestID);
}
