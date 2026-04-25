using UnityEngine;

public class AssignQuest : MonoBehaviour
{
    [SerializeField]
    private string m_questID;

    public void AssignGivenQuest()
    {
        GameManager.Quest.AssignQuest(m_questID);
    }
}
