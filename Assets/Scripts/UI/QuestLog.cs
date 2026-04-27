using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestLog : MonoBehaviour
{
    [SerializeField]
    private GameObject m_entryPrefab;
    [SerializeField]
    private GameObject m_content;
    private Dictionary<string, GameObject> questObjs = new Dictionary<string, GameObject>();
    /**
    void Start()
    {
        GameManager.Quest.AssignQuestEvent.AddListener((id) => NewEntry(id));
        GameManager.Quest.CompleteQuestEvent.AddListener((id) => RemoveEntry(id));
    }

    private void NewEntry(string id)
    {
        GameObject entry = Instantiate(m_entryPrefab, m_content.transform);
        questObjs.Add(id, entry);
        entry.GetComponent<UIQuest>().UpdateText
                (GameManager.Quest.Registry[id].Name, GameManager.Quest.Registry[id].Description);
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_content.GetComponent<RectTransform>());
    }

    private void RemoveEntry(string id)
    {
        Debug.Log("removing entry!");
        Destroy(questObjs[id]);
        questObjs.Remove(id);
    }
    **/

    private void OnEnable()
    {
        foreach (QuestState state in GameManager.Quest.ActiveQuestStates)
        {
            GameObject entry = Instantiate(m_entryPrefab, m_content.transform);
                questObjs.Add(state.QuestID, entry);
                entry.GetComponent<UIQuest>().UpdateText
                    (GameManager.Quest.Registry[state.QuestID].Name, GameManager.Quest.Registry[state.QuestID].Description);
                LayoutRebuilder.ForceRebuildLayoutImmediate(m_content.GetComponent<RectTransform>());
        }
    }

    private void OnDisable()
    {
        foreach (GameObject entry in questObjs.Values)
        {
            Destroy(entry);
        }
        questObjs = new Dictionary<string, GameObject>();
    }
}
