using System.Collections.Generic;
using UnityEngine;

namespace GalaxyGourd.Visioncast
{
    /// <summary>
    /// Data to hold raw results from Visioncaster system.
    /// </summary>
    public struct DataVisioncastResult
    {
        public List<IVisioncastTargetable> Objects;
        public List<List<Vector3>> VisiblePoints;
        public List<float> Distances;
        public List<float> Angles;
    }
}