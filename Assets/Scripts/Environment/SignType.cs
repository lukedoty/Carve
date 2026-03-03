using UnityEngine;

[CreateAssetMenu(fileName = "SignType", menuName = "Scriptable Objects/SignType")]
public class SignType : ScriptableObject
{
    public string signType;
    public Color color;
    public Sprite icon;
}
