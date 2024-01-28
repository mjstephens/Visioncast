using System.Collections.Generic;
using GalaxyGourd.Tick;
using Stephens.Utility;
using UnityEngine;

namespace Stephens.Sensors
{
    [RequireComponent(typeof(IVisionSource))]
    public sealed class DebugVisioncastSource : TickableMonoBehaviour
    {
        #region VARIABLES

        [Header("References")]
        [SerializeField] private GameObject _prefabLineDebug;
        [SerializeField] private MeshFilter _coneRend;
        
        [Header("Draw Options")]
        [SerializeField] private bool _gizmos = true;
        [SerializeField] private bool _lineRenderers;
        
        public override int TickGroup => (int)TickGroups.Debug;

        private IVisionSource _source;        
        private ConeMesh _cone;
        private readonly List<DebugLineInstance> _debugLines = new();
        
        #endregion VARIABLES


        #region INITIALIZATION

        private void Awake()
        {
            _source = GetComponent<IVisionSource>();

            if (_coneRend)
            {
                _cone = new ConeMesh();
                _coneRend.mesh = _cone.CreateConeMesh(
                    "cone",
                    40,
                    Vector3.zero, 
                    Quaternion.Euler(_source.Heading),
                    _source.FieldOfViewRange.x,
                    _source.FieldOfViewRange.y);
            }
            
        }

        #endregion INITIALIZATION


        #region TICK

        public override void Tick(float delta)
        {
            ClearLines();

            Vector3 headingEnd = _source.Position + (_source.Heading * _source.Range);
            DrawLine(_source.Position, headingEnd, Color.yellow);
            if (_source.LastResults == null)
                return;

            Vector3 sourcePosition = _source.Position;
            for (int i = 0; i < _source.LastResults.Objects.Count; i++)
            {
                if (_source.LastResults.Objects[i] == null || !_source.LastResults.Objects[i].IsValidForInteraction)
                    continue;
                
                if (_source.LastResults.VisiblePoints[i].Count == 0)
                {
                    DrawLine(sourcePosition, _source.LastResults.Objects[i].Position, Color.red);
                }
                else
                {
                    for (int e = 0; e < _source.LastResults.VisiblePoints[i].Count; e++)
                    {
                        DrawLine(sourcePosition, _source.LastResults.VisiblePoints[i][e], Color.green);
                    }
                }
            }
        }

        private void DrawLine(Vector3 start, Vector3 end, Color color)
        {
            if (_gizmos)
            {
                Debug.DrawLine(start, end, color);
            }

            if (_lineRenderers)
            {
               DebugLineInstance line = GetNextAvailableLine();
               if (line)
               {
                   line.Activate(start, end, color);
                   line.enabled = true;
               }
            }
        }

        #endregion TICK


        #region UTILITY

        public void Toggle(bool on)
        {
            _gizmos = on;
            _lineRenderers = on;
        }
        
        private DebugLineInstance GetNextAvailableLine()
        {
            // Find first inactive line
            DebugLineInstance next = null;
            foreach (DebugLineInstance line in _debugLines)
            {
                if (!line.enabled)
                {
                    next = line;
                    break;
                }
            }
            
            // If needed, create new line
            if (!next && _prefabLineDebug)
            {
                next = Instantiate(_prefabLineDebug, transform).GetComponent<DebugLineInstance>();
                _debugLines.Add(next);
            }

            return next;
        }

        private void ClearLines()
        {
            foreach (DebugLineInstance line in _debugLines)
            {
                line.enabled = false;
            }
        }

        #endregion UTILITY
    }
}