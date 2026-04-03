using UnityEngine;
using TMPro;

public class UIQuest : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_questName;
    [SerializeField]
    private TextMeshProUGUI m_questDesc; 

    public void UpdateText(string name, string desc)
    {
        m_questName.SetText(name);
        m_questDesc.SetText(desc);
    }
}
