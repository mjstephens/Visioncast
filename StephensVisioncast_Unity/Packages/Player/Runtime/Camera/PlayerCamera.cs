using Stephens.Camera;
using Stephens.KCC;
using Unity.Cinemachine;
using UnityEngine;

namespace Stephens.Player
{
    public class PlayerCamera : MonoBehaviour
    {
        #region VARIABLES

        [Header("References")] 
        [SerializeField] private GameCamera _gameCamera;
        [SerializeField] private VirtualCameraKCCFP _vCamFirstPerson;
        [SerializeField] private VirtualCameraKCCTP _vCamThirdPerson;

        internal VirtualCameraKCC ActiveVCam => _activeCamera;
        internal DataInputValuesKCCCamera DataInputCamera { get; set; }
        internal KCCCameraViewState CameraViewState { get; private set; }
        
        private VirtualCameraKCC _activeCamera;

        #endregion VARIABLES


        #region INITIALIZATION

        private void Awake()
        {
            // Default to first person
            SetActiveCamera(KCCCameraViewState.FirstPerson);
        }

        internal void AssignKCC(IKCCOperator player, ControllerKCC controller)
        {
            _vCamFirstPerson.Init(player, controller);
            _vCamThirdPerson.Init(player, controller);
            
            if (SplitscreenCameraManager.Instance)
            {
                SetCameraChannelsForPlayer();
            }
        }

        #endregion INITIALIZATION


        #region INPUT

        internal void ReceiveCameraInput(DataInputValuesKCCCamera inputData, float delta)
        {
            // Handle camera toggling
            if (inputData.ViewToggled.Started)
            {
                SetActiveCamera(ToggleViewState());
            }
        }

        #endregion
        
        
        #region CAMERAS
        
        private KCCCameraViewState ToggleViewState()
        {
            KCCCameraViewState state = CameraViewState switch
            {
                KCCCameraViewState.ThirdPerson => KCCCameraViewState.FirstPerson,
                KCCCameraViewState.FirstPerson => KCCCameraViewState.ThirdPerson,
                _ => CameraViewState
            };
            
            return state;
        }

        private void SetActiveCamera(KCCCameraViewState state)
        {
            _vCamFirstPerson.VCam.Priority = state == KCCCameraViewState.ThirdPerson
                ? new PrioritySettings() { Enabled = false, Value = 0 }
                : new PrioritySettings() { Enabled = true, Value = 1 };
            _vCamThirdPerson.VCam.Priority = state == KCCCameraViewState.ThirdPerson
                ? new PrioritySettings() { Enabled = true, Value = 1 }
                : new PrioritySettings() { Enabled = false, Value = 0 };
            
            _activeCamera = state == KCCCameraViewState.FirstPerson ? _vCamFirstPerson : _vCamThirdPerson;
            CameraViewState = state;
        }

        private void SetCameraChannelsForPlayer()
        {
            int index = SplitscreenCameraManager.Instance.RegisterCamera(_gameCamera);
            OutputChannels playerChannel = index switch
            {
                0 => OutputChannels.Channel01,
                1 => OutputChannels.Channel02,
                2 => OutputChannels.Channel03,
                3 => OutputChannels.Channel04,
                _ => OutputChannels.Default
            };

            _gameCamera.Brain.ChannelMask = playerChannel;
            _vCamFirstPerson.VCam.OutputChannel = playerChannel;
            _vCamThirdPerson.VCam.OutputChannel = playerChannel;
        }

        #endregion CAMERAS
    }
}