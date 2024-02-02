namespace GalaxyGourd.Visioncast
{
    /// <summary>
    /// Refined data describing "seen" object details
    /// </summary>
    public struct DataVisionSeenObject
    {
        public IVisioncastTargetable ResultObject;
        public bool IsVisible;
        public bool JustBecameVisible;
        public float Distance;
        public float Angle;
    }
}