using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyGourd.Visioncast
{
    /// <summary>
    /// Manages high-level visioncast system flow.
    /// </summary>
    public static class VisioncastManager
    {
        #region VARIABLES

        public static Action<List<VisioncastSource>> OnSourceComponentsModified;

        private static SystemScheduledRaycaster _scheduledRaycaster;
        private static Visioncaster _visioncaster;

        #endregion VARIABLES
        
        
        #region API

        public static void TickRaycasts(float delta)
        {
            _scheduledRaycaster?.Tick(delta);
        }

        public static void TickVisioncasts(float delta)
        {
            _visioncaster?.Tick(delta);
        }

        /// <summary>
        /// Updates the debug visualization for the visioncast sources
        /// </summary>
        public static void TickVisioncastSourceDebug(float delta)
        {
            foreach (VisioncastSource component in _visioncaster.Components)
            {
                component.TickDebug(delta);
            }
        }

        public static void RegisterVisionSource(VisioncastSource source)
        {
            _visioncaster?.RegisterComponent(source);
        }
        
        public static void UnregisterVisionSource(VisioncastSource source)
        {
            _visioncaster?.RemoveComponent(source);
        }

        #endregion API


        #region UTILITY

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemRegistration()
        {
            _scheduledRaycaster = new SystemScheduledRaycaster();
            OnSourceComponentsModified = null;
            _visioncaster = new Visioncaster(_scheduledRaycaster)
            {
                OnSourceComponentsModified = OnSourceComponentsModified
            };
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnAfterSceneLoad()
        {
            OnSourceComponentsModified?.Invoke(_visioncaster.Components);
        }

        #endregion UTILITY
    }
}