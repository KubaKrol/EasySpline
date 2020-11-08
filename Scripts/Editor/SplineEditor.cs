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
        private int anchorToDelete = 0;

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

            anchorToDelete = EditorGUILayout.IntSlider(anchorToDelete, 0, spline.myBezierSpline.myCurves.Count);
            
            if(GUILayout.Button("DeleteAnchor"))
            {
                spline.DeleteAnchor(anchorToDelete);
            }
        }

        private void OnSceneGUI()
        {
            DrawSplineBuiltIn();
            DrawSplineDirections(5);
            DrawAllSplineHandles();

            if (selectedAnchor != null)
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
            Vector3 newAnchor0Position = Handles.PositionHandle(controlPoint.position, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spline, "Change controlPoint position");
                controlPoint.position = newAnchor0Position;
            } 
        }

        private void DrawPositioningHandlesForEveryControlPoint()
        {
            for (int i = 0; i < spline.myBezierSpline.myCurves.Count; i++)
            {
                var currentCurve = spline.myBezierSpline.myCurves[i];

                EditorGUI.BeginChangeCheck();
                Vector3 newAnchor0Position = Handles.PositionHandle(currentCurve.anchor0.position, Quaternion.identity);
                Vector3 newAnchor1Position = Handles.PositionHandle(currentCurve.anchor1.position, Quaternion.identity);
                Vector3 newControl0Position =
                    Handles.PositionHandle(currentCurve.control0.position, Quaternion.identity);
                Vector3 newControl1Position =
                    Handles.PositionHandle(currentCurve.control1.position, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(spline, "Change anchor0 position");
                    currentCurve.anchor0.position = newAnchor0Position;
                    currentCurve.anchor1.position = newAnchor1Position;
                    currentCurve.control0.position = newControl0Position;
                    currentCurve.control1.position = newControl1Position;
                }
            }
        }
        
        private void DrawAllSplineHandles()
        {
            //Draw handles
            for (int i = 0; i < spline.myBezierSpline.myCurves.Count; i++)
            {
                var currentCurve = spline.myBezierSpline.myCurves[i];

                Handles.color = Color.red;
                if (selectedAnchor != currentCurve.anchor0)
                {
                    if(Handles.Button(currentCurve.anchor0.position, Quaternion.identity, 2f, 2f, Handles.SphereHandleCap))
                    {
                        selectedAnchor = currentCurve.anchor0;
                        selectedAnchorIndex = i;
                        selectedControlPoint = null;
                    }
                }
                if (selectedAnchor != currentCurve.anchor1)
                {
                    if (Handles.Button(currentCurve.anchor1.position, Quaternion.identity, 2f, 2f, Handles.SphereHandleCap))
                    {
                        selectedAnchor = currentCurve.anchor1;
                        selectedAnchorIndex = i + 1;
                        selectedControlPoint = null;
                    }
                }

                Handles.color = Color.yellow;
                if (selectedAnchor == currentCurve.anchor0)
                {
                    if (selectedControlPoint != currentCurve.control0)
                    {
                        if(Handles.Button(currentCurve.control0.position, Quaternion.identity, 2f, 2f, Handles.SphereHandleCap))
                        {
                            selectedControlPoint = currentCurve.control0;
                        }   
                    }

                    if (selectedAnchorIndex > 0)
                    {
                        if (selectedControlPoint != spline.myBezierSpline.myCurves[i - 1].control1)
                        {
                            if(Handles.Button(spline.myBezierSpline.myCurves[i-1].control1.position, Quaternion.identity, 2f, 2f, Handles.SphereHandleCap))
                            {
                                selectedControlPoint = spline.myBezierSpline.myCurves[i - 1].control1;
                            }   
                        }

                        Handles.color = Color.red;
                        Handles.DrawLine(currentCurve.anchor0.position, spline.myBezierSpline.myCurves[i-1].control1.position);
                    }
                    Handles.color = Color.red;
                    Handles.DrawLine(currentCurve.anchor0.position, currentCurve.control0.position);
                }

                if (selectedAnchor == currentCurve.anchor1)
                {
                    if (selectedControlPoint != currentCurve.control1)
                    {
                        if(Handles.Button(currentCurve.control1.position, Quaternion.identity, 2f, 2f, Handles.SphereHandleCap))
                        {
                            selectedControlPoint = currentCurve.control1;
                        }   
                    }
                    
                    Handles.color = Color.red;
                    Handles.DrawLine(currentCurve.anchor1.position, currentCurve.control1.position);
                }
            }
        }
    }   
}
