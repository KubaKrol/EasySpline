using System;
using UnityEditor;
using UnityEngine;

namespace EasySpline
{
    /// <summary>
    /// 
    /// </summary>
    [CustomEditor(typeof(Spline))]
    public class SplineEditor : Editor
    {
        private Spline spline;
        
        private int stepSize = 100;

        private ControlPoint selectedAnchor;
        private int selectedAnchorIndex = 0;

        private ControlPoint selectedControlPoint;

        private void OnEnable()
        {
            spline = target as Spline;
            
            if (spline.myBezierSpline == null)
            {
                spline.myBezierSpline = new BezierSpline();
            }
        }

        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("AddCurveFront"))
            {
                spline.AddCurveFront();
            }
            
            if(GUILayout.Button("AddCurveBack"))
            {
                spline.AddCurveBack();
            }
            
            if(GUILayout.Button("DeleteSelectedAnchor"))
            {
                spline.DeleteAnchor(selectedAnchorIndex);
            }
        }

        private void OnSceneGUI()
        {
            DrawSplineBuiltIn();
            DrawSplineDirections(5);
            DrawAllSplineHandles();

            if (selectedAnchor != null && selectedControlPoint == null)
            {
                DrawPositioningHandleForControlPoint(selectedAnchor);   
            }

            if (selectedControlPoint != null)
            {
                DrawPositioningHandleForControlPoint(selectedControlPoint);   
            }
        }

        private void DrawSplineBuiltIn()
        {
            //Draw curves
            for (int i = 0; i < spline.myBezierSpline.myCurves.Count; i++)
            {
                var currentCurve = spline.myBezierSpline.myCurves[i];
                Handles.DrawBezier(currentCurve.anchor0.position, currentCurve.anchor1.position, currentCurve.control0.position, currentCurve.control1.position, Color.white, null, 1f);
            }
        }

        private void DrawSplineDefault()
        {
            for (int i = 0; i < spline.myBezierSpline.myCurves.Count; i++)
            {
                var currentCurve = spline.myBezierSpline.myCurves[i];
                for (int j = 0; j < stepSize; j++)
                {
                    Handles.color = Color.blue;
                    var t1 = Mathf.InverseLerp(0f, stepSize, j);
                    var t2 = Mathf.InverseLerp(0f, stepSize, j+1f);
                    Handles.DrawLine(currentCurve.GetPosition(t1), currentCurve.GetPosition(t2));
                }
            }
        }
        
        private void DrawSplineDirections(int offset)
        {
            //Draw directions
            for (int i = 0; i < spline.myBezierSpline.myCurves.Count; i++)
            {
                var currentCurve = spline.myBezierSpline.myCurves[i];
                for (int j = 0; j < stepSize; j+=offset)
                {
                    Handles.color = Color.green;
                    var t = Mathf.InverseLerp(0f, stepSize, j);
                    Handles.DrawLine(currentCurve.GetPosition(t), currentCurve.GetPosition(t) + currentCurve.GetDirection(t) * 10f);
                }
            }
        }

        private void DrawPositioningHandleForControlPoint(ControlPoint controlPoint)
        {
            EditorGUI.BeginChangeCheck();
            Vector3 newControlPointPosition = Handles.PositionHandle(controlPoint.position, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spline, "Change controlPoint position");
                var positionDelta = newControlPointPosition - controlPoint.position;
                controlPoint.position = newControlPointPosition;
                if (controlPoint == selectedAnchor)
                {
                    var adjacentPoints = spline.myBezierSpline.GetAnchorControlPoints(controlPoint);

                    foreach (var point in adjacentPoints)
                    {
                        point.position += positionDelta;
                    }
                }
            } 
        }
        
        private void DrawAllSplineHandles()
        {
            for (int i = 0; i < spline.myBezierSpline.allControlPoints.Count; i += 3)
            {
                var currentControlPoint = spline.myBezierSpline.allControlPoints[i];
                
                Handles.color = Color.red;
                if (Handles.Button(currentControlPoint.position, Quaternion.identity, 2f, 2f, Handles.SphereHandleCap))
                {
                    selectedAnchor = currentControlPoint;
                    selectedAnchorIndex = i;
                    selectedControlPoint = null;
                }
            }
            
            if (spline.myBezierSpline.GetAnchorControlPoints(selectedAnchor) != null)
            {
                foreach (var controlPoint in spline.myBezierSpline.GetAnchorControlPoints(selectedAnchor))
                {
                    Handles.color = Color.yellow;
                    if (Handles.Button(controlPoint.position, Quaternion.identity, 2f, 2f, Handles.SphereHandleCap))
                    {
                        selectedControlPoint = controlPoint;
                    }
                    
                    Handles.color = Color.red;
                    Handles.DrawLine(selectedAnchor.position, controlPoint.position);
                }   
            }
        }
    }   
}
