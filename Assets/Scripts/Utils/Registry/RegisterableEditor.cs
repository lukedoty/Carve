#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public abstract class RegisterableEditor<T, RC> : Editor
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