using UnityEngine;

namespace GalaxyGourd.Visioncast
{
    /// <summary>
    /// Base class for vision source objects - inherit to add contextual functionality
    /// </summary>
    public abstract class VisioncastSource : MonoBehaviour
    {
        #region VARIABLES

        /// <summary>
        /// Layers that this source can "see"
        /// </summary>
        public abstract LayerMask VisionLayers { get; }
        /// <summary>
        /// Layers of objects that will block this source's line of sight
        /// </summary>
        public abstract LayerMask ObstructionLayers { get; }
        public virtual Vector3 Position => transform.position;
        public virtual Vector3 Heading => transform.forward;
        public abstract float Range { get; }
        public abstract float FieldOfView { get; }
        public DataVisioncastResult LastResults { get; private set; }
        
        private VisioncastSourceDebug _debug;
        
        #endregion VARIABLES


        #region INITIALIZATION

        private void OnEnable()
        {
            VisioncastManager.RegisterVisionSource(this);
        }

        private void OnDisable()
        {
            VisioncastManager.UnregisterVisionSource(this);
        }
        
        #endregion INITIALIZATION


        #region CAST

        internal void ReceiveResults(DataVisioncastResult data)
        {
            LastResults = data;
            OnReceiveResults(data);
        }

        protected abstract void OnReceiveResults(DataVisioncastResult data);

        #endregion CAST


        #region DEBUG

        internal void TickDebug(float delta)
        {
            if (!_debug)
                return;
            
            _debug.Tick(delta);
        }

        internal void AttachDebug(VisioncastSourceDebug debug)
        {
            _debug = debug;
        }
        
        internal void DetachDebug(VisioncastSourceDebug debug)
        {
            _debug = null;
        }

        #endregion DEBUG
    }
}