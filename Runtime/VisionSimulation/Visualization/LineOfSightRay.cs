using UnityEngine;

namespace GalaxyGourd.Visioncast
{
    [RequireComponent(typeof(LineRenderer))]
    public class LineOfSightRay : MonoBehaviour
    {
        #region VARIABLES

        private LineRenderer _line;
        private float _defaultTickness;

        #endregion VARIABLES


        #region INITIALIZATION

        private void Awake()
        {
            _line = GetComponent<LineRenderer>();
            _defaultTickness = _line.startWidth;
        }

        #endregion INITIALIZATION


        #region API

        public virtual void SetParameters(Vector3 start, Vector3 end, Color color, float thickness)
        {
            _line.SetPositions(new []
            {
                start, end
            });
            _line.startColor = color;
            _line.endColor = color;
            _line.startWidth = _defaultTickness * thickness;
            _line.endWidth = _defaultTickness * thickness;
        }

        #endregion API
    }
}