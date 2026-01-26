using MessagePack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(GameManager))]
public class StateManager : MonoBehaviour
{
    private const string k_extension = ".sdat";

    private string m_saveDirectory;
    private List<uint> m_saves = new();

    [SerializeField]
    private StateAsset m_stateOverride;

    [SerializeField]
    private State m_activeState;
    public State ActiveState => m_activeState;

    private void Awake()
    {
        if (m_stateOverride != null) m_activeState = m_stateOverride.State;

        m_saveDirectory = Application.persistentDataPath + "/saves/";
        Directory.CreateDirectory(m_saveDirectory);

        foreach (string s in Directory.GetFiles(m_saveDirectory))
        {
            if (!Path.GetExtension(s).Equals(k_extension)) continue;
            m_saves.Add(uint.Parse(Path.GetFileNameWithoutExtension(s)));
        }
    }

    public void NewState()
    {
        State state = new()
        {
            SaveID = AssignSaveID(),
            SaveCreated = DateTime.Now
        };

        m_activeState = state;
    }

    private uint AssignSaveID()
    {
        uint i = 0;
        while (m_saves.Contains(i)) i++;
        m_saves.Add(i);
        return i;
    }

    public Coroutine Save()
    {
        if (m_activeState == null) return null;
        m_activeState.LastSaved = DateTime.Now;
        return StartCoroutine(ISave());
    }

    private IEnumerator ISave()
    {
        string path = m_saveDirectory + m_activeState.SaveID + k_extension;
        using FileStream fs = File.Create(path);
        Task t = MessagePackSerializer.SerializeAsync(fs, m_activeState);
        yield return new WaitUntil(() => t.IsCompleted);
    }

    public Coroutine Load(uint saveID)
    {
        string path = m_saveDirectory + saveID + k_extension;
        if (!File.Exists(path)) return null;
        return StartCoroutine(ILoad(path));
    }

    private IEnumerator ILoad(string path)
    {
        using FileStream fs = File.OpenRead(path);
        ValueTask<State> t = MessagePackSerializer.DeserializeAsync<State>(fs);
        yield return new WaitUntil(() => t.IsCompleted);
        m_activeState = t.Result;
    }
}
