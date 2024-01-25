using UnityEngine;

namespace Stephens.AI
{
    [CreateAssetMenu(
        fileName = "DAT_AIVisionSource_", 
        menuName = "RPG/AI/Vision Source")]
    public class DataConfigAIVisionSource : ScriptableObject
    {
        public LayerMask VisionLayerMask;
        public float VisionRange;
        public Vector2 FieldOfViewRange;
    }
}