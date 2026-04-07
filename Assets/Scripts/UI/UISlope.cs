using UnityEngine;

public class UISlope : MonoBehaviour
{
    [SerializeField]
    private string m_name;
    public string Name => m_name;

    [SerializeField]
    private Sprite m_slopeIcon;
    public Sprite icon => m_slopeIcon;


    [SerializeField]
    private Sprite m_slopeImage;
    public Sprite image => m_slopeImage;
}