namespace GalaxyGourd.Tick
{
    /// <summary>
    /// Define your project-specific tick groups here
    /// </summary>
    public enum TickGroups
    {
        DefaultUpdate,
        DefaultFixedUpdate,
        DefaultLateUpdate,
        
        VisionCaster,
        Debug,
        PhysicsRaycast,
        ControllerHumanoid,
        HumanoidAnimation,
        UIUpdate,
        InputCollection,
        InputTransmission,
        VisibleObjectBoundsRefresh,
        InteractionSystem,
        CameraMovement
    }
}
