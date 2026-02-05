using MessagePack;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Quest Asset", menuName = "Scriptable Objects/Quest Asset")]
public class Quest : ScriptableObject
{
    [SerializeField]
    private string m_questID;
    public string QuestID => m_questID;

    [SerializeField]
    private string m_name;
    public string Name => m_name;

    [SerializeField]
    private string m_description;
    public string Description => m_description;

    [SerializeField]
    private List<Criterion> m_criteria;
    public List<Criterion> Criteria => m_criteria;
}

[MessagePackObject, System.Serializable]
public class QuestState
{
    [Key(0)]
    public string QuestID;
    [Key(1)]
    public SerializableDictionary<string, bool> CriteriaPassed;

    public QuestState() { }

    public QuestState(Quest q)
    {
        QuestID = q.QuestID;

        CriteriaPassed = new();
        foreach (Criterion c in q.Criteria) CriteriaPassed.Add(c.CriterionID, c.Check());
    }
}
