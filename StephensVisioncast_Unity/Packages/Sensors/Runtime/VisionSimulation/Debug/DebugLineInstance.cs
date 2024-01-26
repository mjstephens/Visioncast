using System;
using UnityEngine;

namespace Stephens.Sensors
{
    public class DebugLineInstance : MonoBehaviour
    {
        #region VARIABLES

        [Header("References")]
        [SerializeField] private LineRenderer _line;
        
        #endregion VARIABLES


        #region INITIALIZATION

        private void OnEnable()
        {
            _line.enabled = true;
        }

        private void OnDisable()
        {
            _line.enabled = false;
        }

        internal void Activate(Vector3 start, Vector3 end, Color color)
        {
            _line.SetPositions(new []{start, end});
            _line.startColor = color;
            _line.endColor = color;
        }

        #endregion INITIALIZATION
    }
}