using System.Collections.Generic;
using UnityEngine;

namespace GalaxyGourd.Tick
{
    /// <summary>
    /// Collates tick callbacks
    /// </summary>
    public static partial class TickRouter
    {
        #region VARIABLES

        // Updates
        private static TickCollection _collectionUpdate;
        private static TickCollection _collectionFixedUpdate;
        private static TickCollection _collectionLateUpdate;
        
        // Alternate updates - A is ticked, then B the next update, etc
        private static TickCollection _collectionAltUpdateA;
        private static TickCollection _collectionAltUpdateB;
        private static bool _altFlagUpdate;
        private static TickCollection _collectionAltFixedUpdateA;
        private static TickCollection _collectionAltFixedUpdateB;
        private static bool _altFlagFixedUpdate;
        private static TickCollection _collectionAltLateUpdateA;
        private static TickCollection _collectionAltLateUpdateB;
        private static bool _altFlagLateUpdate;

        // Custom
        private static TickCustom _collectionTenthSecond;
        private static TickCustom _collectionHalfSecond;
        private static TickCustom _collectionFullSecond;
        private static TickCustom _collectionTwoSeconds;
        private static TickCustom _collectionFiveSeconds;

        private static readonly List<ITickable> _queueAdd = new();
        private static readonly List<ITickable> _queueRemove = new();
        
        #endregion VARIABLES


        #region LOAD

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemRegistration()
        {
            ClearAllTickables();
            ConstructTickCollections(Resources.Load<DataConfigTickCollections>("DAT_TickCollections"));
        }
        
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
            List<ITickable> collection = GetTickGroupList(tickable.TickGroup);
            collection?.Add(tickable);
        }

        private static void Remove(ITickable tickable)
        {
            List<ITickable> collection = GetTickGroupList(tickable.TickGroup);
            collection?.Remove(tickable);
        }

        #endregion REGISTRATION


        #region TICK

        internal static void TickUpdate(float delta)
        {
            _collectionUpdate.Tick(delta);

            if (_altFlagUpdate)
                _collectionAltUpdateA.Tick(delta);
            else
                _collectionAltUpdateB.Tick(delta);

            _altFlagUpdate = !_altFlagUpdate;
            CheckCustomTicks(delta);
        }
        
        internal static void TickFixedUpdate(float delta)
        {
            _collectionFixedUpdate.Tick(delta);

            if (_altFlagFixedUpdate)
                _collectionAltFixedUpdateA.Tick(delta);
            else
                _collectionAltFixedUpdateB.Tick(delta);
            
            _altFlagFixedUpdate = !_altFlagFixedUpdate;
        }
        
        internal static void TickLateUpdate(float delta)
        {
            _collectionLateUpdate.Tick(delta);

            if (_altFlagLateUpdate)
                _collectionAltLateUpdateA.Tick(delta);
            else
                _collectionAltLateUpdateB.Tick(delta);

            _altFlagLateUpdate = !_altFlagLateUpdate;
            FlushQueued();
        }

        private static void CheckCustomTicks(float delta)
        {
            if (_collectionTenthSecond.TickHasElapsed(0.1f, delta))
            {
                _collectionTenthSecond.Tick(delta);
            }
            
            if (_collectionHalfSecond.TickHasElapsed(0.5f, delta))
            {
                _collectionHalfSecond.Tick(delta);
            }
            
            if (_collectionFullSecond.TickHasElapsed(1f, delta))
            {
                _collectionFullSecond.Tick(delta);
            }
            
            if (_collectionTwoSeconds.TickHasElapsed(2f, delta))
            {
                _collectionTwoSeconds.Tick(delta);
            }
            
            if (_collectionFiveSeconds.TickHasElapsed(5f, delta))
            {
                _collectionFiveSeconds.Tick(delta);
            }
        }

        #endregion TICK


        #region UTILITY

        private static void ConstructTickCollections(DataConfigTickCollections data)
        {
            _collectionUpdate = new TickCollection(data.GetUpdateGroups());
            _collectionFixedUpdate = new TickCollection(data.GetFixedUpdateGroups());
            _collectionLateUpdate = new TickCollection(data.GetLateUpdateGroups());
            
            _collectionAltUpdateA = new TickCollection(data.GetAltUpdateAGroups());
            _collectionAltUpdateB = new TickCollection(data.GetAltUpdateBGroups());
            _collectionAltFixedUpdateA = new TickCollection(data.GetAltFixedUpdateAGroups());
            _collectionAltFixedUpdateB = new TickCollection(data.GetAltFixedUpdateBGroups());
            _collectionAltLateUpdateA = new TickCollection(data.GetAltLateUpdateAGroups());
            _collectionAltLateUpdateB = new TickCollection(data.GetAltLateUpdateBGroups());
            
            _collectionTenthSecond = new TickCustom(data.GetTimedUpdate100MSGroups());
            _collectionHalfSecond = new TickCustom(data.GetTimedUpdate500MSGroups());
            _collectionFullSecond = new TickCustom(data.GetTimedUpdate1SGroups());
            _collectionTwoSeconds = new TickCustom(data.GetTimedUpdate2SGroups());
            _collectionFiveSeconds = new TickCustom(data.GetTimedUpdate5SGroups());
        }

        private static List<ITickable> GetTickGroupList(int key)
        {
            foreach (Dictionary<int, List<ITickable>> group in _collectionUpdate.Tickables)
                if (group.TryGetValue(key, out var list)) return list;
            foreach (Dictionary<int, List<ITickable>> group in _collectionFixedUpdate.Tickables)
                if (group.TryGetValue(key, out var list)) return list;
            foreach (Dictionary<int, List<ITickable>> group in _collectionLateUpdate.Tickables)
                if (group.TryGetValue(key, out var list)) return list;
            foreach (Dictionary<int, List<ITickable>> group in _collectionAltUpdateA.Tickables)
                if (group.TryGetValue(key, out var list)) return list;
            foreach (Dictionary<int, List<ITickable>> group in _collectionAltUpdateB.Tickables)
                if (group.TryGetValue(key, out var list)) return list;
            foreach (Dictionary<int, List<ITickable>> group in _collectionAltFixedUpdateA.Tickables)
                if (group.TryGetValue(key, out var list)) return list;
            foreach (Dictionary<int, List<ITickable>> group in _collectionAltFixedUpdateB.Tickables)
                if (group.TryGetValue(key, out var list)) return list;
            foreach (Dictionary<int, List<ITickable>> group in _collectionAltLateUpdateA.Tickables)
                if (group.TryGetValue(key, out var list)) return list;
            foreach (Dictionary<int, List<ITickable>> group in _collectionAltLateUpdateB.Tickables)
                if (group.TryGetValue(key, out var list)) return list;
            foreach (Dictionary<int, List<ITickable>> group in _collectionTenthSecond.Tickables)
                if (group.TryGetValue(key, out var list)) return list;
            foreach (Dictionary<int, List<ITickable>> group in _collectionHalfSecond.Tickables)
                if (group.TryGetValue(key, out var list)) return list;
            foreach (Dictionary<int, List<ITickable>> group in _collectionTenthSecond.Tickables)
                if (group.TryGetValue(key, out var list)) return list;
            foreach (Dictionary<int, List<ITickable>> group in _collectionTwoSeconds.Tickables)
                if (group.TryGetValue(key, out var list)) return list;
            foreach (Dictionary<int, List<ITickable>> group in _collectionFiveSeconds.Tickables)
                if (group.TryGetValue(key, out var list)) return list;

            return null;
        }

        private static void ClearAllTickables()
        {
            _queueAdd.Clear();
            _queueRemove.Clear();
        }

        #endregion UTILITY
    }
}