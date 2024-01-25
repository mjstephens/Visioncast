using UnityEngine;

namespace Stephens.Sensors
{
    /// <summary>
    /// Base class for vision source objects - inherit to add contextual functionality
    /// </summary>
    public abstract class VisioncastSource : MonoBehaviour, IVisionSource
    {
        #region VARIABLES

        public abstract LayerMask VisionLayer { get; }
        public Vector3 Position => transform.position;
        public Vector3 Heading => transform.forward;
        public abstract float Range { get; }
        public abstract Vector2 FieldOfViewRange { get; }
        public DataVisioncastResult LastResults { get; protected set; }
        
        #endregion VARIABLES


        #region INITIALIZATION

        private void OnEnable()
        {
            SensorsManager.RegisterVisionSource(this);
        }

        private void OnDisable()
        {
            SensorsManager.UnregisterVisionSource(this);
        }

        #endregion INITIALIZATION


        #region CAST

        void IVisionSource.ReceiveResults(DataVisioncastResult data)
        {
            LastResults = data;
            OnReceiveResults(data);
        }

        protected virtual void OnReceiveResults(DataVisioncastResult data)
        {
            
        }

        #endregion CAST
    }
}