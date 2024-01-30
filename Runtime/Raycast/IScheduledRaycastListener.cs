using System.Collections.Generic;
using UnityEngine;

namespace GalaxyGourd.Visioncast
{
    public interface IScheduledRaycastListener
    {
        #region PROPERTIES
        
        List<RaycastHit> RaycasterResults { get; set; }
        List<DataScheduledRaycastRequest> RaycasterRequests { get; set; }

        #endregion PROPERTIES
        
        
        #region METHODS

        void ReceiveScheduledRaycasterResults();
        void OnEmptyRaycastRequestResult();

        #endregion METHODS
    }
}