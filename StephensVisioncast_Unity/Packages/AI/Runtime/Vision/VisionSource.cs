using System.Collections.Generic;
using Stephens.Sensors;
using UnityEngine;

namespace Stephens.AI
{
    public class VisionSource : VisioncastSource
    {
        #region VARIABLES

        [Header("Data")]
        [SerializeField] private DataConfigAIVisionSource _dataInteraction;

        public override LayerMask VisionLayer => _dataInteraction.VisionLayerMask;
        public override float Range => _dataInteraction.VisionRange;
        public override Vector2 FieldOfViewRange => _dataInteraction.FieldOfViewRange;

        // We keep track of all visible objects
        protected List<DataVisionSeenObject> _filteredVisionObjects = new ();
        protected IVisibleObject _keyObject;
        protected readonly List<IVisibleObject> _newlySeenObjects = new();
        protected readonly List<IVisibleObject> _newlyLostObjects = new();

        #endregion VARIABLES
        

        #region VISION

        protected override void OnReceiveResults(DataVisioncastResult data)
        {
            base.OnReceiveResults(data);

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
            List<DataVisionSeenObject> newResults = 
                VisioncastResultsResolver.Resolve<IVisibleObject>(data, _filteredVisionObjects);
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