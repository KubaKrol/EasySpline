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
        private Vector3 splinePosition;
        
        private int stepSize = 100;

        private ControlPoint selectedAnchor;
        private ControlPoint selectedControlPoint;
        private int selectedAnchorIndex = 0;

        private bool mirroredControlPoints = true;
        private bool rotatingAnchors;
        private bool showMainHandle = true;
        
        private void OnEnable()
        {
            spline = target as Spline;
            
            if (spline.myBezierSpline == null)
                spline.myBezierSpline = new BezierSpline(spline.transform.position);

            splinePosition = spline.transform.position;
            selectedAnchor = null;
            selectedControlPoint = null;
        }
        
        public override void OnInspectorGUI()
        {
            GUILayout.BeginVertical("HelpBox");
            var labelStyle = GUI.skin.GetStyle("Label");
            labelStyle.alignment = TextAnchor.UpperCenter;
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.fontSize = 15;
            GUILayout.Label("Spline settings", labelStyle);
            
            GUILayout.BeginHorizontal();
            
            if(GUILayout.Button("AddCurveBack", GUILayout.Height(30)))
                spline.AddCurveBack();
            
            if (GUILayout.Button("AddCurveFront", GUILayout.Height(30)))
                spline.AddCurveFront();

            GUILayout.EndHorizontal();

            mirroredControlPoints = EditorGUILayout.Toggle("Mirrored control points", mirroredControlPoints);
            rotatingAnchors = EditorGUILayout.Toggle("Rotating anchors", rotatingAnchors);
            showMainHandle = EditorGUILayout.Toggle("Show Main Handle", showMainHandle);
            GUILayout.EndVertical();
            
            GUILayout.BeginVertical("HelpBox");
            GUILayout.Label("Anchor settings", labelStyle);
            labelStyle.alignment = TextAnchor.MiddleLeft;
            if (selectedAnchor == null) 
                GUILayout.Label("Selected Anchor: none");
            else
            {
                GUILayout.Label("Selected Anchor: " + selectedAnchorIndex / 3f);
                GUILayout.Space(3);
                selectedAnchor.position = EditorGUILayout.Vector3Field("position:", selectedAnchor.position);
                var Vect3 = EditorGUILayout.Vector3Field("rotation:", new Vector4(selectedAnchor.rotation.eulerAngles.x, selectedAnchor.rotation.eulerAngles.y, selectedAnchor.rotation.eulerAngles.z));
                selectedAnchor.rotation = new Quaternion(Quaternion.Euler(Vect3).x,Quaternion.Euler(Vect3).y,Quaternion.Euler(Vect3).z,Quaternion.Euler(Vect3).w);
                selectedAnchor.scale = EditorGUILayout.Vector3Field("scale:", selectedAnchor.scale);
                GUILayout.Space(5);
                if(GUILayout.Button("DeleteSelectedAnchor", GUILayout.Height(30)))
                    spline.DeleteAnchor(selectedAnchor);
            }
            GUILayout.EndVertical();

            Repaint();
        }

        private void OnSceneGUI()
        {
            DrawSpline();
            //DrawSplineDirections(5);
            DrawAllSplineHandles();
            CheckKeyboardInput();
            
            if (selectedAnchor != null && selectedControlPoint == null)
            {
                if (!rotatingAnchors)
                {
                    DrawPositioningHandleForControlPoint(selectedAnchor);      
                }
                else
                {
                    DrawRotationHandleForControlPoint(selectedAnchor);
                }
            }
            
            if (selectedControlPoint != null)
                DrawPositioningHandleForControlPoint(selectedControlPoint);

            if (showMainHandle)
            {
                Tools.hidden = false;
                ApplySplinePosition();
            }
            else
            {
                Tools.hidden = true;
            }
        }

        private void DrawSpline()
        {
            //Draw curves
            for (int i = 0; i < spline.myBezierSpline.myCurves.Count; i++)
            {
                var currentCurve = spline.myBezierSpline.myCurves[i];
                Handles.DrawBezier(currentCurve.anchor0.position, currentCurve.anchor1.position, currentCurve.control0.position, currentCurve.control1.position, Color.white, null, 1f);
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
        
        private void ApplySplinePosition()
        {
            var positionDelta = spline.transform.position - splinePosition;
            for (int i = 0; i < spline.myBezierSpline.allControlPoints.Count; i++)
            {
                spline.myBezierSpline.allControlPoints[i].position += positionDelta;
            }
            splinePosition = spline.transform.position;
        }
        
        private void DrawPositioningHandleForControlPoint(ControlPoint controlPoint)
        {
            EditorGUI.BeginChangeCheck();
            Vector3 newControlPointPosition = Handles.PositionHandle(controlPoint.position, Quaternion.identity); //Local or global orientation
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spline, "Change controlPoint position");
                var positionDelta = newControlPointPosition - controlPoint.position;
                controlPoint.position = newControlPointPosition;
                
                if (controlPoint == selectedAnchor)
                {
                    var adjacentPoints = spline.myBezierSpline.GetAnchorControlPoints(controlPoint);
                    foreach (var point in adjacentPoints)
                        point.position += positionDelta;
                }
                if (mirroredControlPoints && controlPoint == selectedControlPoint)
                {
                    var adjacentPoints = spline.myBezierSpline.GetAnchorControlPoints(selectedAnchor);
                    foreach (var point in adjacentPoints)
                        if (point != controlPoint)
                            point.position = selectedAnchor.position - (controlPoint.position - selectedAnchor.position);
                }
            } 
        }

        private void DrawRotationHandleForControlPoint(ControlPoint controlPoint)
        {
            EditorGUI.BeginChangeCheck();
            Quaternion newControlPointRotation = Handles.RotationHandle(controlPoint.rotation, controlPoint.position);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spline, "Change controlPoint position");
                var rotationDelta = Quaternion.Angle(newControlPointRotation, controlPoint.rotation);
                controlPoint.rotation = newControlPointRotation;
            }  
        }

        private void DrawAllSplineHandles()
        {
            for (int i = 0; i < spline.myBezierSpline.allControlPoints.Count; i += 3)
            {
                var currentControlPoint = spline.myBezierSpline.allControlPoints[i];
                
                Handles.color = Color.white;
                if (Handles.Button(currentControlPoint.position, Quaternion.identity, 1.75f, 2f, Handles.SphereHandleCap))
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
                    Handles.color = Color.gray;
                    if (Handles.Button(controlPoint.position, Quaternion.identity, 1f, 1f, Handles.CubeHandleCap))
                    {
                        selectedControlPoint = controlPoint;
                    }
                    
                    Handles.DrawLine(selectedAnchor.position, controlPoint.position);
                }   
            }
        }

        private void CheckKeyboardInput()
        {
            Event e = Event.current;
            switch (e.type)
            {
                case EventType.KeyDown:
                {
                    /*if (Event.current.keyCode == (KeyCode.E))
                    {
                        rotating = true;
                        e.Use();
                    }

                    if (Event.current.keyCode == (KeyCode.W))
                    {
                        rotating = false;
                        e.Use();
                    }*/
                    
                    if (Event.current.keyCode == (KeyCode.M))
                    {
                        if (mirroredControlPoints)
                            mirroredControlPoints = false;
                        else
                            mirroredControlPoints = true;
                        
                        e.Use();
                    }
                    
                    break;
                }
            }
        }
    }   
}
