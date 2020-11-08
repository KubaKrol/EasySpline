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
            
            if(GUILayout.Button("Delete0Anchor"))
            {
                spline.DeleteAnchor(0);
            }
            
            if(GUILayout.Button("Delete2Anchor"))
            {
                spline.DeleteAnchor(2);
            }
            
            if(GUILayout.Button("DeleteLastAnchor"))
            {
                spline.DeleteAnchor(spline.myBezierSpline.myCurves.Count * 2 - 1);
            }
        }

        private void OnSceneGUI()
        {
            DrawSplineHandles(spline);
            //DrawSplineDirections(spline, 5);
            DrawAllSplineHandles(spline);
        }

        private void DrawSplineHandles(Spline spline)
        {
            //Draw curves
            for (int i = 0; i < spline.myBezierSpline.myCurves.Count; i++)
            {
                var currentCurve = spline.myBezierSpline.myCurves[i];
                Handles.DrawBezier(currentCurve.anchor0.position, currentCurve.anchor1.position, currentCurve.control0.position, currentCurve.control1.position, Color.white, null, 1f);
            }
        }

        private void DrawSplineDefault(Spline spline)
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
        
        private void DrawSplineDirections(Spline spline, int offset)
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
        
        private void DrawAllSplineHandles(Spline spline)
        { 
            //Draw handles
            for (int i = 0; i < spline.myBezierSpline.myCurves.Count; i++)
            {
                var currentCurve = spline.myBezierSpline.myCurves[i];

                EditorGUI.BeginChangeCheck();
                Vector3 newAnchor0Position = Handles.PositionHandle(currentCurve.anchor0.position, Quaternion.identity);
                Vector3 newAnchor1Position = Handles.PositionHandle(currentCurve.anchor1.position, Quaternion.identity);
                Vector3 newControl0Position = Handles.PositionHandle(currentCurve.control0.position, Quaternion.identity);
                Vector3 newControl1Position = Handles.PositionHandle(currentCurve.control1.position, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(spline, "Change anchor0 position");
                    currentCurve.anchor0.position = newAnchor0Position;
                    currentCurve.anchor1.position = newAnchor1Position;
                    currentCurve.control0.position = newControl0Position;
                    currentCurve.control1.position = newControl1Position;
                }

                Handles.color = Color.red;
                Handles.DrawWireCube(currentCurve.anchor0.position, new Vector3(1f, 1f, 1f));
                Handles.DrawWireCube(currentCurve.anchor1.position, new Vector3(1f, 1f, 1f));
                Handles.color = Color.magenta;
                Handles.DrawWireCube(currentCurve.control0.position, new Vector3(0.5f, 0.5f, 0.5f));
                Handles.DrawWireCube(currentCurve.control1.position, new Vector3(0.5f, 0.5f, 0.5f));
                Handles.color = Color.red;
                Handles.DrawLine(currentCurve.anchor0.position, currentCurve.control0.position);
                Handles.DrawLine(currentCurve.anchor1.position, currentCurve.control1.position);
            }
        }
    }   
}
