using System.Collections.Generic;
using UnityEngine;

namespace Stephens.Sensors
{
    public static class SensorsManager
    {
        #region VARIABLES

        private static SystemScheduledRaycaster _scheduledRaycaster;
        private static SystemVisioncaster _visioncaster;

        #endregion VARIABLES
        
        
        #region API

        /// <summary>
        /// Schedules one or more raycasts to be performed in a job, after which the results will be returned to the requester.
        /// Note that there is no guarantee the results will be ready in the same frame; requesters should ensure they are built
        /// to handle such conditions.
        /// </summary>
        public static void ScheduleRaycasts(IScheduledRaycastListener listener, List<DataScheduledRaycastRequest> requests)
        {
            _scheduledRaycaster.Schedule(listener, requests);
        }

        public static void RegisterVisionSource(IVisionSource source)
        {
            _visioncaster.RegisterComponent(source);
        }
        
        public static void UnregisterVisionSource(IVisionSource source)
        {
            _visioncaster.RemoveComponent(source);
        }

        #endregion API


        #region UTILITY

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemRegistration()
        {
            _scheduledRaycaster = new SystemScheduledRaycaster();
            _visioncaster = new SystemVisioncaster(_scheduledRaycaster, LayerMask.GetMask(
                "Default",
                "Visible",
                "Interactable"));
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnAfterSceneLoad()
        {
            _scheduledRaycaster.Register();
            _visioncaster.Register();
        }

        #endregion UTILITY
    }
}