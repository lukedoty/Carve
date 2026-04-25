using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using System.Linq;
#endif

[ExecuteAlways]
public class ZooManager : MonoBehaviour
{
#pragma warning disable 0414
    [SerializeField]
    private string m_path;
    [SerializeField]
    private float m_spacing = 1.0f;
#pragma warning restore 0414
}

#if UNITY_EDITOR
[CustomEditor(typeof(ZooManager))]
public class ZooManagerEditor : Editor
{
    private SerializedProperty m_path;
    private SerializedProperty m_spacing;

    private void OnEnable()
    {
        m_path = serializedObject.FindProperty("m_path");
        m_spacing = serializedObject.FindProperty("m_spacing");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ZooManager zooManager = (ZooManager)target;
        if (GUILayout.Button("Generate Zoo")) GenerateZoo(zooManager);
    }

    private void GenerateZoo(ZooManager zooManager)
    {
        while (zooManager.transform.childCount > 0)
        {
            DestroyImmediate(zooManager.transform.GetChild(0).gameObject);
        }

        string[] files = Directory.GetFiles(m_path.stringValue, "*.fbx", SearchOption.AllDirectories);

        Vector3 pos = Vector3.zero;
        for (int i = 0; i < files.Length; i++)
        {
            GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(files[i], typeof(GameObject));
            GameObject obj = Instantiate(prefab, zooManager.transform);

            MeshFilter[] meshFilters = obj.GetComponents<MeshFilter>().Concat(obj.GetComponentsInChildren<MeshFilter>()).ToArray();
            Bounds bounds = new(obj.transform.position, Vector3.zero);
            foreach (MeshFilter mf in meshFilters) bounds.Encapsulate(mf.sharedMesh.bounds);

            float halfWidth = bounds.extents.z / 2;
            pos += (m_spacing.floatValue + halfWidth) * Vector3.right;
            obj.transform.position = pos;
            pos += halfWidth * Vector3.right;

            EditorUtility.SetDirty(obj);
        }
    }
}
#endif
