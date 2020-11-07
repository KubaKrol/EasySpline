using UnityEngine;

namespace EasySpline
{
    /// <summary>
    /// 
    /// </summary>
    public static class MathUtility
    {
        /// <summary>
        /// t [0,1]
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <param name="D"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vector3 CubicBezierPoint(Vector3 A, Vector3 B, Vector3 C, Vector3 D, float t)
        {
            var x = A.x * Mathf.Pow((1f - t), 3f) + 3f * B.x * t * Mathf.Pow((1 - t), 2) + 3f * C.x * Mathf.Pow(t, 2) * (1 - t) + D.x * Mathf.Pow(t, 3);
            var y = A.y * Mathf.Pow((1f - t), 3f) + 3f * B.y * t * Mathf.Pow((1 - t), 2) + 3f * C.y * Mathf.Pow(t, 2) * (1 - t) + D.y * Mathf.Pow(t, 3);
            var z = A.z * Mathf.Pow((1f - t), 3f) + 3f * B.z * t * Mathf.Pow((1 - t), 2) + 3f * C.z * Mathf.Pow(t, 2) * (1 - t) + D.z * Mathf.Pow(t, 3);
            
            return new Vector3(x, y, z);
        }
    }
}

