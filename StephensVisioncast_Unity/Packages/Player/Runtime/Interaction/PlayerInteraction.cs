using System.Collections.Generic;
using Stephens.Sensors;
using UnityEngine;

namespace Stephens.Player
{
    public class PlayerInteraction : MonoBehaviour
    {
        #region VARIABLES

        private PlayerCamera _camera;
        private IVisibleObject _key; // The key is defined as the "active" item, or the item most directly in front of the player
        private UIVisibleObjectOverlays _interactablesOverlay;

        #endregion VARIABLES


        #region INITIALIZATION

        internal void Init(PlayerCamera playerCam)
        {
            _camera = playerCam;

            _interactablesOverlay = _camera.GetComponentInChildren<UIVisibleObjectOverlays>();
            _interactablesOverlay.Init(_camera.GameCamera.UICamera);
        }
        
        #endregion INITIALIZATION


        #region VISION

        internal void OnVisionObjectsGathered(List<IVisibleObject> seen, List<IVisibleObject> lost, IVisibleObject key)
        {
            // Hide/show objects
            foreach (IVisibleObject l in lost)
                _interactablesOverlay.DoObjectJustLostVisible(l);
            foreach (IVisibleObject s in seen)
                _interactablesOverlay.DoObjectJustBecameVisible(s);
            
            // Highlight key object 
            if (key != null)
            {
                _interactablesOverlay.SetKeyVisibleObject(key);
            }
            
            // If this key is new
            if (key != null && key != _key)
            {
                // Tell the manager we have a new key
                _interactablesOverlay.KeyObjectIsNew();
            }
            
            if (key == null && _key != null)
            {
                _interactablesOverlay.KeyObjectHasLost();
            }
            
            _key = key;
        }

        internal void OnNoObjectsVisible(List<DataVisionSeenObject> visionObjects)
        {
            for (int i = visionObjects.Count - 1; i >= 0; i--)
            {
                _interactablesOverlay.DoObjectJustLostVisible(visionObjects[i].ResultObject);
            }        
        }

        #endregion
    }
}