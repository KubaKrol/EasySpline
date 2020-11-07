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

        public BezierSpline myBezierSpline { get; private set; }
        
        private void Update()
        {
            if (myBezierSpline == null)
            {
                myBezierSpline = new BezierSpline(stepsPerCurve);
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                AddCurveFront();
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                AddCurveBack();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetSpline();
            }
        }

        public void AddCurveFront()
        {
            myBezierSpline.AddCurveFront();
        }

        public void AddCurveBack()
        {
            myBezierSpline.AddCurveBack();
        }

        public void ResetSpline()
        {
            myBezierSpline = new BezierSpline(stepsPerCurve);
        }
    }   
}
