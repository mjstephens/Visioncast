using Stephens.AI;
using Stephens.Sensors;
using UnityEngine;

namespace Stephens.Player
{
    [RequireComponent(typeof(PlayerInteraction))]
    public class VisionSourcePlayer : VisionSource
    {
        #region VARIABLES

        private PlayerInteraction _playerInteraction;

        #endregion VARIABLES


        #region INITIALIZATION

        private void Awake()
        {
            _playerInteraction = GetComponent<PlayerInteraction>();
        }
        
        #endregion INITIALIZATION


        #region VISION

        protected override void FilterVisionObjectsForInteraction(DataVisioncastResult data)
        {
            base.FilterVisionObjectsForInteraction(data);
            
            if (data == null)
                return;
            
            // Pass data to player interaction manager
            if (_playerInteraction)
            {
                _playerInteraction.OnVisionObjectsGathered(_newlySeenObjects, _newlyLostObjects, _keyObject);
            }
        }

        protected override void ClearVisionData()
        {
            if (_playerInteraction)
            {
                _playerInteraction.OnNoObjectsVisible(_filteredVisionObjects);
            }
            
            base.ClearVisionData();
        }

        #endregion VISION
    }
}