using UnityEngine;
using System.Reflection;
using Yarn.Unity;

public static class YarnStateFunctions
{
    [YarnFunction("get-state-string")]
    public static string GetStateString(string variableName) => GetStateVariable<string>(variableName);

    [YarnFunction("get-state-int")]
    public static int GetStateInt(string variableName) => GetStateVariable<int>(variableName);

    [YarnFunction("get-state-float")]
    public static float GetStateFloat(string variableName) => GetStateVariable<float>(variableName);

    [YarnFunction("get-state-bool")]
    public static bool GetStateBool(string variableName) => GetStateVariable<bool>(variableName);

    [YarnCommand("set-state-string")]
    public static void SetStateString(string variableName, string value) => SetStateVariable<string>(variableName, value);

    [YarnCommand("set-state-int")]
    public static void SetStateInt(string variableName, int value) => SetStateVariable<int>(variableName, value);

    [YarnCommand("set-state-float")]
    public static void SetStateFloat(string variableName, float value) => SetStateVariable<float>(variableName, value);

    [YarnCommand("set-state-bool")]
    public static void SetStateBool(string variableName, bool value) => SetStateVariable<bool>(variableName, value);

    private static T GetStateVariable<T>(string variableName)
    {
        FieldInfo fieldInfo = typeof(State).GetField(variableName);

        if (fieldInfo == null)
        {
            Debug.LogError($"Variable {variableName} does not exist within State.");
            return default;
        }

        if (typeof(T).IsAssignableFrom(fieldInfo.FieldType))
        {
            return (T)fieldInfo.GetValue(GameManager.ActiveState);
        }
        else
        {
            Debug.LogError($"Variable {variableName} exists within State, but is the wrong type (expected {typeof(T)}, got {fieldInfo.FieldType})");
            return default;
        }
    }

    private static void SetStateVariable<T>(string variableName, T value)
    {
        FieldInfo fieldInfo = typeof(State).GetField(variableName);
        if (fieldInfo == null) Debug.LogError($"Variable {variableName} does not exist within State.");

        if (fieldInfo.FieldType == typeof(T)) fieldInfo.SetValue(GameManager.ActiveState, value);
        else Debug.LogError($"Variable {variableName} exists, but is the wrong type (expected {typeof(T)}, got {fieldInfo.FieldType})");
    }
}