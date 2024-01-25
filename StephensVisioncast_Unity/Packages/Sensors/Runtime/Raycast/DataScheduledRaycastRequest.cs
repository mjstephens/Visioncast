using UnityEngine;

namespace Stephens.Sensors
{
    public struct DataScheduledRaycastRequest
    {
        public Vector3 SourcePosition;
        public Vector3 Direction;
        public float MaxDistance;
        public int LayerMask;
    }
}