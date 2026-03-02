using System;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GenerateSign : MonoBehaviour
{
    public GameObject signPanel;
    public GameObject signGraphic;
    public float signDisplacement;
    public float signTop;

    public enum SignType
    {
        GREEN,
        BLUE,
        BLACK,
        DOUBLEBLACK,
        SKIPATROL,
        FOOD
    }

    private Dictionary<SignType, Color> signTypeColors = new Dictionary<SignType, Color>
    {
        {SignType.GREEN, Color.forestGreen},
        {SignType.BLUE, Color.royalBlue},
        {SignType.BLACK, Color.black},
        {SignType.DOUBLEBLACK, Color.black},
        {SignType.SKIPATROL, Color.darkRed},
        {SignType.FOOD, Color.saddleBrown},
    };

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
            panel.transform.localScale = panel.transform.localScale / this.transform.localScale.x;
            panel.transform.position = panel.transform.position + new UnityEngine.Vector3(0, signTop - (signDisplacement * signNum), 0.1f);

            GameObject graphic = Instantiate(signGraphic, this.transform);
            graphic.transform.localScale = graphic.transform.localScale / this.transform.localScale.x;
            graphic.transform.position = graphic.transform.position + new UnityEngine.Vector3(0, signTop - (signDisplacement * signNum), 0.15f);

            TextMeshProUGUI signText = graphic.GetComponentInChildren<TextMeshProUGUI>();

            signText.SetText(sign.text);

            UnityEngine.UI.Image[] images = graphic.GetComponentsInChildren<UnityEngine.UI.Image>();
            int i = 0;
            foreach (UnityEngine.UI.Image image in images)
            {
                if (i == 0)
                {
                    image.color = signTypeColors.GetValueOrDefault(sign.signType);
                    i++;
                }
            }

            signNum++;
        }
    }

}
