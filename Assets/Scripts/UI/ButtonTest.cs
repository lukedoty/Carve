using UnityEngine;

public class ButtonTest : MonoBehaviour
{
    [SerializeField]
    private GameObject m_target;
    public void ChangeTargetActivity()
    {
        foreach (QuestState q in GameManager.Quest.ActiveQuestStates)
        {
            Debug.Log(GameManager.Quest.Registry[q.QuestID].Name);
        }

        if (m_target.activeSelf)
        {
            m_target.SetActive(false);
        } else
        {
            m_target.SetActive(true);
        }
    }
}
