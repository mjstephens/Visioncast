using System.Collections.Generic;
using UnityEngine;

namespace GalaxyGourd.Visioncast
{
    /// <summary>
    /// Source that filters visioncast results into more detailed data. 
    /// </summary>
    public class VisioncastSourceFiltered : VisioncastSource
    {
        #region VARIABLES

        [Header("Data")]
        [SerializeField] private DataConfigVisioncastSource _dataInteraction;

        public override LayerMask VisionLayers => _dataInteraction.VisionLayerMask;
        public override LayerMask ObstructionLayers => _dataInteraction.ObstructionLayerMask;
        public override float Range => _dataInteraction.VisionRange;
        public override float FieldOfView => _dataInteraction.FieldOfView;

        /// <summary>
        /// All objects currently seen by this source
        /// </summary>
        protected List<DataVisionSeenObject> _filteredVisionObjects = new ();
        /// <summary>
        /// The object most directly in the center of the source's field of view
        /// </summary>
        protected IVisioncastTargetable _keyObject;
        /// <summary>
        /// A list of objects that were newly seen since the most recent visioncast update 
        /// </summary>
        protected readonly List<IVisioncastTargetable> _newlySeenObjects = new();
        /// <summary>
        /// A list of objects that were newly un-seen since the most recent visioncast update
        /// </summary>
        protected readonly List<IVisioncastTargetable> _newlyLostObjects = new();

        #endregion VARIABLES
        

        #region VISION

        protected override void OnReceiveResults(DataVisioncastResult data)
        {
            FilterVisionObjectsForInteraction(data);
        }

        protected virtual void FilterVisionObjectsForInteraction(DataVisioncastResult data)
        {
            // If there are no objects visible we can get out
            if (data.Objects == null)
            {
                ClearVisionData();
                return;
            }
            
            // Resolve seen objects into workable data
            List<DataVisionSeenObject> newResults = VisioncastResultsResolver.Resolve<IVisioncastTargetable>(
                data, 
                _filteredVisionObjects);
            _newlySeenObjects.Clear();
            _newlyLostObjects.Clear();

            // We need to act on the objects that were seen previously, but no more
            for (int i = 0; i < _filteredVisionObjects.Count; i++)
            {
                // If we HAD seen an item but NO LONGER have it in the results array
                if (_filteredVisionObjects[i].IsVisible &&
                    !VisioncastResultsResolver.DataSeenContainsObject(newResults, _filteredVisionObjects[i].ResultObject))
                {
                    _newlyLostObjects.Add(_filteredVisionObjects[i].ResultObject);
                }
            }

            // Collect new objects
            foreach (DataVisionSeenObject visionObject in newResults)
            {
                if (visionObject.JustBecameVisible)
                    _newlySeenObjects.Add(visionObject.ResultObject);
                else if (!visionObject.IsVisible && !_newlyLostObjects.Contains(visionObject.ResultObject))
                    _newlyLostObjects.Add(visionObject.ResultObject);
            }

            _filteredVisionObjects = newResults;
            
            // The object closest to the view center is our key object
            float closestAngle = float.MaxValue;
            _keyObject = null;
            foreach (DataVisionSeenObject obj in _filteredVisionObjects)
            {
                if (obj.IsVisible)
                {
                    obj.ResultObject.Seen();
                }
                
                if (obj.IsVisible && obj.Angle < closestAngle)
                {
                    closestAngle = obj.Angle;
                    _keyObject = obj.ResultObject;
                }
            }
        }

        protected virtual void ClearVisionData()
        {
            _filteredVisionObjects.Clear();
            _newlySeenObjects.Clear();
            _newlyLostObjects.Clear();
        }

        #endregion VISION
    }
}