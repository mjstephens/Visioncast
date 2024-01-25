using UnityEngine;

namespace Stephens.Sensors
{
    /// <summary>
    /// Basic implementation of visioncast source
    /// </summary>
    public class VisionSourceSimple : VisioncastSource
    {
        #region VARIABLES

        [Header("Vision Values")]
        [SerializeField] private LayerMask _mask;
        [SerializeField] private float _range;
        [SerializeField] private Vector2 _fieldOfViewRange;

        public override LayerMask VisionLayer => _mask;
        public override float Range => _range;
        public override Vector2 FieldOfViewRange => _fieldOfViewRange;

        #endregion VARIABLES
    }
}