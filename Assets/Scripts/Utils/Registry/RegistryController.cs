using UnityEngine;
using System.Collections.Generic;

public abstract class RegistryController<T> : MonoBehaviour
    where T : IRegisterable
{
    [SerializeField]
    protected List<T> m_registryList;
#if UNITY_EDITOR
    public List<T> RegistryList => m_registryList;
#endif

    protected Dictionary<string, T> m_registry;
    public Dictionary<string, T> Registry => m_registry;

    protected virtual void OnValidate()
    {
        if (m_registryList == null) return;

        foreach (T item in m_registryList)
        {
            if (item == null) continue;
            if (m_registryList.FindAll(x => x != null && x.ID == item.ID).Count > 1)
                Debug.LogError($"An item with the same ID \"{item.ID}\" has already been added to this Registry.");
        }
    }

    protected virtual void Awake()
    {
        m_registry = new Dictionary<string, T>();

        foreach (T item in m_registryList)
        {
            if (m_registry.ContainsKey(item.ID)) continue;
            m_registry.Add(item.ID, item);
        }
    }

    public virtual bool IsIdRegistered(string id)
    {
        if (m_registry == null)
        {
            if (m_registryList.Find(x => x != null && x.ID == id) != null) return true;
        }
        else
        {
            if (m_registry.ContainsKey(id)) return true;
        }

        Debug.LogError($"This Registry does not contain an item with ID \"{id}\".");
        return false;
    }
}