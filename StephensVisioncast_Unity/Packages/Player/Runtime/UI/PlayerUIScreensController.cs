using Stephens.Camera;
using UnityEngine;

namespace Stephens.Player
{
    public class PlayerUIScreensController : MonoBehaviour
    {
        #region VARIABLES

        [Header("References")]
        [SerializeField] private UICamera _uiCamera;

        private IPlayerUI[] _uiScreens;

        #endregion VARIABLES


        #region INITIALIZATION

        private void Awake()
        {
            // Set UI camera for all screens
            _uiScreens = GetComponentsInChildren<IPlayerUI>();
            foreach (IPlayerUI screen in _uiScreens)
            {
                screen.Init(_uiCamera);
            }
        }

        #endregion INITIALIZATION
    }
}