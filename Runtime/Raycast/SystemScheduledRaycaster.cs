using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace GalaxyGourd.Visioncast
{
    /// <summary>
    /// Collects requests and groups raycasts for efficient processing
    /// </summary>
    internal class SystemScheduledRaycaster : GroupedRequestHandler<IScheduledRaycastListener, DataScheduledRaycastRequest>
    {
        #region VARIABLES
        
        private NativeArray<RaycastCommand> _raycastCommands;
        private NativeArray<RaycastHit> _raycastHits;
        private JobHandle _jobHandle;

        #endregion VARIABLES


        #region SCHEDULE

        protected override void OnScheduled()
        {
            
        }

        #endregion SCHEDULE


        #region EXECUTE
        
        protected override void ExecuteScheduledRequests()
        {
            ExecuteRaycasts();
            ClearCaches();
        }

        protected override void OnNoRequestsTick()
        {
            for (int i = 0; i < _listeners.Count; i++)
            {
                _listeners[i]?.OnEmptyRaycastRequestResult();
            }
        }

        [BurstCompile]
        private void ExecuteRaycasts()
        {
            _raycastCommands = new NativeArray<RaycastCommand>(_requests.Count, Allocator.TempJob);
            _raycastHits = new NativeArray<RaycastHit>(_requests.Count, Allocator.TempJob);
            for (int i = 0; i < _raycastCommands.Length; i++)
            {
                _raycastCommands[i] = new RaycastCommand(
                    _requests[i].SourcePosition,
                    _requests[i].Direction.normalized,
                    new QueryParameters(layerMask:_requests[i].LayerMask),
                    _requests[i].MaxDistance);
            }
            _jobHandle = RaycastCommand.ScheduleBatch(_raycastCommands, _raycastHits, 1);
            
            // Force complete job
            _jobHandle.Complete();
            
            // Clear previous listener results
            for (int i = 0; i < _listeners.Count; i++)
            {
                // Clear listener data
                if (_listeners[i].RaycasterResults == null)
                {
                    _listeners[i].RaycasterResults = new List<RaycastHit>();
                }
                else
                {
                    _listeners[i].RaycasterResults.Clear();
                }

                if (_listeners[i].RaycasterRequests == null)
                {
                    _listeners[i].RaycasterRequests = new List<DataScheduledRaycastRequest>();
                }
                else
                {
                    _listeners[i].RaycasterRequests.Clear();
                }
            }

            // Individual results in contiguous order; remap to listeners
            for (int i = 0; i < _raycastCommands.Length; i++)
            {
                // The index of the listener for this request
                int listenerIndex = GetListenerIndexForRequest(i);
                _listeners[listenerIndex].RaycasterResults.Add(_raycastHits[i]);
                _listeners[listenerIndex].RaycasterRequests.Add(_requests[i]);
            }
            
            // Casts are complete, inform listeners
            for (int i = 0; i < _listeners.Count; i++)
            {
                _listeners[i]?.ReceiveScheduledRaycasterResults();
            }
        }
        
        #endregion EXECUTE


        #region CLEANUP

        private void ClearCaches()
        {
            ClearBaseCaches();
            
            if (_raycastCommands.IsCreated)
            {
                _raycastCommands.Dispose();
            }

            if (_raycastHits.IsCreated)
            {
                _raycastHits.Dispose();
            }
        }

        #endregion CLEANUP
    }
}