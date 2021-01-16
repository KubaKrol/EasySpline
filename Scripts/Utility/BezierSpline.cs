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
        [SerializeReference] public List<ControlPoint> allControlPoints;

        public BezierSpline()
        {
            myCurves = new List<CubicBezierCurve>();
            var initialCurve = new CubicBezierCurve();
            myCurves.Add(initialCurve);
            UpdateControlPointsList();
        }
        
        public BezierSpline(Vector3 position)
        {
            myCurves = new List<CubicBezierCurve>();
            var initialCurve = new CubicBezierCurve(position);
            myCurves.Add(initialCurve);
            UpdateControlPointsList();
        }
        
        /// <summary>
        /// Add new curve to the front of the spline
        /// </summary>
        public void AddCurveFront()
        {
            var lastCurve = myCurves[myCurves.Count-1];
            
            var newAnchor0 = lastCurve.anchor1;
            var newControl0 = new ControlPoint(lastCurve.anchor1.position + lastCurve.GetDirection(1f).normalized * 4f);
            var newControl1 = new ControlPoint(lastCurve.anchor1.position + lastCurve.GetDirection(1f).normalized * 8f);
            var newAnchor1 = new ControlPoint(lastCurve.anchor1.position + lastCurve.GetDirection(1f).normalized * 12f);
            
            var newCurve = new CubicBezierCurve(newAnchor0, newControl0, newControl1, newAnchor1);
            myCurves.Add(newCurve);
            UpdateControlPointsList();
        }
        
        /// <summary>
        /// Add new curve to the back of the spline
        /// </summary>
        public void AddCurveBack()
        {
            var lastCurve = myCurves[0];
            
            var newAnchor0 = new ControlPoint(lastCurve.anchor0.position - lastCurve.GetDirection(0f).normalized * 12f);
            var newControl0 = new ControlPoint(lastCurve.anchor0.position - lastCurve.GetDirection(0f).normalized * 8f);
            var newControl1 = new ControlPoint(lastCurve.anchor0.position - lastCurve.GetDirection(0f).normalized * 4f);
            var newAnchor1 = lastCurve.anchor0;
            
            var newCurve = new CubicBezierCurve(newAnchor0, newControl0, newControl1, newAnchor1);
            myCurves.Insert(0, newCurve);
            UpdateControlPointsList();
        }

        /// <summary>
        /// Delete anchor by index from beginning of the spline
        /// </summary>
        /// <param name="anchorIndex"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void DeleteAnchor(ControlPoint anchor)
        {
            if (allControlPoints.Count <= 4)
                return;
            
            var anchorIndex = allControlPoints.IndexOf(anchor) / 3;
            
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
            UpdateControlPointsList();
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

            float startingValue = (float)Mathf.FloorToInt(curveIndex) / myCurves.Count;
            float endingValue = ((Mathf.FloorToInt(curveIndex) + 1f) / myCurves.Count);
            return myCurves[Mathf.FloorToInt(curveIndex)].GetPosition(Mathf.InverseLerp(startingValue, endingValue, t));
        }

        /// <summary>
        /// t [0,1] - spline progress
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Vector3 GetDirection(float t)
        {
            if(t < 0 || t > 1f)
                throw new ArgumentOutOfRangeException("t", "out of range");
            
            float curveIndex = t * myCurves.Count;

            float startingValue = (float)Mathf.FloorToInt(curveIndex) / myCurves.Count;
            float endingValue = ((Mathf.FloorToInt(curveIndex) + 1f) / myCurves.Count);
            return myCurves[Mathf.FloorToInt(curveIndex)].GetDirection(Mathf.InverseLerp(startingValue, endingValue, t));    
        }
        
        /// <summary>
        /// Adjacent control points of an anchor
        /// </summary>
        /// <param name="anchor"></param>
        /// <returns></returns>
        public ControlPoint[] GetAnchorControlPoints(ControlPoint anchor)
        {
            if (!allControlPoints.Contains(anchor))
                return null;

            var index = allControlPoints.IndexOf(anchor);
            if (index == 0)
                return new[] {allControlPoints[1]};

            return index == allControlPoints.Count - 1 ? new[] {allControlPoints[allControlPoints.Count - 2]} : new[] {allControlPoints[index - 1], allControlPoints[index + 1]};
        }


        private void UpdateControlPointsList()
        {
            if (allControlPoints == null)
                allControlPoints = new List<ControlPoint>();
            
            allControlPoints.Clear();
            
            for (int i = 0; i < myCurves.Count; i++)
            {
                AddControlPointToList(myCurves[i].anchor0);
                AddControlPointToList(myCurves[i].control0);
                AddControlPointToList(myCurves[i].control1);
                AddControlPointToList(myCurves[i].anchor1);
            }
        }

        private void AddControlPointToList(ControlPoint controlPoint)
        {
            if (!allControlPoints.Contains(controlPoint))
            {
                allControlPoints.Add(controlPoint);
            }
        }
    }   
}
