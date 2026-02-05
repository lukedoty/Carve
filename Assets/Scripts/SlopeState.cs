using MessagePack;

[MessagePackObject(keyAsPropertyName: true), System.Serializable]
public class SlopeState
{
    //slope name or pointer
    public bool Discovered = false;
    public bool Finished = false;
    public float BestTime = 0;
    public float BestScore = 0;
}
