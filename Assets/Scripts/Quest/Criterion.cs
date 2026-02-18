using System;
using UnityEngine;

public abstract class Criterion : ScriptableObject
{
    [SerializeField]
    private string m_description;
    public string Description => m_description;

    public abstract bool Check();
}
