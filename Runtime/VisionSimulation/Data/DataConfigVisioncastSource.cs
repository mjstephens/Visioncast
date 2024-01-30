using UnityEngine;

namespace GalaxyGourd.Visioncast
{
    [CreateAssetMenu(
        fileName = "DAT_AIVisionSource_", 
        menuName = "RPG/AI/Vision Source")]
    public class DataConfigVisioncastSource : ScriptableObject
    {
        public LayerMask VisionLayerMask;
        public LayerMask ObstructionLayerMask;
        public float VisionRange;
        public float FieldOfView;
    }
}