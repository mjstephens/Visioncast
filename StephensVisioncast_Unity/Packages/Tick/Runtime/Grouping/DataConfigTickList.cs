using UnityEngine;

namespace Stephens.Tick
{
    [CreateAssetMenu]
    public class DataConfigTickList : ScriptableObject
    {
        [SerializeField] internal string[] UpdateGroups;
        [SerializeField] internal string[] FixedUpdateGroups;
        [SerializeField] internal string[] LateUpdateGroups;
    }
}