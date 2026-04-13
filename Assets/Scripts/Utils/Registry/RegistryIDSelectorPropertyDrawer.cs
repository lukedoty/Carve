#if UNITY_EDITOR
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(RegistryIDSelector<>), true)]
public class RegistryIDSelectorPropertyDrawer : PropertyDrawer
{
    private List<string> m_idList;

    public override VisualElement CreatePropertyGUI(SerializedProperty inProperty)
    {
        if (inProperty == null) return null;

        VisualElement container = new();

        SerializedProperty idProp = inProperty.FindPropertyRelative("m_id");

        System.Type[] registryType = inProperty.boxedValue.GetType().GetGenericArguments();
        MethodInfo populateIDListMethod = this.GetType().GetMethod(nameof(PopulateIDList), BindingFlags.Instance | BindingFlags.NonPublic);
        populateIDListMethod = populateIDListMethod.MakeGenericMethod(registryType);
        populateIDListMethod.Invoke(this, null);

        if (m_idList.Count == 0)
        {
            container.Add(new Label($"{inProperty.displayName} - The {registryType[0].Name} registry is empty."));
            return container;
        }

        string defaultOption = m_idList.Contains(idProp.stringValue) ? idProp.stringValue : m_idList[0];
        DropdownField idField = new(inProperty.displayName, m_idList, defaultOption);
        idField.RegisterValueChangedCallback(e => {
            idProp.stringValue = e.newValue;
            idProp.serializedObject.ApplyModifiedProperties();
        });

        container.Add(idField);

        return container;
    }

    private void PopulateIDList<T>()
        where T : IRegisterable
    {
        m_idList ??= new List<string>();
        m_idList.Clear();

        RegistryController<T> controller = Resources.Load<RegistryController<T>>("Game Manager");

        foreach (T registerable in controller.RegistryList)
        {
            m_idList.Add(registerable.ID);
        }
    }
}
#endif