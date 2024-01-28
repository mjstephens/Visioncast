using System.Collections.Generic;
using GalaxyGourd.Tick;
using Stephens.Camera;
using Stephens.Sensors;
using UnityEngine;

namespace Stephens.Player
{
    public class UIVisibleObjectOverlays : TickableMonoBehaviour, IPlayerUI
    {
        #region VARIABLES

        [Header("References")] 
        [SerializeField] private GameObject _prefabVisibleObjMarkerInstance;
        [SerializeField] private RectTransform _markerParent;
        [SerializeField] private CanvasGroup _group;
        
        public override int TickGroup => (int)TickGroups.UIUpdate;
        public UICamera UICamera { get; set; }

        private readonly List<UIVisibleObjectMarkerInstance> _markerPool = new();
        private readonly Dictionary<IVisibleObject, UIVisibleObjectMarkerInstance> _visibleObjects = new();
        private UnityEngine.Camera _uiCamera;
        private IVisibleObject _key;

        #endregion VARIABLES


        #region INITIALIZATION

        internal void Init(UnityEngine.Camera uiCamera)
        {
            _uiCamera = uiCamera;
        }

        #endregion INITIALIZATION
        
        
        #region TICK
        
        public override void Tick(float delta)
        {
            foreach (KeyValuePair<IVisibleObject, UIVisibleObjectMarkerInstance> obj in _visibleObjects)
            {
                SetMarkerPosition(obj.Value.RectTransform, obj.Key.Position);
            }
        }

        private void SetMarkerPosition(RectTransform marker, Vector3 worldPos)
        {
            Vector3 pos = _uiCamera.WorldToScreenPoint(worldPos);
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                _markerParent, 
                pos, 
                _uiCamera, 
                out Vector3 pp);

            marker.position = pp;
        }

        #endregion TICK


        #region VISION

        internal void DoObjectJustBecameVisible(IVisibleObject obj)
        {
            UIVisibleObjectMarkerInstance overlay = GetNextMarkerInstance();
            overlay.gameObject.SetActive(true);
            _visibleObjects.Add(obj, overlay);
            SetMarkerPosition(overlay.RectTransform, obj.Position);
        }

        internal void DoObjectJustLostVisible(IVisibleObject obj)
        {
            if (!_visibleObjects.ContainsKey(obj))
                return;
            
            _visibleObjects[obj].gameObject.SetActive(false);
            _visibleObjects.Remove(obj);
        }

        internal void SetKeyVisibleObject(IVisibleObject obj)
        {
            foreach (KeyValuePair<IVisibleObject, UIVisibleObjectMarkerInstance> v in _visibleObjects)
            {
                bool key = v.Key == obj;
                v.Value.SetAsKey(key);
                
                if (key)
                {
                    _key = v.Key;
                }
            }
        }

        /// <summary> 
        /// Called first frame when a new object becomes the key
        /// </summary>
        internal void KeyObjectIsNew()
        {
            
        }

        /// <summary>
        /// Called first frame after a key object exists but is then lost (not replaced by anything)
        /// </summary>
        internal void KeyObjectHasLost()
        {
            _key = null;
        }
        
        internal void SetObjectInteractionProgress(IVisibleObject obj, float progress)
        {
            if (!_visibleObjects.ContainsKey(obj))
                return;
            
            _visibleObjects[obj].SetInteractionDurationProgress(progress);
        }

        internal void ClearAllVisibleObjects()
        {
            foreach (KeyValuePair<IVisibleObject, UIVisibleObjectMarkerInstance> obj in _visibleObjects)
            {
                obj.Value.gameObject.SetActive(false);
            }
            _visibleObjects.Clear();

            KeyObjectHasLost();
        }

        internal void SetVisibility(bool visible)
        {
            _group.alpha = visible ? 1 : 0;
        }

        #endregion VISION


        #region UTILITY

        private UIVisibleObjectMarkerInstance GetNextMarkerInstance()
        {
            UIVisibleObjectMarkerInstance next = null;
            foreach (UIVisibleObjectMarkerInstance instance in _markerPool)
            {
                if (!instance.gameObject.activeSelf)
                {
                    next = instance;
                    break;
                }
            }
            
            //
            if (!next)
            {
                next = Instantiate(_prefabVisibleObjMarkerInstance, _markerParent).GetComponent<UIVisibleObjectMarkerInstance>();
                _markerPool.Add(next);
            }

            return next;
        }

        #endregion UTILITY
    }
}