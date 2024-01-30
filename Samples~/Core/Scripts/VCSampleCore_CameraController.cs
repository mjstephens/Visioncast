using System;
using UnityEngine;

namespace VisioncastSamples.Core
{
    public class VCSampleCore_CameraController : MonoBehaviour
    {
        #region VARIABLES

        [Serializable]
        internal struct CameraMode
        {
            public Transform CameraTarget;
            public string Label;
        }
        
        [Header("References")]
        [SerializeField] private CameraMode[] _modes;

        internal CameraMode[] Modes => _modes;
        
        private CameraMode _currentTarget;

        #endregion VARIABLES
        

        #region INITIALIZATION

        private void Awake()
        {
            SetActiveMode(0);
        }

        #endregion INITIALIZATION


        #region UPDATE

        internal void SetActiveMode(int index)
        {
            _currentTarget = _modes[index];
        }
        
        private void Update()
        {
            transform.position = 
                Vector3.Lerp(transform.position, _currentTarget.CameraTarget.position, Time.deltaTime * 20);
            transform.rotation = 
                Quaternion.Slerp(transform.rotation, _currentTarget.CameraTarget.rotation, Time.deltaTime * 20);
        }

        #endregion UPDATE
    }
}