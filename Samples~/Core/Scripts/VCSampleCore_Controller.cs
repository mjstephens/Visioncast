using System.Collections.Generic;
using GalaxyGourd.Visioncast;
using UnityEngine;

namespace VisioncastSamples.Core
{
    public class VCSampleCore_Controller : MonoBehaviour
    {
        #region VARIABLES

        [Header("References")] 
        [SerializeField] private VCSampleCore_UIController _uiController;
        [SerializeField] private VCSampleCore_CameraController _cameraController;
        
        internal List<VisioncastSource> Sources { get; private set; }
        
        private int _currentCameraMode = -1;
        private bool _debugLoSToggle = false;
        private bool _debugConesToggle = true;
        
        #endregion VARIABLES
        
        
        #region INITIALIZATION

        private void Awake()
        {
            VisioncastManager.OnSourceComponentsModified += OnVisionSourcesChanged;

            AdvanceCameraMode(1);
        }
        
        #endregion INITIALIZATION


        #region UTILITY

        internal string AdvanceCameraMode(int advance)
        {
            _currentCameraMode += advance;
            if (_currentCameraMode < 0)
                _currentCameraMode = _cameraController.Modes.Length - 1;
            else if (_currentCameraMode >= _cameraController.Modes.Length)
                _currentCameraMode = 0;
            
            _cameraController.SetActiveMode(_currentCameraMode);
            return _cameraController.Modes[_currentCameraMode].Label;
        }

        internal bool ToggleLoSDebugLines()
        {
            // Find all debug scripts, toggle lines
            _debugLoSToggle = !_debugLoSToggle;
            foreach (VisioncastSourceDebug db in FindObjectsByType<VisioncastSourceDebug>(FindObjectsSortMode.None))
            {
                db.Toggle(_debugLoSToggle);
            }

            return _debugLoSToggle;
        }

        internal bool ToggleSightDebugCones()
        {
            // Find all debug scripts, toggle lines
            _debugConesToggle = !_debugConesToggle;
            foreach (VisionCone cone in FindObjectsByType<VisionCone>(FindObjectsSortMode.None))
            {
                cone.Toggle(_debugConesToggle);
            }

            return _debugConesToggle;
        }
        
        private void OnVisionSourcesChanged(List<VisioncastSource> sources)
        {
            Sources = sources;
            _uiController.OnVisionSourcesModified();
        }

        #endregion UTILITY
    }
}

