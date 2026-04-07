using UnityEngine;
using UnityEngine.InputSystem;

public class LogController : MonoBehaviour
{
    [SerializeField]
    private GameObject m_questLog;
    private InputAction m_toggleLog;
    private void OnToggleLog(InputAction.CallbackContext context)
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
