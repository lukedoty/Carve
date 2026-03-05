using UnityEngine;

public static class MathHelpers
{
    public static float MaxMagnitude(float a, float b)
    {
        if (Mathf.Abs(a) > Mathf.Abs(b)) return a;
        return b;
    }
}
