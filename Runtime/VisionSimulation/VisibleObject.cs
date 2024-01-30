using UnityEngine;

namespace GalaxyGourd.Visioncast
{
    /// <summary>
    /// An object whose collider is exposed to the visioncast system, allowing it to be "seen" by game entities
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class VisibleObject : MonoBehaviour, IVisioncastVisible
    {
        #region VARIABLES
        
        public virtual Vector3 Position => transform.position;
        public Collider Collider { get; protected set; }
        public Vector3[] VisiblePoints { get; private set; }

        private bool _isBox;
        private readonly Vector3[] _bounds = new Vector3[6];

        #endregion VARIABLES


        #region INITIALIZATION

        protected virtual void Awake()
        {
            Collider = GetComponent<Collider>();
            _isBox = Collider is BoxCollider;
            
            RecalculateVisiblePoints();
        }

        #endregion INITIALIZATION


        #region VISION
        
        private void FixedUpdate()
        {
            // We only need to recalculate collider bounds if the object has moved at all
            if (transform.hasChanged)
            {
                transform.hasChanged = false;
                RecalculateVisiblePoints();
            }
        }

        protected virtual void RecalculateVisiblePoints()
        {
            if (_isBox)
            {
                VisiblePoints = VisioncastUtility.GetBoxColliderExtentsFaces(Collider as BoxCollider, 0, _bounds);
            }
            else
            {
                VisiblePoints = VisioncastUtility.GetColliderBoundsFaces(Collider, 0, _bounds);
            }
        }

        public virtual void Seen()
        {
            
        }

        #endregion VISION
    }
}