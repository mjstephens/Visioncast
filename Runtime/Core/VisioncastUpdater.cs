using UnityEngine;

namespace GalaxyGourd.Visioncast
{
    /// <summary>
    /// Default updater for visioncast system. Replace with custom logic to tick at different intervals.
    /// </summary>
    public class VisioncastUpdater : MonoBehaviour
    {
        #region VARIABLES
        
        private enum UpdateRate
        {
            Update,
            FixedUpdate,
            LateUpdate
        }

        [SerializeField] private UpdateRate _updateRate;
        [SerializeField] private UpdateRate _debugUpdateRate;

        #endregion VARIABLES


        #region UPDATE

        private void Update()
        {
            if (_updateRate == UpdateRate.Update)
                TickSystems(Time.deltaTime);
            if (_debugUpdateRate == UpdateRate.Update)
                VisioncastManager.TickVisioncastSourceDebug(Time.deltaTime);
        }
        
        private void FixedUpdate()
        {
            if (_updateRate == UpdateRate.FixedUpdate)
                TickSystems(Time.fixedDeltaTime);
            if (_debugUpdateRate == UpdateRate.FixedUpdate)
                VisioncastManager.TickVisioncastSourceDebug(Time.fixedDeltaTime);
        }
        
        private void LateUpdate()
        {
            if (_updateRate == UpdateRate.LateUpdate)
                TickSystems(Time.deltaTime);
            if (_debugUpdateRate == UpdateRate.LateUpdate)
                VisioncastManager.TickVisioncastSourceDebug(Time.deltaTime);
        }

        private static void TickSystems(float delta)
        {
            VisioncastManager.TickRaycasts(delta);
            VisioncastManager.TickVisioncasts(delta);
        }

        #endregion UPDATE
    }
}