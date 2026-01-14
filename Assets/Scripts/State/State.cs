using UnityEngine;
using MessagePack;

[MessagePackObject(keyAsPropertyName: true), System.Serializable]
public class State
{
    [Header("Metadata")]

    public uint SaveID;
    public UDateTime SaveCreated;
    public UDateTime LastSaved;
    public uint TimePlayedSeconds;
}

[CreateAssetMenu(fileName = "New State Asset", menuName = "Scriptable Objects/State Asset")]
public class StateAsset : ScriptableObject
{
    [SerializeField]
    State m_state;
    public State State => m_state;
}
