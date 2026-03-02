using System;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GenerateSign : MonoBehaviour
{
    public GameObject signPanel;
    public float signDisplacement;
    public float signTop;
    [Serializable]
    public struct SignInfo
    {
        public string text;
        public SignType signType;
        [Range (0,359)]
        public int direction;
    }

    public List<SignInfo> signs;

    void OnDrawGizmosSelected()
    {
        // int signNum = 0;
        // foreach (SignInfo sign in signs)
        // {
        //     Gizmos.color = Color.green;
        //     Gizmos.DrawCube(new(transform.position.x + 0, transform.position.y + signTop - (signDisplacement * signNum), transform.position.z + 0.1f), UnityEngine.Vector3.one);
        //     signNum++;
        // }
    }
  
    void Start()
    {
        int signNum = 0;
        foreach (SignInfo sign in signs)
        {
            GameObject panel = Instantiate(signPanel, this.transform);
            panel.transform.localScale = panel.transform.localScale / this.transform.localScale.x;
            panel.transform.position = panel.transform.position + new UnityEngine.Vector3(0, signTop - (signDisplacement * signNum), 0.1f);
            signNum++;
        }
    }

}
