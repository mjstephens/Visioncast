using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stephens.Sensors
{
    public class DataVisioncastResult
    {
        public List<IVisibleObject> Objects;
        public List<List<Vector3>> VisiblePoints;
        public List<float> Distances;
        public List<float> Angles;
    }
}