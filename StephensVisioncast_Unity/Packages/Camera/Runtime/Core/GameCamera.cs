using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using Stephens.Tick;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Stephens.Camera
{
    /// <summary>
    /// Base class for all Cinemachine brain cameras.
    /// </summary>
    [RequireComponent(typeof(CinemachineBrain))]
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class GameCamera : MonoBehaviour, IGameCamera, ITickable
    {
        #region VARIABLES

        [Header("References")]
        [SerializeField] private UnityEngine.Camera _camera;
        [SerializeField] protected CinemachineBrain _brain;
        
        [Header("UI")]
        [SerializeField] private Canvas _uiCanvas;
        [SerializeField] protected GameObject _pointerObj;

        [Header("Splitscreen References")]
        [SerializeField] private UnityEngine.Camera _uiCamera;
        [SerializeField] private Transform _splitscreenCursorUIParent;
        [SerializeField] private EventSystem _eventSystem;
        [SerializeField] private GraphicRaycaster _raycaster;

        public TickGroup TickGroup => TickGroup.UIUpdate;
        public UnityEngine.Camera Camera => _camera;
        public Canvas UICanvas => _uiCanvas;
        public UnityEngine.Camera UICamera => _uiCamera;
        public Transform CursorParent => _splitscreenCursorUIParent;
        public EventSystem UIInputModule => _eventSystem;
        public GraphicRaycaster Raycaster => _raycaster;
        public GameObject PointerObj => _pointerObj;
        public CinemachineBrain Brain => _brain;
        
        private ScreenAspect _aspect;
        private float _aspectRaw;
        private readonly List<IScreenAspectListener> _aspectListeners = new();

        #endregion VARIABLES


        #region INITIALIZATION

        private void OnEnable()
        {
            TickRouter.Register(this);
        }

        private void OnDisable()
        {
            TickRouter.Unregister(this);
        }

        #endregion INITIALIZATION
        

        #region ASPECT

        void ITickable.Tick(float delta)
        {
            float aspect = (float)Screen.width / (float)Screen.height;
            if (Math.Abs(_aspectRaw - aspect) > Mathf.Epsilon)
            {
                _aspectRaw = aspect;
                _aspect = ResolveAspect();
                foreach (IScreenAspectListener listener in _aspectListeners)
                {
                    listener.OnScreenAspectChange(_aspect);
                }
            }
        }

        private ScreenAspect ResolveAspect()
        {
            return _aspectRaw switch
            {
                < 1.51f => ScreenAspect.A3_2,
                < 1.61f => ScreenAspect.A16_10,
                < 1.8f => ScreenAspect.A16_9,
                _ => ScreenAspect.AUltrawide
            };
        }
        
        public void RegisterAspectListener(IScreenAspectListener listener)
        {
            _aspectListeners.Add(listener);
            listener.OnScreenAspectChange(_aspect);
        }

        public void UnregisterScreenAspectListener(IScreenAspectListener listener)
        {
            _aspectListeners.Remove(listener);
        }

        #endregion ASPECT


        #region UTILITY

        internal void SetCameraRect(Rect rect)
        {
            Camera.rect = rect;
            _uiCamera.rect = rect;
        }

        #endregion UTILITY
    }
}