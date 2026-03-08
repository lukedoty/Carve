using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

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

    protected virtual bool IsIdRegistered(string id)
    {
        if (m_registry.ContainsKey(id)) return true;

        Debug.LogError($"This Registry does not contain an item with ID \"{id}\".");
        return false;
    }
}

public interface IRegisterable
{
    public string ID { get; }
}

#if UNITY_EDITOR
public abstract class RegisterableEditor<T, RC>:Editor
    where T : Object, IRegisterable
    where RC : RegistryController<T>
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Separator();

        string itemName = typeof(T).Name;

        T item = (T)target;

        RC registryController = Resources.Load<RC>("Game Manager");
        if (registryController == null) Debug.LogError("Registry not a part of the game manager.");

        T otherItem = registryController.RegistryList.Find(x => x != null && x.ID == item.ID);
        bool idIsRegistered = otherItem != null;
        bool isRegistered = idIsRegistered && item == otherItem;

        if (idIsRegistered)
        {
            if (isRegistered)
            {
                EditorGUILayout.LabelField($"This {itemName} is registered.");
                if (GUILayout.Button($"Unregister {itemName}")) Unregister(registryController, item.ID);
            }
            else
            {
                EditorGUILayout.LabelField($"This {itemName} is not registered.");
                EditorGUILayout.LabelField($"This ID is already in use by another {itemName}.");
                if (GUILayout.Button($"Replace Other {itemName}"))
                {
                    Unregister(registryController, otherItem.ID);
                    Register(registryController, item);
                }
                if (GUILayout.Button($"View Other {itemName}"))
                {
                    Selection.activeObject = otherItem;
                }
            }
        }
        else
        {
            EditorGUILayout.LabelField($"This {itemName} is not registered.");
            if (GUILayout.Button($"Register {itemName}")) Register(registryController, item);
        }
    }

    private void Register(RC registryController, T item)
    {
        registryController.RegistryList.Add(item);
        EditorUtility.SetDirty(registryController);
    }

    private void Unregister(RC registryController, string ID)
    {
        registryController.RegistryList.RemoveAll(x => x != null && x.ID == ID);
        EditorUtility.SetDirty(registryController);
    }
}
#endif
