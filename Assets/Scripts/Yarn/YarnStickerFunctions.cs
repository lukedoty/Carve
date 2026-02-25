using Yarn.Unity;

public static class YarnStickerFunctions
{
    [YarnFunction("has-sticker")]
    public static bool HasSticker(string stickerID) => GameManager.Sticker.HasSticker(stickerID);

    [YarnCommand("award-sticker")]
    public static void AwardSticker(string stickerID) => GameManager.Sticker.AwardSticker(stickerID);
}