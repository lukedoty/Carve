using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField]
    private GameObject m_pauseMenu;
    [SerializeField]
    private GameObject m_controlsMenu;
    [SerializeField]
    private GameObject m_resumeBtn;
    [SerializeField]
    private GameObject m_controlsBtn;
    void Update()
    {
        if (GameManager.Input.UI.Cancel)
        {
            if (m_controlsMenu.activeSelf)
            {
                ToggleControls();
            } else
            {
                Resume();
            }
        }

        if (GameManager.Input.UI.Pause)
        {
            Resume();
        }
    }

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(m_resumeBtn);
        if (!m_pauseMenu.activeSelf)
        {
            ToggleControls();
        }
    }
    public void GoToScene(string sceneName)
    {
        GameManager.Scene.LoadSceneAndSwap(sceneName);
        Time.timeScale = 1;
    }

    public void ToggleControls()
    {
        if (m_pauseMenu.activeSelf)
        {
            m_pauseMenu.SetActive(false);
            m_controlsMenu.SetActive(true);
        } else
        {
            m_pauseMenu.SetActive(true);
            m_controlsMenu.SetActive(false);
            EventSystem.current.SetSelectedGameObject(m_controlsBtn);
        }
    }

    public void Resume()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }
}
