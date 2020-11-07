using System.Collections.Generic;

namespace EasySpline
{
    /// <summary>
    /// 
    /// </summary>
    public class BezierSpline
    {
        public List<CubicBezierCurve> myCurves;
        
        public BezierSpline(int stepsPerCurve)
        {
            myCurves = new List<CubicBezierCurve>();
            var initialCurve = new CubicBezierCurve();
            myCurves.Add(initialCurve);
        }
        
        /// <summary>
        /// Add new curve to the front of the spline
        /// </summary>
        public void AddCurveFront()
        {
            var lastCurve = myCurves[myCurves.Count-1];
            
            var newAnchor0 = lastCurve.anchor1;
            var newControl0 = lastCurve.anchor1 + lastCurve.GetDirection(1f).normalized;
            var newControl1 = lastCurve.anchor1 + lastCurve.GetDirection(1f).normalized * 2f;
            var newAnchor1 = lastCurve.anchor1 + lastCurve.GetDirection(1f).normalized * 3f;
            
            var newCurve = new CubicBezierCurve(newAnchor0, newControl0, newControl1, newAnchor1);
            myCurves.Add(newCurve);
        }
        
        /// <summary>
        /// Add new curve to the back of the spline
        /// </summary>
        public void AddCurveBack()
        {
            var lastCurve = myCurves[0];
            
            var newAnchor0 = lastCurve.anchor0 - lastCurve.GetDirection(0f).normalized * 3f;
            var newControl0 = lastCurve.anchor0 - lastCurve.GetDirection(0f).normalized * 2f;
            var newControl1 = lastCurve.anchor0 - lastCurve.GetDirection(0f).normalized;
            var newAnchor1 = lastCurve.anchor0;
            
            var newCurve = new CubicBezierCurve(newAnchor0, newControl0, newControl1, newAnchor1);
            myCurves.Insert(0, newCurve);
        }
    }   
}
