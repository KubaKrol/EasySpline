using UnityEngine;

namespace EasySpline
{
    [ExecuteInEditMode]
    public class SplineWalker : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private Spline mySpline;

        [Header("Settings")]
        [Range(0f, 1f)]
        [SerializeField] private float progress;
        [SerializeField] private bool includeRotation;

        private void Update()
        {
            if (mySpline != null)
            {
                transform.position = mySpline.myBezierSpline.GetPosition(progress);
                
                if (includeRotation)
                {
                    transform.forward = mySpline.myBezierSpline.GetDirection(progress);   
                }
            }
        }
    }   
}
