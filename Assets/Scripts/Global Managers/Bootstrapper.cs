using UnityEngine;

public static class Bootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Execute()
    {
        Object.DontDestroyOnLoad(Object.Instantiate(Resources.Load("Game Manager")));
        Debug.Log("Game Manager instantiation by Bootstrapper was successful.");
    }
}