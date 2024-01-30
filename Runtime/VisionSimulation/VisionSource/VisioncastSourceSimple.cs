using UnityEngine;

namespace GalaxyGourd.Visioncast
{
    /// <summary>
    /// Basic implementation of visioncast source without complex filtering.
    /// </summary>
    public class VisioncastSourceSimple : VisioncastSource
    {
        #region VARIABLES

        [Header("Vision Values")]
        [SerializeField] private LayerMask _visible;
        [SerializeField] private LayerMask _obstruction;
        [SerializeField] private float _range;
        [SerializeField] private float _fieldOfView;

        public override LayerMask VisionLayers => _visible;
        public override LayerMask ObstructionLayers => _obstruction;
        public override float Range => _range;
        public override float FieldOfView => _fieldOfView;

        #endregion VARIABLES


        #region METHODS

        public void OverrideFieldOfView(float val)
        {
            _fieldOfView = val;
        }

        public void OverrideRange(float val)
        {
            _range = val;
        }

        protected override void OnReceiveResults(DataVisioncastResult data)
        {
            if (data.Objects == null)
                return;
            
            for (int i = 0; i < data.Objects.Count; i++)
            {
                if (data.VisiblePoints[i].Count > 0)
                {
                    data.Objects[i].Seen();
                }
            }
        }

        #endregion METHODS
    }
}