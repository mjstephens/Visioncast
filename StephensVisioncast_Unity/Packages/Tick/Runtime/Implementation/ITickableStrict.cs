namespace GalaxyGourd.Tick
{
    /// <summary>
    /// Override of ITickable that ensures inheritors are IMMEDIATELY added or removed from the tick queue when registering
    /// or unregistering with the TickRouter
    /// </summary>
    public interface ITickableStrict : ITickable
    {
        
    }
}