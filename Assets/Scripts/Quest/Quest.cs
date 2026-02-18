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
    private Criterion[] m_criteria;
    public Criterion[] Criteria => m_criteria;
}

[MessagePackObject, System.Serializable]
public class QuestState
{
    [Key(0)]
    public string QuestID;
    [Key(1)]
    public List<int> PassedCriteriaIndices;

    public QuestState() { }

    public QuestState(Quest q)
    {
        QuestID = q.QuestID;

        PassedCriteriaIndices = new();
        for (int i = 0; i < q.Criteria.Length; i++)
        {
            if (q.Criteria[i].Check()) PassedCriteriaIndices.Add(i);
        }
    }
}
