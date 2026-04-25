using UnityEngine;

[CreateAssetMenu(fileName = "New Playtime Criterion Asset", menuName = "Scriptable Objects/Criteria/Playtime Criterion Asset")]
public class PlaytimeCriterion : Criterion
{
    [SerializeField]
    private int m_targetPlaytimeSeconds;

    public override bool Check()
    {
        if (GameManager.ActiveState.TimePlayedSeconds >= m_targetPlaytimeSeconds) return true;
        return false;
    }
}