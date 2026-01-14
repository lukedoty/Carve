using UnityEngine;

[RequireComponent(typeof(SceneManager), typeof(StateManager), typeof(InputManager))]
public class GameManager : MonoBehaviour
{
    private static GameManager s_instance;

    private SceneManager m_sceneManager;
    public static SceneManager Scene => s_instance.m_sceneManager;

    private StateManager m_stateManager;
    public static StateManager State => s_instance.m_stateManager;

    private InputManager m_inputManager;
    public static InputManager Input => s_instance.m_inputManager;


    private void Awake()
    {
        if (s_instance != null && s_instance != this) Destroy(this);
        else s_instance = this;

        m_sceneManager = GetComponent<SceneManager>();
        m_stateManager = GetComponent<StateManager>();
        m_inputManager = GetComponent<InputManager>();
    }
}
