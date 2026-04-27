using UnityEngine;
public enum QuestRewardType
{
    Sticker,
    Quest
}

[System.Serializable]
public struct QuestReward
{
    public QuestRewardType RewardType;
    public string ID;
}
