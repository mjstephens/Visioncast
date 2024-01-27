using Unity.Cinemachine;
using Stephens.Tick;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Stephens.Camera
{
    /// <summary>
    /// Base class for all Cinemachine brain cameras.
    /// </summary>
    [RequireComponent(typeof(CinemachineBrain))]
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class GameCamera : MonoBehaviour
    {
        #region VARIABLES

        [Header("References")]
        [SerializeField] private UnityEngine.Camera _camera;
        [SerializeField] private UICamera _uiCamera;
        [SerializeField] protected CinemachineBrain _brain;
        [SerializeField] private EventSystem _eventSystem;

        public TickGroup TickGroup => TickGroup.UIUpdate;
        public UnityEngine.Camera Camera => _camera;
        public UnityEngine.Camera UICamera => _uiCamera.Camera;
        public EventSystem UIInputModule => _eventSystem;
        public CinemachineBrain Brain => _brain;

        #endregion VARIABLES


        #region UTILITY

        internal void SetCameraRect(Rect rect)
        {
            Camera.rect = rect;
            _uiCamera.Camera.rect = rect;
        }

        #endregion UTILITY
    }
}