namespace Stephens.Sensors
{
    /// <summary>
    /// Defines base data relating to seen objects
    /// </summary>
    public struct DataVisionSeenObject
    {
        public IVisibleObject ResultObject;
        public bool IsVisible;
        public bool JustBecameVisible;
        
        public float Distance;
        public float Angle;
    }
}