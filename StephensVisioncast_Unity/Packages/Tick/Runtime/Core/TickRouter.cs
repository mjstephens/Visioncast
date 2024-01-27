using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Stephens.Tick
{
    /// <summary>
    /// Collates tick callbacks
    /// </summary>
    public static partial class TickRouter
    {
        #region VARIABLES

        // Updates
        private static readonly Dictionary<TickGroup, List<ITickable>> _tickablesUpdate = new();
        private static readonly Dictionary<TickGroup, List<ITickable>> _tickablesFixedUpdate = new();
        private static readonly Dictionary<TickGroup, List<ITickable>> _tickablesLateUpdate = new();
        
        // Alternate updates - A is ticked, then B the next update, etc
        private static readonly Dictionary<TickGroup, List<ITickable>> _tickablesAltUpdateA = new();
        private static readonly Dictionary<TickGroup, List<ITickable>> _tickablesAltUpdateB = new();
        private static bool _altFlagUpdate;
        private static readonly Dictionary<TickGroup, List<ITickable>> _tickablesAltFixedUpdateA = new();
        private static readonly Dictionary<TickGroup, List<ITickable>> _tickablesAltFixedUpdateB = new();
        private static bool _altFlagFixedUpdate;
        private static readonly Dictionary<TickGroup, List<ITickable>> _tickablesAltLateUpdateA = new();
        private static readonly Dictionary<TickGroup, List<ITickable>> _tickablesAltLateUpdateB = new();
        private static bool _altFlagLateUpdate;

        // Custom
        private static readonly TickCustom _tickablesTenthSecond = new(0.1f);
        private static readonly TickCustom _tickablesHalfSecond = new(0.5f);
        private static readonly TickCustom _tickablesFullSecond = new(1f);
        private static readonly TickCustom _tickablesTwoSeconds = new(2f);
        private static readonly TickCustom _tickablesFiveSeconds = new(5f);

        private static readonly List<ITickable> _queueAdd = new();
        private static readonly List<ITickable> _queueRemove = new();
        
        #endregion VARIABLES


        #region LOAD

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemRegistration()
        {
            ClearAllTickables();
            
            // Load project-defined tick groups, compile into runtime groups
            DataConfigTickList groups = Resources.Load<DataConfigTickList>("DAT_TickGroups");
            foreach (string group in groups.UpdateGroups)
            {
                _tickablesUpdate1.Add(new DataTickGroup()
                {
                    Interval = TickInterval.Update,
                    Key = group
                }, new List<ITickable>());
            }
        }
        
        private static readonly Dictionary<DataTickGroup, List<ITickable>> _tickablesUpdate1 = new();


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnAfterSceneLoad()
        {
            GameObject tickSource = new GameObject("[TickSource]");
            tickSource.AddComponent<TickSource>();
        }

        #endregion LOAD
        
        
        #region REGISTRATION

        public static void Register(ITickable tickable)
        {
            // Immediately add if strict
            if (tickable is ITickableStrict)
            {
                Add(tickable);
            }
            else
            {            
                _queueAdd.Add(tickable);
            }
        }

        public static void Unregister(ITickable tickable)
        {
            // Immediately remove if strict
            if (tickable is ITickableStrict)
            {
                Remove(tickable);
            }
            else
            {
                _queueRemove.Add(tickable);
            }
        }

        internal static void FlushQueued()
        {
            foreach (ITickable tickable in _queueAdd)
            {
                Add(tickable);
            }
            
            foreach (ITickable tickable in _queueRemove)
            {
                Remove(tickable);
            }
            
            _queueAdd.Clear();
            _queueRemove.Clear(); 
        }

        private static void Add(ITickable tickable)
        {
            Dictionary<TickGroup, List<ITickable>> collection = GetTickCollectionForType(tickable.TickGroup);
            collection.TryAdd(tickable.TickGroup, new List<ITickable>());
            collection[tickable.TickGroup].Add(tickable);
        }

        private static void Remove(ITickable tickable)
        {
            Dictionary<TickGroup, List<ITickable>> collection = GetTickCollectionForType(tickable.TickGroup);
            collection[tickable.TickGroup].Remove(tickable);
        }

        #endregion REGISTRATION


        #region TICK

        internal static void TickUpdate(float delta)
        {
            foreach (TickGroup group in _orderedGroupsUpdate)
            {
                TickCollection(group,_tickablesUpdate, delta);
            }

            // Tick the alternate group
            foreach (TickGroup group in _altFlagUpdate ? _orderedGroupsAltUpdateA : _orderedGroupsAltUpdateB)
            {
                TickCollection(group, _altFlagUpdate ? _tickablesAltUpdateA : _tickablesAltUpdateB, delta);
            }

            _altFlagUpdate = !_altFlagUpdate;
            CheckCustomTicks(delta);
        }
        
        internal static void TickFixedUpdate(float delta)
        {
            foreach (TickGroup group in _orderedGroupsFixedUpdate)
            {
                TickCollection(group, _tickablesFixedUpdate, delta);
            }
            
            // Tick the alternate group
            foreach (TickGroup group in _altFlagFixedUpdate ? _orderedGroupsAltFixedUpdateA : _orderedGroupsAltFixedUpdateB)
            {
                TickCollection(group,_altFlagFixedUpdate ? _tickablesAltFixedUpdateA : _tickablesAltFixedUpdateB, delta);
            }

            _altFlagFixedUpdate = !_altFlagFixedUpdate;
        }
        
        internal static void TickLateUpdate(float delta)
        {
            foreach (TickGroup group in _orderedGroupsLateUpdate)
            {
                TickCollection(group, _tickablesLateUpdate, delta);
            }

            // Tick the alternate group
            foreach (TickGroup group in _altFlagLateUpdate ? _orderedGroupsAltLateUpdateA : _orderedGroupsAltLateUpdateB)
            {
                TickCollection(group,_altFlagLateUpdate ? _tickablesAltLateUpdateA : _tickablesAltLateUpdateB, delta);
            }

            _altFlagLateUpdate = !_altFlagLateUpdate;
            FlushQueued();
        }

        private static void CheckCustomTicks(float delta)
        {
            if (_tickablesTenthSecond.TickHasElapsed(delta))
            {
                foreach (TickGroup group in _orderedGroupsCSTTenthSecond)
                {
                    TickCollection(group, _tickablesTenthSecond.Tickables, delta);
                }
            }
            
            if (_tickablesHalfSecond.TickHasElapsed(delta))
            {
                foreach (TickGroup group in _orderedGroupsCSTHalfSecond)
                {
                    TickCollection(group, _tickablesHalfSecond.Tickables, delta);
                }
            }
            
            if (_tickablesFullSecond.TickHasElapsed(delta))
            {
                foreach (TickGroup group in _orderedGroupsCSTFullSecond)
                {
                    TickCollection(group, _tickablesFullSecond.Tickables, delta);
                }
            }
            
            if (_tickablesTwoSeconds.TickHasElapsed(delta))
            {
                foreach (TickGroup group in _orderedGroupsCSTTwoSeconds)
                {
                    TickCollection(group, _tickablesTwoSeconds.Tickables, delta);
                }
            }
            
            if (_tickablesFiveSeconds.TickHasElapsed(delta))
            {
                foreach (TickGroup group in _orderedGroupsCSTFiveSeconds)
                {
                    TickCollection(group, _tickablesFiveSeconds.Tickables, delta);
                }
            }
        }

        private static void TickCollection(TickGroup group, Dictionary<TickGroup, List<ITickable>> collection, float delta)
        {
            if (!collection.ContainsKey(group))
                return;

            foreach (ITickable tickable in collection[group])
            {
                tickable.Tick(delta);
            }
        }

        #endregion TICK


        #region UTILITY

        private static Dictionary<TickGroup, List<ITickable>> GetTickCollectionForType(TickGroup group)
        {
            if (_orderedGroupsUpdate.Contains(group)) return _tickablesUpdate;
            if (_orderedGroupsAltUpdateA.Contains(group)) return _tickablesAltUpdateA;
            if (_orderedGroupsAltUpdateB.Contains(group)) return _tickablesAltUpdateB;
            if (_orderedGroupsFixedUpdate.Contains(group)) return _tickablesFixedUpdate;
            if (_orderedGroupsAltFixedUpdateA.Contains(group)) return _tickablesAltFixedUpdateA;
            if (_orderedGroupsAltFixedUpdateB.Contains(group)) return _tickablesAltFixedUpdateB;
            if (_orderedGroupsLateUpdate.Contains(group)) return _tickablesLateUpdate;
            if (_orderedGroupsAltLateUpdateA.Contains(group)) return _tickablesAltLateUpdateA;
            if (_orderedGroupsAltLateUpdateB.Contains(group)) return _tickablesAltLateUpdateB;
            if (_orderedGroupsCSTTenthSecond.Contains(group)) return _tickablesTenthSecond.Tickables;
            if (_orderedGroupsCSTHalfSecond.Contains(group)) return _tickablesHalfSecond.Tickables;
            if (_orderedGroupsCSTFullSecond.Contains(group)) return _tickablesFullSecond.Tickables;
            if (_orderedGroupsCSTTwoSeconds.Contains(group)) return _tickablesTwoSeconds.Tickables;
            if (_orderedGroupsCSTFiveSeconds.Contains(group)) return _tickablesFiveSeconds.Tickables;

            return null;
        }

        private static void ClearAllTickables()
        {
            _tickablesUpdate.Clear();
            _tickablesAltUpdateA.Clear();
            _tickablesAltUpdateB.Clear();
            _tickablesFixedUpdate.Clear();
            _tickablesAltFixedUpdateA.Clear();
            _tickablesAltFixedUpdateB.Clear();
            _tickablesLateUpdate.Clear();
            _tickablesAltLateUpdateA.Clear();
            _tickablesAltLateUpdateB.Clear();
            _tickablesTenthSecond.Reset();
            _tickablesHalfSecond.Reset();
            _tickablesFullSecond.Reset();
            _tickablesTwoSeconds.Reset();
            _tickablesFiveSeconds.Reset();
            _queueAdd.Clear();
            _queueRemove.Clear();
        }

        #endregion UTILITY
    }
}