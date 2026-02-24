using Yarn.Unity;

public static class YarnQuestFunctions
{
    [YarnFunction("is-quest-active")]
    public static bool IsQuestActive(string questID) => GameManager.Quest.IsQuestActive(questID);

    [YarnFunction("is-quest-complete")]
    public static bool IsQuestComplete(string questID) => GameManager.Quest.IsQuestComplete(questID);

    [YarnFunction("assign-quest")]
    public static bool AssignQuest(string questID) => GameManager.Quest.AssignQuest(questID);

    [YarnFunction("complete-quest")]
    public static bool CompleteQuest(string questID) => GameManager.Quest.CompleteQuest(questID);
}
