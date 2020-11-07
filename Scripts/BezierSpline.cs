using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasySpline
{
    /// <summary>
    /// 
    /// </summary>
    [ExecuteInEditMode]
    public class BezierSpline : MonoBehaviour
    {
        public List<CubicBezierCurve> myCurves;
        
        private void Update()
        {
            if (myCurves == null)
            {
                Initialize();
            }
            
            DrawSpline();

            if (Input.GetKeyDown(KeyCode.F))
            {
                AddCurveFront();
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                AddCurveBack();
            }
        }

        public void AddCurveFront()
        {
            var lastCurve = myCurves[myCurves.Count-1];
            
            var newAnchor0 = lastCurve.anchor1;
            var newControl0 = lastCurve.anchor1 + lastCurve.GetDirection(1f).normalized;
            var newControl1 = lastCurve.anchor1 + lastCurve.GetDirection(1f).normalized * 2f;
            var newAnchor1 = lastCurve.anchor1 + lastCurve.GetDirection(1f).normalized * 3f;
            
            var newCurve = new CubicBezierCurve(newAnchor0, newControl0, newControl1, newAnchor1, 100);
            myCurves.Add(newCurve);
        }
        
        public void AddCurveBack()
        {
            var lastCurve = myCurves[0];
            
            var newAnchor0 = lastCurve.anchor0 - lastCurve.GetDirection(0f).normalized * 3f;
            var newControl0 = lastCurve.anchor0 - lastCurve.GetDirection(0f).normalized * 2f;
            var newControl1 = lastCurve.anchor0 - lastCurve.GetDirection(0f).normalized;
            var newAnchor1 = lastCurve.anchor0;
            
            var newCurve = new CubicBezierCurve(newAnchor0, newControl0, newControl1, newAnchor1, 100);
            myCurves.Insert(0, newCurve);
        }

        private void DrawSpline()
        {
            for (int i = 0; i < myCurves.Count; i++)
            {
                var currentCurve = myCurves[i];

                for (int j = 0; j < currentCurve.stepSize; j++)
                {
                    var t = Mathf.InverseLerp(0, currentCurve.stepSize, j);
                    var firstPoint = currentCurve.GetPosition(t);
                    t = Mathf.InverseLerp(0, currentCurve.stepSize, j+1);
                    var nextPoint = currentCurve.GetPosition(t);
                    Debug.DrawLine(firstPoint, nextPoint, Color.blue);
                }
            }
        }
        
        void OnDrawGizmosSelected()
        {
            for (int i = 0; i < myCurves.Count; i++)
            {
                var currentCurve = myCurves[i];
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(currentCurve.anchor0, 1);
                Gizmos.DrawSphere(currentCurve.anchor1, 1);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(currentCurve.control0, 0.5f);
                Gizmos.DrawSphere(currentCurve.control1, 0.5f);
            }
        }

        private void Initialize()
        {
            myCurves = new List<CubicBezierCurve>();
            
            var initialCurve = new CubicBezierCurve();
            myCurves.Add(initialCurve);
        }
    }   
}
