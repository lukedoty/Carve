using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerCursor : MonoBehaviour
{
    [SerializeField]
    private GameObject m_map;
    private InputAction m_navigate;
    private Vector3 m_mapPos;
    private float m_mapHeight;
    private float m_mapWidth;
    void Start()
    {
        m_navigate = InputSystem.actions.FindAction("Navigate");
        m_mapPos = m_map.transform.position;
        m_mapHeight = m_map.GetComponent<RectTransform>().rect.height;
        m_mapWidth = m_map.GetComponent<RectTransform>().rect.width;
    }
    void Update()
    {
        Vector2 currDistance = m_navigate.ReadValue<Vector2>();
        if (currDistance.x != 0 || currDistance.y != 0)
        {
            transform.Translate(m_navigate.ReadValue<Vector2>() * Time.deltaTime * 300);
        }
    }
}
