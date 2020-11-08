using UnityEngine;

namespace EasySpline
{
    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public class ControlPoint
    {
        public ControlPoint(Vector3 position)
        {
            this.position = position;
            rotation = Quaternion.identity;
            scale = new Vector3(1f, 1f, 1f);
        }
        
        public ControlPoint(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }

        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }   
}
