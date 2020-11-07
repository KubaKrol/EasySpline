﻿using System;
using UnityEngine;

namespace EasySpline
{
    /// <summary>
    /// 
    /// </summary>
    public class CubicBezierCurve
    {
        public Vector3 anchor0;
        public Vector3 control0;
        public Vector3 control1;
        public Vector3 anchor1;

        public int stepSize;

        /// <summary>
        /// Constructor with initial coordinates
        /// </summary>
        public CubicBezierCurve()
        {
            stepSize = 100;
            anchor0 = new Vector3(0f, 0f, 0f);
            control0 = new Vector3(3f, 0f, 0f);
            control1 = new Vector3(3f, 0f, 5f);
            anchor1 = new Vector3(0f, 0f, 5f);
        }
        
        public CubicBezierCurve(Vector3 anchor0, Vector3 control0, Vector3 control1, Vector3 anchor1, int stepSize)
        {
            this.anchor0 = anchor0;
            this.control0 = control0;
            this.control1 = control1;
            this.anchor1 = anchor1;
            this.stepSize = stepSize;
        }

        /// <summary>
        /// t [0,1]
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Vector3 GetPosition(float t)
        {
            /*if (t < 0 || t > 1)
                throw new ArgumentOutOfRangeException("t", "must be between 0 and 1");*/
            
            return MathUtility.CubicBezierPoint(anchor0, control0, control1, anchor1, t);
        }

        /// <summary>
        /// t [0,1]
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Vector3 GetDirection(float t)
        {
            /*if (t < 0 || t > 1)
                throw new ArgumentOutOfRangeException("t", "must be between 0 and 1");*/

            return GetPosition(t + 0.1f) - GetPosition(t);
        }
    }
}

