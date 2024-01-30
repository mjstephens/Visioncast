using System;
using System.Collections.Generic;
using GalaxyGourd.Visioncast;
using UnityEngine;

namespace VisioncastSamples.Core
{
    [Serializable]
    internal struct CameraMode
    {
        public GameObject CameraObj;
        public string Label;
    }
    
    public class VCSampleCore_Controller : MonoBehaviour
    {
        #region VARIABLES

        [Header("References")] 
        [SerializeField] private VCSampleCore_UIController _uiController;
        [SerializeField] private GameObject _prefabTestVisionSource;
        [SerializeField] private Transform _visionSourceSpawnerParent;
        [SerializeField] private CameraMode[] _camModes;
        
        internal List<VisioncastSource> Sources { get; private set; }
        
        private int _currentCameraMode = -1;
        private bool _debugLoSToggle = true;
        private bool _debugConesToggle = true;
        
        #endregion VARIABLES
        
        
        #region INITIALIZATION

        private void Awake()
        {
            VisioncastManager.OnSourceComponentsModified += OnVisionSourcesChanged;
            
            for (int i = 0; i < _visionSourceSpawnerParent.childCount; i++)
            {
                if (!_visionSourceSpawnerParent.GetChild(i).gameObject.activeInHierarchy)
                    continue;
                
                Transform t = Instantiate(_prefabTestVisionSource, _visionSourceSpawnerParent.GetChild(i)).transform;
                t.localPosition = Vector3.zero;                
                t.localEulerAngles = Vector3.zero;                
            }

            AdvanceCameraMode(1);
        }
        
        #endregion INITIALIZATION


        #region UTILITY

        internal string AdvanceCameraMode(int advance)
        {
            // Disable all
            foreach (CameraMode mode in _camModes)
            {
                mode.CameraObj.SetActive(false);
            }
            
            _currentCameraMode += advance;
            if (_currentCameraMode < 0)
                _currentCameraMode = _camModes.Length - 1;
            else if (_currentCameraMode >= _camModes.Length)
                _currentCameraMode = 0;
            
            _camModes[_currentCameraMode].CameraObj.SetActive(true);
            return _camModes[_currentCameraMode].Label;
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

