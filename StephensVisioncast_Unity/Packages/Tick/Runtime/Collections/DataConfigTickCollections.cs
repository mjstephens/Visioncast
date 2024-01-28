using System;
using UnityEngine;

namespace GalaxyGourd.Tick
{
    [CreateAssetMenu(
        fileName = "DAT_TickCollections", 
        menuName = "Galaxy Gourd/Ticking/Tick Collections")] 
    public class DataConfigTickCollections : ScriptableObject
    {
        [SerializeField] private TickGroups[] UpdateGroups;
        [SerializeField] private TickGroups[] FixedUpdateGroups;
        [SerializeField] private TickGroups[] LateUpdateGroups;
        
        [Space]
        [SerializeField] private TickGroups[] TimedUpdate_100MSGroups;
        [SerializeField] private TickGroups[] TimedUpdate_500MSGroups;
        [SerializeField] private TickGroups[] TimedUpdate_1SGroups;
        [SerializeField] private TickGroups[] TimedUpdate_2SGroups;
        [SerializeField] private TickGroups[] TimedUpdate_5SGroups;
        
        [Space]
        [SerializeField] private TickGroups[] AltUpdate_AGroups;
        [SerializeField] private TickGroups[] AltUpdate_BGroups;
        [SerializeField] private TickGroups[] AltFixedUpdate_AGroups;
        [SerializeField] private TickGroups[] AltFixedUpdate_BGroups;
        [SerializeField] private TickGroups[] AltLateUpdate_AGroups;
        [SerializeField] private TickGroups[] AltLateUpdate_BGroups;

        public int[] GetUpdateGroups() { return Array.ConvertAll(UpdateGroups, value => (int) value); }
        public int[] GetFixedUpdateGroups() { return Array.ConvertAll(FixedUpdateGroups, value => (int) value); }
        public int[] GetLateUpdateGroups() { return Array.ConvertAll(LateUpdateGroups, value => (int) value); }
        public int[] GetTimedUpdate100MSGroups() { return Array.ConvertAll(TimedUpdate_100MSGroups, value => (int) value); }
        public int[] GetTimedUpdate500MSGroups() { return Array.ConvertAll(TimedUpdate_500MSGroups, value => (int) value); }
        public int[] GetTimedUpdate1SGroups() { return Array.ConvertAll(TimedUpdate_1SGroups, value => (int) value); }
        public int[] GetTimedUpdate2SGroups() { return Array.ConvertAll(TimedUpdate_2SGroups, value => (int) value); }
        public int[] GetTimedUpdate5SGroups() { return Array.ConvertAll(TimedUpdate_5SGroups, value => (int) value); }
        public int[] GetAltUpdateAGroups() { return Array.ConvertAll(AltUpdate_AGroups, value => (int) value); }
        public int[] GetAltUpdateBGroups() { return Array.ConvertAll(AltUpdate_BGroups, value => (int) value); }
        public int[] GetAltFixedUpdateAGroups() { return Array.ConvertAll(AltFixedUpdate_AGroups, value => (int) value); }
        public int[] GetAltFixedUpdateBGroups() { return Array.ConvertAll(AltFixedUpdate_BGroups, value => (int) value); }
        public int[] GetAltLateUpdateAGroups() { return Array.ConvertAll(AltLateUpdate_AGroups, value => (int) value); }
        public int[] GetAltLateUpdateBGroups() { return Array.ConvertAll(AltLateUpdate_BGroups, value => (int) value); }
    }
}