using UnityEngine;

public static class QuaternionExtensions
{
    public static Quaternion Round(this Quaternion q)
    {
        return new Quaternion(
            Mathf.Round(q.x * 1000f) / 1000f,
            Mathf.Round(q.y * 1000f) / 1000f,
            Mathf.Round(q.z * 1000f) / 1000f,
            Mathf.Round(q.w * 1000f) / 1000f
        );
    }
}
