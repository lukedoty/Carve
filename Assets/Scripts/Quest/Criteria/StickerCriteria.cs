using UnityEngine;

[CreateAssetMenu(fileName = "New Sticker Criterion Asset", menuName = "Scriptable Objects/Criteria/Sticker Criterion Asset")]
public class StickerCriteria : Criterion
{
    [SerializeField]
    private RegistryIDSelector<Sticker> m_sticker;
    public override bool Check()
    {
        return GameManager.Sticker.HasSticker(m_sticker);
    }
}
