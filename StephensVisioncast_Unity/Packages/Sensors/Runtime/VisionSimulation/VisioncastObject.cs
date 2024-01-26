using Stephens.Tick;
using Stephens.Utility;
using UnityEngine;

namespace Stephens.Sensors
{
    /// <summary>
    /// An object whose collider is exposed to the visioncast system, allowing it to be "seen" by game entities
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class VisioncastObject : MonoBehaviour, IVisibleObject, ITickable
    {
        #region VARIABLES
        
        public TickGroup TickGroup => TickGroup.VisibleObjectBoundsRefresh;
        public virtual Vector3 Position => transform.position;
        public Collider Collider { get; protected set; }
        public bool IsValidForInteraction { get; private set; }
        public Vector3[] VisiblePoints => _visiblePoints;
        public Bounds WorldBounds => Collider.bounds;

        private Vector3[] _visiblePoints;
        private bool _isBox;
        private readonly Vector3[] _bounds = new Vector3[6];

        #endregion VARIABLES


        #region INITIALIZATION

        protected virtual void Awake()
        {
            Collider = GetComponent<Collider>();
            _isBox = Collider is BoxCollider;
            IsValidForInteraction = true;
            
            RecalculateVisiblePoints();
        }

        protected virtual void OnEnable()
        {
            TickRouter.Register(this);
        }
        
        protected virtual void OnDisable()
        {
            TickRouter.Unregister(this);
        }

        protected virtual void OnDestroy()
        {
            IsValidForInteraction = false;
        }

        #endregion INITIALIZATION


        #region VISION

        private void RecalculateVisiblePoints()
        {
            if (_isBox)
            {
                _visiblePoints = ColliderUtility.GetBoxColliderExtentsFaces(Collider as BoxCollider, 0, _bounds);
            }
            else
            {
                _visiblePoints = ColliderUtility.GetColliderBoundsFaces(Collider, 0, _bounds);
            }
        }

        public virtual void Seen()
        {
            
        }

        #endregion VISION


        #region TICK

        void ITickable.Tick(float delta)
        {
            // We only need to recalculate collider bounds if the object has moved at all
            if (transform.hasChanged)
            {
                transform.hasChanged = false;
                RecalculateVisiblePoints();
            }
        }

        #endregion TICK
    }
}