using System.Collections.Generic;
using GalaxyGourd.Tick;
using Stephens.Input;
using UnityEngine;

namespace Stephens.Camera
{
    public class UICamera : TickableMonoBehaviour
    {
        #region VARIABLES

        [Header("References")]
        [SerializeField] private UnityEngine.Camera _camera;
        [SerializeField] private Canvas _uiCanvas;
        [SerializeField] private UISimulatedPointer _simulatedPointer;

        public override int TickGroup => (int)TickGroups.UIUpdate;
        public UnityEngine.Camera Camera => _camera;
        public Canvas UICanvas => _uiCanvas;
        public UISimulatedPointer SimulatedPointer => _simulatedPointer;
        
        private ScreenAspect _aspect;
        private float _aspectRaw;
        private readonly List<IScreenAspectListener> _aspectListeners = new();
        
        #endregion VARIABLES


        #region TICK

        public override void Tick(float delta)
        {
            //
            _simulatedPointer.UpdatePointer(delta);
            
            //
            float aspect = (float)Screen.width / (float)Screen.height;
            if (Mathf.Abs(_aspectRaw - aspect) > Mathf.Epsilon)
            {
                _aspectRaw = aspect;
                _aspect = ResolveAspect();
                foreach (IScreenAspectListener listener in _aspectListeners)
                {
                    listener.OnScreenAspectChange(_aspect);
                }
            }
        }

        #endregion TICK


        #region ASPECT

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
    }
}