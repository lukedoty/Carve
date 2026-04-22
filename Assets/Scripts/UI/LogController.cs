using UnityEngine;
using UnityEngine.InputSystem;

public class LogController : MonoBehaviour
{
    [SerializeField]
    private GameObject m_questLog;
    private void Update()
    {
        if (GameManager.Input.UI.ToggleLog)
        {
            if (m_questLog.activeSelf)
            {
                m_questLog.SetActive(false);
            } else
            {
                m_questLog.SetActive(true);
            }    
        }

    }
}
