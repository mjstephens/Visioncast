using Stephens.Input;
using Stephens.KCC;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Stephens.Player
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInstance : MonoBehaviour, IKCCOperator
    {
        #region VARIABLES
        
        [Header("References")]
        [SerializeField] private PlayerCamera _playerCamera;
        [SerializeField] private GameObject _prefabKCC;
        [SerializeField] protected GameObject _prefabVisionSource;
        
        // Camera properties
        public VirtualCameraKCC ActivePlayerCamera => _playerCamera.ActiveVCam;
        public DataInputValuesKCCCamera CameraInputData
        {
            get => _playerCamera.DataInputCamera;
            set => _playerCamera.DataInputCamera = value;
        }
        public KCCCameraViewState ViewState => _playerCamera.CameraViewState;

        private PlayerInput _playerInput;
        private ControllerKCC _kccController;
        private ActionMapListenerKCC _actionMapListenerKCC;
        private DefaultKCCAnimator _kccAnimator;

        #endregion VARIABLES


        #region INITIALIZATION

        private void Awake()
        {
            // Initialize KCC input listener, subscribe to receive input
            _playerInput = GetComponent<PlayerInput>();
            _actionMapListenerKCC = GetComponent<ActionMapListenerKCC>();
            _actionMapListenerKCC.Init(_playerInput);
            _actionMapListenerKCC.SetReceivers(this);

            // If we don't have a kcc prefab (maybe we're testing), bail now
            if (!_prefabKCC)
                return;
            
            // Spawn KCC prefab
            _kccController = Instantiate(_prefabKCC).GetComponent<ControllerKCC>();
            _kccController.Motor.SetPositionAndRotation(transform.position, transform.rotation);
            _kccController.Operator = this;
            
            // Tell the camera about our KCC
            _playerCamera.AssignKCC(this, _kccController);
            
            // Init KCC animator
            _kccAnimator = _kccController.transform.GetComponentInChildren<DefaultKCCAnimator>();
            _kccAnimator.Init(_kccController.AnimatorProvider);
        }

        #endregion INITIALIZATION


        #region INPUT

        void IInputReceiver<DataInputValuesControllerKCC>.ReceiveInput(DataInputValuesControllerKCC inputData, float delta)
        {
            // Forward input to our KCC controller
            (_kccController as IInputReceiver<DataInputValuesControllerKCC>).ReceiveInput(inputData, delta);
        }

        void IInputReceiver<DataInputValuesKCCCamera>.ReceiveInput(DataInputValuesKCCCamera inputData, float delta)
        {
            // Forward input to our camera class
            _playerCamera.ReceiveCameraInput(inputData, delta);
        }

        #endregion INPUT
    }
}