using System;
using Stephens.Sensors;
using UnityEngine;

namespace VisioncastTests.Core
{
    [Serializable]
    internal struct CameraMode
    {
        public GameObject CameraObj;
        public string Label;
    }
    
    public class VCTestCore_Controller : MonoBehaviour
    {
        #region VARIABLES

        [Header("References")] 
        [SerializeField] private GameObject _prefabTestVisionSource;
        [SerializeField] private Transform _visionSourceSpawnerParent;
        [SerializeField] private CameraMode[] _camModes;
        
        private int _currentCameraMode = -1;
        private bool _debugLoSToggle = true;
        
        #endregion VARIABLES
        
        
        #region INITIALIZATION

        private void Awake()
        {
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
            foreach (DebugVisioncastSource db in FindObjectsByType<DebugVisioncastSource>(FindObjectsSortMode.None))
            {
                db.Toggle(_debugLoSToggle);
            }

            return _debugLoSToggle;
        }

        #endregion UTILITY
    }
}

