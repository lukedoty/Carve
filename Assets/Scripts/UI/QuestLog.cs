using UnityEngine;

public class QuestLog : MonoBehaviour
{
    [SerializeField]
    private GameObject m_entryPrefab;
    [SerializeField]
    private GameObject m_content;
    void Start()
    {
        GameManager.Quest.AssignQuestEvent.AddListener((id) => NewEntry(id));
    }

    private void NewEntry(string id)
    {
        GameObject entry = Instantiate(m_entryPrefab, m_content.transform);
        entry.GetComponent<UIQuest>().UpdateText
                (GameManager.Quest.Registry[id].Name, GameManager.Quest.Registry[id].Description);
    }
}
