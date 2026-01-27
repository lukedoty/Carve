using MessagePack;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Quest Asset", menuName = "Scriptable Objects/Quest Asset")]
public class Quest : ScriptableObject
{
    [SerializeField]
    private string m_questID;
    public string QuestID => m_questID;

    private List<string> m_criteria;
    public List<string> Criteria => m_criteria;
}

[MessagePackObject(keyAsPropertyName: true), System.Serializable]
public class QuestState
{
    public string QuestID;
    public List<string> Criteria;

    public QuestState() { }
    public QuestState(Quest q)
    {
        QuestID = q.QuestID;

        //TODO: Will need to be updated once criteria are no longer strings
        Criteria = new();
        foreach (string c in q.Criteria) Criteria.Add(new string(c));
    }
}
