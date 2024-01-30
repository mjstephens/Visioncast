using System.Collections.Generic;
using GalaxyGourd.Visioncast;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VisioncastSamples.Core
{
    public class VCSampleCore_UIController : MonoBehaviour
    {
        #region VARIABLES

        [Header("References")] 
        [SerializeField] private VCSampleCore_Controller _controller;
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
        [SerializeField] private Button _buttonToggleConesDebug;
        [SerializeField] private TMP_Text _textConesDebugState;
        [SerializeField] private Image _bgLoSToggleButton;
        [SerializeField] private Image _bgConesToggleButton;
        [SerializeField] private Slider _sliderRange;
        [SerializeField] private TMP_Text _textLabelRange;
        [SerializeField] private Slider _sliderFoV;
        [SerializeField] private TMP_Text _textLabelFoV;
        
        [Header("Colors")]
        [SerializeField] private Color _colorOn;
        [SerializeField] private Color _colorOff;
        
        private List<VisioncastSource> _sources;
        
        #endregion VARIABLES


        #region INITIALIZATION

        private void OnEnable()
        {
            _buttonToggleUI.onClick.AddListener(OnButtonToggleUI);
            _buttonToggleInfo.onClick.AddListener(OnButtonToggleInfo);
            _buttonNextCamera.onClick.AddListener(OnButtonNextCamera);
            _buttonPreviousCamera.onClick.AddListener(OnButtonPreviousCamera);
            _buttonToggleLoSDebug.onClick.AddListener(OnButtonToggleLoSDebug);
            _buttonToggleConesDebug.onClick.AddListener(OnButtonToggleConesDebug);
            _sliderRange.onValueChanged.AddListener(OnSliderVisionRange);
            _sliderFoV.onValueChanged.AddListener(OnSliderVisionFoV);
           
            OnVisionSourcesModified();
        }

        private void OnDisable()
        {
            _buttonToggleUI.onClick.RemoveListener(OnButtonToggleUI);
            _buttonToggleInfo.onClick.RemoveListener(OnButtonToggleInfo);
            _buttonNextCamera.onClick.RemoveListener(OnButtonNextCamera);
            _buttonPreviousCamera.onClick.RemoveListener(OnButtonPreviousCamera);
            _buttonToggleLoSDebug.onClick.RemoveListener(OnButtonToggleLoSDebug);
            _buttonToggleConesDebug.onClick.RemoveListener(OnButtonToggleConesDebug);
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
        
        private void OnButtonToggleConesDebug()
        {
            bool state = _controller.ToggleSightDebugCones();
            _textConesDebugState.text = state ? "on" : "off";
            _bgConesToggleButton.color = state ? _colorOn : _colorOff;
        }
        
        private void OnSliderVisionRange(float val)
        {
            _textLabelRange.text = val.ToString("F0");
            foreach (VisioncastSource visionSource in _controller.Sources)
            {
                if (visionSource is VisioncastSourceSimple s)
                {
                    s.OverrideRange(val);
                }
            }
        }
        
        private void OnSliderVisionFoV(float val)
        {
            _textLabelFoV.text = val.ToString("F0");
            foreach (VisioncastSource visionSource in _controller.Sources)
            {
                if (visionSource is VisioncastSourceSimple s)
                {
                    s.OverrideFieldOfView(val);
                }
            }
        }

        #endregion UI EVENTS


        #region UTILITY

        internal void OnVisionSourcesModified()
        {
            if (_controller.Sources == null || _controller.Sources.Count == 0)
                return;
            
            _sliderRange.value = _controller.Sources[0].Range;
            _textLabelRange.text = _sliderRange.value.ToString("F0");
            _sliderFoV.value = _controller.Sources[0].FieldOfView;
            _textLabelFoV.text = _sliderFoV.value.ToString("F0");
        }

        #endregion UTILITY
    }
}

