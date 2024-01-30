using GalaxyGourd.Visioncast;
using UnityEngine;

namespace VisioncastSamples.Core
{
    public class VCSampleCore_VisibleObject : VisibleObject
    {
        #region REFERENCES

        [Header("Debug")]
        [SerializeField] private Color _colorSeen;
        [SerializeField] private float _colorSeenIncrement;
        [SerializeField] private float _colorFadeRate;

        private Renderer _debugRenderer;
        private Color _defaultColor;
        private float _normColorValue;
        private Color _targetColor;
        
        #endregion REFERENCES


        #region INITIALIZATION

        protected override void Awake()
        {
            base.Awake();

            _debugRenderer = GetComponent<Renderer>();
            _defaultColor = _debugRenderer.material.color;
        }

        #endregion
    
    
        #region VISION

        public override void Seen()
        {
            base.Seen();

            _normColorValue += _colorSeenIncrement;
        }

        #endregion VISION


        #region TICK

        private void Update()
        {
            _targetColor = Color.Lerp(_defaultColor, _colorSeen, _normColorValue);
            _debugRenderer.material.color = _targetColor;
            
            _normColorValue -= Time.deltaTime * _colorFadeRate;
            _normColorValue = Mathf.Clamp01(_normColorValue);
        }

        #endregion TICK
    }
}

