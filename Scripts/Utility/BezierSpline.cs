using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasySpline
{
    /// <summary>
    /// Spline is made out of CubicBezierCurves
    /// </summary>
    [System.Serializable]
    public class BezierSpline
    {
        [SerializeReference] public List<CubicBezierCurve> myCurves;

        public BezierSpline()
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
            var newControl0 = new ControlPoint(lastCurve.anchor1.position + lastCurve.GetDirection(1f).normalized);
            var newControl1 = new ControlPoint(lastCurve.anchor1.position + lastCurve.GetDirection(1f).normalized * 2f);
            var newAnchor1 = new ControlPoint(lastCurve.anchor1.position + lastCurve.GetDirection(1f).normalized * 3f);
            
            var newCurve = new CubicBezierCurve(newAnchor0, newControl0, newControl1, newAnchor1);
            myCurves.Add(newCurve);
        }
        
        /// <summary>
        /// Add new curve to the back of the spline
        /// </summary>
        public void AddCurveBack()
        {
            var lastCurve = myCurves[0];
            
            var newAnchor0 = new ControlPoint(lastCurve.anchor0.position - lastCurve.GetDirection(0f).normalized * 3f);
            var newControl0 = new ControlPoint(lastCurve.anchor0.position - lastCurve.GetDirection(0f).normalized * 2f);
            var newControl1 = new ControlPoint(lastCurve.anchor0.position - lastCurve.GetDirection(0f).normalized);
            var newAnchor1 = lastCurve.anchor0;
            
            var newCurve = new CubicBezierCurve(newAnchor0, newControl0, newControl1, newAnchor1);
            myCurves.Insert(0, newCurve);
        }

        /// <summary>
        /// Delete anchor by index from beginning of the spline
        /// </summary>
        /// <param name="anchorIndex"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void DeleteAnchor(int anchorIndex)
        {
            if (anchorIndex < 0 || anchorIndex > myCurves.Count)
                throw new ArgumentOutOfRangeException("anchorIndex", "Out of range");

            if (anchorIndex > 0 && anchorIndex < myCurves.Count)
            {
                var adjacentCurvesIndexes = new[] {anchorIndex - 1, anchorIndex};

                var newAnchor0 = myCurves[adjacentCurvesIndexes[0]].anchor0;
                var newControl0 = myCurves[adjacentCurvesIndexes[0]].control0;
                var newControl1 = myCurves[adjacentCurvesIndexes[1]].control1;
                var newAnchor1 = myCurves[adjacentCurvesIndexes[1]].anchor1;

                var newCurve = new CubicBezierCurve(newAnchor0, newControl0, newControl1, newAnchor1);
                
                myCurves.RemoveAt(adjacentCurvesIndexes[0]);
                myCurves.RemoveAt(adjacentCurvesIndexes[0]);
                
                myCurves.Insert(anchorIndex-1, newCurve);
            }
            else
            {
                if (anchorIndex == 0)
                {
                    myCurves.RemoveAt(0);
                }
                if (anchorIndex == myCurves.Count)
                {
                    myCurves.RemoveAt(myCurves.Count - 1);
                }
            }
        }

        /// <summary>
        /// Will create additional curve connecting first anchor with the last anchor
        /// </summary>
        public void CreateLoop()
        {
            
        }

        /// <summary>
        /// t [0,1] - spline progress
        /// </summary>
        /// <param name="t"></param>
        public Vector3 GetPosition(float t)
        {
            if(t < 0 || t > 1f)
                throw new ArgumentOutOfRangeException("t", "out of range");
            
            float curveIndex = t * myCurves.Count;

            if (Mathf.FloorToInt(curveIndex) > 0 && Mathf.FloorToInt(curveIndex) < myCurves.Count - 1)
            {
                float startingValue = (float)Mathf.FloorToInt(curveIndex) / myCurves.Count;
                float endingValue = ((Mathf.FloorToInt(curveIndex) + 1f) / myCurves.Count);
                return myCurves[Mathf.FloorToInt(curveIndex)].GetPosition(Mathf.InverseLerp(startingValue, endingValue, t));
            }

            if (Mathf.FloorToInt(curveIndex) == 0)
            {
                float startingValue = 0f;
                float endingValue = 1f / myCurves.Count;
                return myCurves[Mathf.FloorToInt(curveIndex)].GetPosition(Mathf.InverseLerp(startingValue, endingValue, t));
            }

            if (Mathf.FloorToInt(curveIndex) == myCurves.Count - 1)
            {
                float startingValue = (myCurves.Count - 1f) / myCurves.Count;
                float endingValue = 1f;
                return myCurves[Mathf.FloorToInt(curveIndex)].GetPosition(Mathf.InverseLerp(startingValue, endingValue, t));
            }
            
            return Vector3.zero;
        }
    }   
}
