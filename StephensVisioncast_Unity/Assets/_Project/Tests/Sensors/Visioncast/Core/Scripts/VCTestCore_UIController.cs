using Stephens.AI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VisioncastTests.Core
{
    public class VCTestCore_UIController : MonoBehaviour
    {
        #region VARIABLES

        [Header("Data")]
        [SerializeField] private DataConfigAIVisionSource _config;
        
        [Header("References")] 
        [SerializeField] private VCTestCore_Controller _controller;
        [SerializeField] private Button _buttonToggleUI;
        [SerializeField] private TMP_Text _labelUIToggle;
        [SerializeField] private GameObject _contentObj;
        [SerializeField] private Button _buttonToggleInfo;
        [SerializeField] private GameObject _objInfoPanel;
        [SerializeField] private Button _buttonNextCamera;
        [SerializeField] private Button _buttonPreviousCamera;
        [SerializeField] private TMP_Text _textCameraLabel;
        [SerializeField] private Button _buttonToggleLoSDebug;
        [SerializeField] private TMP_Text _textLoSDebugState;
        [SerializeField] private Image _bgLoSToggleButton;
        [SerializeField] private Slider _sliderRange;
        [SerializeField] private TMP_Text _textLabelRange;
        [SerializeField] private Slider _sliderFoV;
        [SerializeField] private TMP_Text _textLabelFoV;
        
        [Header("Colors")]
        [SerializeField] private Color _colorOn;
        [SerializeField] private Color _colorOff;
        
        #endregion VARIABLES


        #region INITIALIZATION

        private void OnEnable()
        {
            _buttonToggleUI.onClick.AddListener(OnButtonToggleUI);
            _buttonToggleInfo.onClick.AddListener(OnButtonToggleInfo);
            _buttonNextCamera.onClick.AddListener(OnButtonNextCamera);
            _buttonPreviousCamera.onClick.AddListener(OnButtonPreviousCamera);
            _buttonToggleLoSDebug.onClick.AddListener(OnButtonToggleLoSDebug);
            _sliderRange.onValueChanged.AddListener(OnSliderVisionRange);
            _sliderFoV.onValueChanged.AddListener(OnSliderVisionFoV);

            // Set config defaults
            _sliderRange.value = _config.VisionRange;
            _textLabelRange.text = _sliderRange.value.ToString("F0");
            _sliderFoV.value = _config.FieldOfViewRange.x;
            _textLabelFoV.text = _sliderFoV.value.ToString("F0");
        }

        private void OnDisable()
        {
            _buttonToggleUI.onClick.RemoveListener(OnButtonToggleUI);
            _buttonToggleInfo.onClick.RemoveListener(OnButtonToggleInfo);
            _buttonNextCamera.onClick.RemoveListener(OnButtonNextCamera);
            _buttonPreviousCamera.onClick.RemoveListener(OnButtonPreviousCamera);
            _buttonToggleLoSDebug.onClick.RemoveListener(OnButtonToggleLoSDebug);
            _sliderRange.onValueChanged.RemoveListener(OnSliderVisionRange);
            _sliderFoV.onValueChanged.RemoveListener(OnSliderVisionFoV);
        }

        #endregion INITIALIZATION


        #region UI EVENTS

        private void OnButtonToggleUI()
        {
            _contentObj.SetActive(!_contentObj.activeSelf);
            _labelUIToggle.text = _contentObj.activeSelf ? "hide ui" : "show ui";
        }
        
        private void OnButtonToggleInfo()
        {
            _objInfoPanel.SetActive(!_objInfoPanel.activeSelf);
        }
        
        private void OnButtonNextCamera()
        {
            _textCameraLabel.text = _controller.AdvanceCameraMode(1);
        }
        
        private void OnButtonPreviousCamera()
        {
            _textCameraLabel.text = _controller.AdvanceCameraMode(-1);
        }

        private void OnButtonToggleLoSDebug()
        {
            bool state = _controller.ToggleLoSDebugLines();
            _textLoSDebugState.text = state ? "on" : "off";
            _bgLoSToggleButton.color = state ? _colorOn : _colorOff;
        }
        
        private void OnSliderVisionRange(float val)
        {
            _textLabelRange.text = val.ToString("F0");
            _config.VisionRange = val;
        }
        
        private void OnSliderVisionFoV(float val)
        {
            _textLabelFoV.text = val.ToString("F0");
            _config.FieldOfViewRange = Vector2.one * val;
        }

        #endregion UI EVENTS
    }
}

