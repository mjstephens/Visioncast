using System.Collections.Generic;
using Stephens.Tick;
using UnityEngine;

namespace Stephens.Sensors
{
    [RequireComponent(typeof(IVisionSource))]
    public sealed class DebugVisionSource : MonoBehaviour, ITickable
    {
        #region VARIABLES

        [Header("Draw Options")]
        [SerializeField] private bool _gizmos = true;
        [SerializeField] private bool _lineRenderers;
        
        public TickGroup TickGroup => TickGroup.Debug;

        private IVisionSource _source;
        private readonly List<DebugLineInstance> _debugLines = new();
        
        #endregion VARIABLES


        #region INITIALIZATION

        private void Awake()
        {
            _source = GetComponent<IVisionSource>();
        }

        private void OnEnable()
        {
            TickRouter.Register(this);
        }

        private void OnDisable()
        {
            TickRouter.Unregister(this);
        }

        #endregion INITIALIZATION


        #region TICK

        void ITickable.Tick(float delta)
        {
            DrawLine(_source.Position, _source.Heading * _source.Range, Color.yellow);
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
               // DebugLineInstance line = GetNextAvailableLine();
            }
        }

        #endregion TICK


        #region UTILITY

        // private DebugLineInstance GetNextAvailableLine()
        // {
        //     
        // }

        #endregion UTILITY
    }
}