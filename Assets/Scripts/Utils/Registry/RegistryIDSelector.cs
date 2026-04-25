using UnityEngine;

[System.Serializable]
public class RegistryIDSelector<T>
    where T : IRegisterable
{
    [SerializeField]
    private string m_id;
    public string ID => m_id;

    public RegistryIDSelector(string id)
    {
        m_id = id;
    }

    public static implicit operator string(RegistryIDSelector<T> r)
    {
        return r.m_id;
    }
}
