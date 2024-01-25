using System;
using System.Collections.Generic;
using Stephens.Tick;
using Stephens.Utility;
using UnityEngine;

namespace Stephens.Sensors
{
    /// <summary>
    /// Class responsible for executing visioncast requests
    /// </summary>
    internal class SystemVisioncaster : ComponentSystem<IVisionSource>, ITickable, IScheduledRaycastListener
    {
        #region VARIABLES

        public TickGroup TickGroup => TickGroup.VisionCaster; // 0.1 second intervals
        
        // Scheduled raycaster items
        public List<RaycastHit> RaycasterResults { get; set; }
        public List<DataScheduledRaycastRequest> RaycasterRequests { get; set; }

        // Aligned buffers
        private static readonly List<List<IVisibleObject>> _visibleObjects = new();
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
        private static LayerMask _obstructionCheckLayerMask;
        private bool _waitingForRaycasts;
        private readonly List<IVisionSource> _queuedAdd = new();
        private readonly List<IVisionSource> _queuedRemove = new();
        
        private const int CONST_VisionCastColliderBuffer = 64;

        #endregion VARIABLES
        
        
        #region INITIALIZATION

        internal SystemVisioncaster(SystemScheduledRaycaster raycaster, LayerMask layermaskObstruction)
        {
            _raycaster = raycaster;
            _obstructionCheckLayerMask = layermaskObstruction;
            RaycasterRequests = new List<DataScheduledRaycastRequest>();
            
        }

        internal void Register()
        {
            TickRouter.Register(this);
        }

        #endregion INITIALIZATION


        #region REGISTRATION

        public override void RegisterComponent(IVisionSource component)
        {
            if (_components.Count == 0)
            {
                base.RegisterComponent(component);
                _hitsBuffer.Add(new Collider[CONST_VisionCastColliderBuffer]);
            }
            else
            {
                _queuedAdd.Add(component);
            }
        }

        public override void RemoveComponent(IVisionSource component)
        {
            _queuedRemove.Add(component);
        }

        /// <summary>
        /// Syncs the addition and removal of components until after raycasting has been completed
        /// </summary>
        private void ResolveComponentQueues()
        {
            foreach (IVisionSource source in _queuedAdd)
            {
                _components.Add(source);
                _hitsBuffer.Add(new Collider[CONST_VisionCastColliderBuffer]);
            }
            
            foreach (IVisionSource source in _queuedRemove)
            {
                _hitsBuffer.RemoveAt(_components.IndexOf(source));
                _components.Remove(source);
            }
            
            _queuedAdd.Clear();
            _queuedRemove.Clear();
        }

        #endregion REGISTRATION


        #region TICK

        void ITickable.Tick(float delta)
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
                IVisionSource source = _components[i];
                _visibleObjects.Add(new List<IVisibleObject>());
                _visibleObjectPoints.Add(new List<List<Vector3>>());
                _visibleObjectDistances.Add(new List<float>());
                _visibleObjectAngles.Add(new List<float>());
                _visioncastResults.Add(new DataVisioncastResult()
                {
                    Objects = new List<IVisibleObject>(),
                    VisiblePoints = new List<List<Vector3>>(12),
                    Distances = new List<float>(),
                    Angles = new List<float>()
                });
                
                if (Physics.OverlapSphereNonAlloc(
                        source.Position,
                        source.Range,
                        _hitsBuffer[i],
                        source.VisionLayer,
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
                        float normalizedDistance = 1 - (objDistance / source.Range);
                        float targFoV = Mathf.Lerp(
                            source.FieldOfViewRange.x, 
                            source.FieldOfViewRange.y, 
                            normalizedDistance);
                        
                        if (angle <= targFoV && hit.TryGetComponent(out IVisibleObject visibleObject))
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
                IVisionSource source = _components[i];
                
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
                        RaycasterRequests.Add(new DataScheduledRaycastRequest()
                        {
                            SourcePosition = source.Position,
                            Direction = objectPoint - source.Position,
                            MaxDistance = source.Range,
                            LayerMask = _obstructionCheckLayerMask
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
                IVisibleObject thisObject = _visibleObjects[map.Item1][map.Item2];
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

        private static void OnNoObjectsVisibleForSource(IVisionSource source)
        {
            source.ReceiveResults(null);
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