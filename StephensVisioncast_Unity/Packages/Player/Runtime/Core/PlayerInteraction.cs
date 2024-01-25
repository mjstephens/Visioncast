using System;
using System.Collections.Generic;
using Stephens.Sensors;
using UnityEngine;

namespace Stephens.Player
{
    public class PlayerInteraction : MonoBehaviour
    {
        #region VARIABLES


        
        #endregion VARIABLES


        #region INITIALIZATION

        
        
        #endregion INITIALIZATION


        #region VISION

        internal void OnVisionObjectsGathered(List<IVisibleObject> seen, List<IVisibleObject> lost, IVisibleObject key)
        {
            
        }

        internal void OnNoObjectsVisible(List<DataVisionSeenObject> visionObjects)
        {
            
        }

        #endregion
    }
}