using System;
using UnityEngine;

namespace EasySpline
{
    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public class CubicBezierCurve
    {
        [SerializeReference] public ControlPoint anchor0;
        [SerializeReference] public ControlPoint control0;
        [SerializeReference] public ControlPoint control1;
        [SerializeReference] public ControlPoint anchor1;

        public int stepSize;

        /// <summary>
        /// Constructor with initial coordinates
        /// </summary>
        public CubicBezierCurve()
        {
            anchor0 = new ControlPoint(new Vector3(0f, 0f, 0f));
            control0 = new ControlPoint(new Vector3(3f, 0f, 0f));
            control1 = new ControlPoint(new Vector3(3f, 0f, 5f));
            anchor1 = new ControlPoint(new Vector3(0f, 0f, 5f));
        }
        
        public CubicBezierCurve(Vector3 position)
        {
            anchor0 = new ControlPoint(new Vector3(position.x - 8f, position.y + 0f, position.z - 10f));
            control0 = new ControlPoint(new Vector3(position.x, position.y + 0f, position.z - 10f));
            control1 = new ControlPoint(new Vector3(position.x, position.y + 0f, position.z + 10f));
            anchor1 = new ControlPoint(new Vector3(position.x + 8f, position.y + 0f, position.z + 10f));
        }
        
        public CubicBezierCurve(ControlPoint anchor0, ControlPoint control0, ControlPoint control1, ControlPoint anchor1)
        {
            this.anchor0 = anchor0;
            this.control0 = control0;
            this.control1 = control1;
            this.anchor1 = anchor1;
        }

        /// <summary>
        /// t [0,1]
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Vector3 GetPosition(float t)
        {
            return MathUtility.CubicBezierPoint(anchor0.position, control0.position, control1.position, anchor1.position, t);
        }

        /// <summary>
        /// t [0,1]
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Vector3 GetDirection(float t)
        {
            return GetPosition(t) - GetPosition(t - 0.01f);
        }
    }
}

