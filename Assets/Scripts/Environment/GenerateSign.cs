using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GenerateSign : MonoBehaviour
{
    public GameObject signPanel;
    public GameObject signGraphic;
    public float signDisplacement;
    public float signTop;

    [Serializable]
    public struct SignInfo
    {
        public string text; // Slope/Feature name
        public SignType signType; // Determines color and icon
        [Range (0,359)]
        public int direction; // Rotation of the arrow in degrees
    }

    public List<SignInfo> signs;
  
    void Start()
    {
        int signNum = 0;
        foreach (SignInfo sign in signs)
        {
            GameObject panel = Instantiate(signPanel, this.transform);
            //panel.transform.localScale = panel.transform.localScale / this.transform.localScale.x;
            panel.transform.position = panel.transform.position + new UnityEngine.Vector3(0, signTop * transform.localScale.x - (signDisplacement * signNum * transform.localScale.x), 0.06f * transform.localScale.x) / 40;

            GameObject graphic = Instantiate(signGraphic, this.transform);
            //graphic.transform.localScale = graphic.transform.localScale / this.transform.localScale.x;
            graphic.transform.position = graphic.transform.position + new UnityEngine.Vector3(0, signTop * transform.localScale.x - (signDisplacement * signNum * transform.localScale.x ), 0.09f * transform.localScale.x) / 40;

            TextMeshProUGUI signText = graphic.GetComponentInChildren<TextMeshProUGUI>();

            signText.SetText(sign.text);

            Image[] images = graphic.GetComponentsInChildren<Image>();
            int i = 0;
            foreach (Image image in images)
            {
                if (i == 0)
                {
                    image.color = sign.signType.color;
                }
                if (i == 2)
                {
                    Vector3 angle = image.transform.eulerAngles;
                    image.transform.rotation = Quaternion.Euler(new(angle.x, angle.y, sign.direction));
                }
                if (i == 4)
                {
                    image.sprite = sign.signType.icon;
                    image.color = sign.signType.color;
                }
                i++;
            }

            signNum++;
        }
    }

}
