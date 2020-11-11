using System;
using UnityEngine;

namespace EasySpline
{
    /// <summary>
    /// Spline component, used to display the bezier spline
    /// </summary>
    [ExecuteInEditMode]
    public class Spline : MonoBehaviour
    {
        [SerializeField]
        [Range(1, 200)]
        private int stepsPerCurve = 100;

        [SerializeField] public BezierSpline myBezierSpline;

        private float t = 0f;
        
        /// <summary>
        /// temporary
        /// </summary>
        private void Update()
        {
            if (Input.GetKey(KeyCode.P))
            {
                t += Time.deltaTime;
                if (t > 1f)
                {
                    t = 0f;
                }
            }
        }

        /// <summary>
        /// temporary
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(myBezierSpline.GetPosition(t), 1f);
        }

        public void AddCurveFront()
        {
            myBezierSpline.AddCurveFront();
        }

        public void AddCurveBack()
        {
            myBezierSpline.AddCurveBack();
        }

        public void DeleteAnchor(ControlPoint controlPoint)
        {
            myBezierSpline.DeleteAnchor(controlPoint);
        }
    }   
}
