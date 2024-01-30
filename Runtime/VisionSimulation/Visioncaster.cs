using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyGourd.Visioncast
{
    /// <summary>
    /// Class responsible for executing visioncast requests
    /// </summary>
    internal class Visioncaster : IScheduledRaycastListener
    {
        #region VARIABLES
        
        internal List<VisioncastSource> Components => _components;
        internal Action<List<VisioncastSource>> OnSourceComponentsModified;
        public List<RaycastHit> RaycasterResults { get; set; }
        public List<DataScheduledRaycastRequest> RaycasterRequests { get; set; }
        
        // Aligned buffers
        private static readonly List<List<IVisioncastVisible>> _visibleObjects = new();
        // item1 = index of _visibleObjects object to which points belong
        private static readonly List<List<List<Vector3>>> _visibleObjectPoints = new();
        private static readonly List<List<float>> _visibleObjectDistances = new();
        private static readonly List<List<float>> _visibleObjectAngles = new();
        // We use this to map our raycast requests to the correct source/target
        // 1 = index of component(source), 2 = index of _visibleObjects, 3 = index of _visibleObjectPoints
        private static readonly List<Tuple<int, int, int>> _raycastRequestMap = new();
        // Results for each of the components
        private static readonly List<DataVisioncastResult> _visioncastResults = new();
        // Hitbuffer for each source
        private readonly List<Collider[]> _hitsBuffer = new();
        private readonly SystemScheduledRaycaster _raycaster;
        private bool _waitingForRaycasts;
        private readonly List<VisioncastSource> _components = new();
        private readonly List<VisioncastSource> _queuedAdd = new();
        private readonly List<VisioncastSource> _queuedRemove = new();
        
        private const int CONST_VisionCastColliderBuffer = 64;

        #endregion VARIABLES
        
        
        #region INITIALIZATION

        internal Visioncaster(SystemScheduledRaycaster raycaster)
        {
            _raycaster = raycaster;
            RaycasterRequests = new List<DataScheduledRaycastRequest>();
        }

        #endregion INITIALIZATION


        #region REGISTRATION

        public void RegisterComponent(VisioncastSource component)
        {
            if (_components.Count == 0)
            {
                _components.Add(component);
                _hitsBuffer.Add(new Collider[CONST_VisionCastColliderBuffer]);
            }
            else
            {
                _queuedAdd.Add(component);
            }
        }

        public void RemoveComponent(VisioncastSource component)
        {
            _queuedRemove.Add(component);
        }

        /// <summary>
        /// Syncs the addition and removal of components until after raycasting has been completed
        /// </summary>
        private void ResolveComponentQueues()
        {
            foreach (VisioncastSource source in _queuedAdd)
            {
                _components.Add(source);
                _hitsBuffer.Add(new Collider[CONST_VisionCastColliderBuffer]);
            }
            
            foreach (VisioncastSource source in _queuedRemove)
            {
                _hitsBuffer.RemoveAt(_components.IndexOf(source));
                _components.Remove(source);
            }
            
            _queuedAdd.Clear();
            _queuedRemove.Clear();
            
            OnSourceComponentsModified?.Invoke(_components);
        }

        #endregion REGISTRATION


        #region TICK

        internal void Tick(float delta)
        {
            if (_waitingForRaycasts)
                return;
            
            CalculateVisionBroadphase();
            ExecuteVisionNarrowphase();
            _raycaster.Schedule(this, RaycasterRequests);
        }

        #endregion TICK


        #region LOGIC
        
        /// <summary>
        /// Gathers targets to be raycasted against, eliminating objects that are too far or not in front of the source
        /// </summary>
        private void CalculateVisionBroadphase()
        {
            // Clear buffers for new calculation
            ClearBuffers();

            // Iterate through sources and gather "visible" objects
            for (int i = 0; i < _components.Count; i++)
            {
                VisioncastSource source = _components[i];
                _visibleObjects.Add(new List<IVisioncastVisible>());
                _visibleObjectPoints.Add(new List<List<Vector3>>());
                _visibleObjectDistances.Add(new List<float>());
                _visibleObjectAngles.Add(new List<float>());
                _visioncastResults.Add(new DataVisioncastResult
                {
                    Objects = new List<IVisioncastVisible>(),
                    VisiblePoints = new List<List<Vector3>>(12),
                    Distances = new List<float>(),
                    Angles = new List<float>()
                });
                
                if (Physics.OverlapSphereNonAlloc(
                        source.Position,
                        source.Range,
                        _hitsBuffer[i],
                        source.VisionLayers,
                        QueryTriggerInteraction.UseGlobal) > 0)
                {
                    foreach (Collider hit in _hitsBuffer[i])
                    {
                        if (!hit)
                            break;
                        
                        // We know the object is in range, but we also need to filter by angle
                        Vector3 closestPoint = hit.ClosestPoint(source.Position);
                        Vector3 dirToTarget = closestPoint - source.Position;
                        float dot = Vector3.Dot(source.Heading.normalized, dirToTarget.normalized);
                        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
                        float objDistance = Vector3.Distance(closestPoint, source.Position);
                        
                        if (angle <= source.FieldOfView && hit.TryGetComponent(out IVisioncastVisible visibleObject))
                        {
                            _visibleObjects[i].Add(visibleObject);
                            List<Vector3> objectPoints = new List<Vector3>(7) { closestPoint };
                            objectPoints.AddRange(visibleObject.VisiblePoints);
                            _visibleObjectPoints[i].Add(objectPoints);
                            _visibleObjectDistances[i].Add(objDistance);
                            _visibleObjectAngles[i].Add(angle);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Takes resulting broadphase objects and schedules raycasts against them
        /// </summary>
        private void ExecuteVisionNarrowphase()
        {
            RaycasterRequests.Clear();
            _raycastRequestMap.Clear();
            
            for (int i = 0; i < _components.Count; i++)
            {
                VisioncastSource source = _components[i];
                
                // If there are NO visible objects
                if (_visibleObjects[i].Count == 0)
                {
                    OnNoObjectsVisibleForSource(source);
                    continue;
                }
                
                for (int e = 0; e < _visibleObjects[i].Count; e++)
                {
                    for (int y = 0; y < _visibleObjectPoints[i][e].Count; y++)
                    {
                        Vector3 objectPoint = _visibleObjectPoints[i][e][y];
                        RaycasterRequests.Add(new DataScheduledRaycastRequest
                        {
                            SourcePosition = source.Position,
                            Direction = objectPoint - source.Position,
                            MaxDistance = source.Range,
                            LayerMask = source.ObstructionLayers
                        });
                        
                        // Add matching key map
                        _raycastRequestMap.Add(new Tuple<int, int, int>(i, e, y));
                    }
                }
            }
        }

        void IScheduledRaycastListener.ReceiveScheduledRaycasterResults()
        {
            // Distribute raycast hits
            for (int i = 0; i < RaycasterResults.Count; i++)
            {
                // Map objects
                RaycastHit hit = RaycasterResults[i];
                Tuple<int, int, int> map = _raycastRequestMap[i];
                
                DataVisioncastResult result = _visioncastResults[map.Item1];
                IVisioncastVisible thisObject = _visibleObjects[map.Item1][map.Item2];
                Vector3 thisPoint = _visibleObjectPoints[map.Item1][map.Item2][map.Item3];

                // Add object to source results if not present
                if (!result.Objects.Contains(thisObject))
                {
                    result.Objects.Add(thisObject);
                    result.Angles.Add(_visibleObjectAngles[map.Item1][map.Item2]);
                    result.Distances.Add(_visibleObjectDistances[map.Item1][map.Item2]);
                    result.VisiblePoints.Add(new List<Vector3>(7));
                }

                // Did we hit the object itself?
                if (hit.collider && hit.collider == thisObject.Collider)
                {
                    result.VisiblePoints[result.Objects.IndexOf(thisObject)].Add(thisPoint);
                }
            }

            // Distribute results
            for (int i = 0; i < _components.Count; i++)
            {
                _components[i].ReceiveResults(_visioncastResults[i]);
            }
            
            // Now that we've recieved raycasts, we can restart visioncast
            _waitingForRaycasts = false;
            ResolveComponentQueues();
        }

        void IScheduledRaycastListener.OnEmptyRaycastRequestResult()
        {
            ResolveComponentQueues();
        }

        private static void OnNoObjectsVisibleForSource(VisioncastSource source)
        {
            source.ReceiveResults(new DataVisioncastResult());
        }

        #endregion LOGIC


        #region UTILITY

        private void ClearBuffers()
        {
            _visibleObjects.Clear();
            _visibleObjectPoints.Clear();
            _visibleObjectDistances.Clear();
            _visibleObjectAngles.Clear();
            _visioncastResults.Clear();
            
            // Hits buffers
            for (int i = 0; i < _hitsBuffer.Count; i++)
            {
                for (int e = 0; e < _hitsBuffer[i].Length; e++)
                {
                    _hitsBuffer[i][e] = null;
                } 
            }
        }

        #endregion UTILITY


        #region RESET

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemRegistration()
        {
            _visibleObjects.Clear();
            _visibleObjectPoints.Clear();
            _visibleObjectDistances.Clear();
            _visibleObjectAngles.Clear();
            _raycastRequestMap.Clear();
            _visioncastResults.Clear();
        }

        #endregion RESET
    }
}