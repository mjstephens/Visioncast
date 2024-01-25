using System.Collections.Generic;
using UnityEngine;

namespace Stephens.Sensors
{
    public interface IScheduledRaycastListener
    {
        #region PROPERTIES
        
        List<RaycastHit> RaycasterResults { get; set; }
        List<DataScheduledRaycastRequest> RaycasterRequests { get; set; }

        #endregion PROPERTIES
        
        
        #region METHODS

        void ReceiveScheduledRaycasterResults();
        
        #endregion METHODS
    }
}