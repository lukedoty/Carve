using MessagePack;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "New Quest Asset", menuName = "Scriptable Objects/Quest Asset")]
public class Quest : ScriptableObject, IRegisterable
{
    [SerializeField]
    private string m_questID;
    public string ID => m_questID;

    [SerializeField]
    private string m_name;
    public string Name => m_name;

    [SerializeField]
    private string m_description;
    public string Description => m_description;

    [SerializeField]
    private Criterion[] m_criteria;
    public Criterion[] Criteria => m_criteria;

    [SerializeField]
    private QuestReward[] m_rewards;
    public QuestReward[] Rewards => m_rewards;
}

[MessagePackObject, System.Serializable]
public class QuestState
{
    [Key(0)]
    public string QuestID;
    [Key(1)]
    public bool[] PassedCriteria;

    public QuestState() { }

    public QuestState(Quest q)
    {
        QuestID = q.ID;

        PassedCriteria = new bool[q.Criteria.Length];
        for (int i = 0; i < q.Criteria.Length; i++)
        {
            if (q.Criteria[i].Check()) PassedCriteria[i] = true;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Quest))]
public class QuestEditor : RegisterableEditor<Quest, QuestManager> { }
#endif