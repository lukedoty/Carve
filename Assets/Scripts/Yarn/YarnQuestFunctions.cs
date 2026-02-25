using Yarn.Unity;

public static class YarnQuestFunctions
{
    [YarnFunction("is-quest-active")]
    public static bool IsQuestActive(string questID) => GameManager.Quest.IsQuestActive(questID);

    [YarnFunction("is-quest-complete")]
    public static bool IsQuestComplete(string questID) => GameManager.Quest.IsQuestComplete(questID);

    [YarnCommand("assign-quest")]
    public static void AssignQuest(string questID) => GameManager.Quest.AssignQuest(questID);

    [YarnCommand("complete-quest")]
    public static void CompleteQuest(string questID) => GameManager.Quest.CompleteQuest(questID);
}
