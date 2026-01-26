using UnityEngine;
using MessagePack;
using System.Collections.Generic;

[MessagePackObject(keyAsPropertyName: true), System.Serializable]
public class State
{
    [Header("Metadata")]
    public uint SaveID;
    public UDateTime SaveCreated;
    public UDateTime LastSaved;
    public uint TimePlayedSeconds;

    //[Header("Settings")]

    //[Header("Quests")]
    //Active Quests
    //Completed Quests

    //[Header("Collectables")]
    //Stickers
    //Characters
    //Customizations

    [Header("Slopes")]
    public SlopeState SlopeA;
    public SlopeState SlopeB;

    //[Header("Lifts")]
    //Discovered lifts
}

[CreateAssetMenu(fileName = "New State Asset", menuName = "Scriptable Objects/State Asset")]
public class StateAsset : ScriptableObject
{
    [SerializeField]
    State m_state;
    public State State => m_state;
}
