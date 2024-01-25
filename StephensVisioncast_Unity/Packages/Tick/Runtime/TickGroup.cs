namespace Stephens.Tick
{
    public enum TickGroup
    {
        // GG generic
        Physics,                // Ticks physics simulation
        Input,                  // Ticks raw input device listening
        InputCollection,
        InputTransmission,      // Ticks sending input values to listeners
        UIUpdate,
        Debug,                  // Ticks debug visualizations
        
        // KCC2D
        ControllerKCC2D,        // Ticks physics calcs for KCC2D
        ViewKCC2D,              // Ticks interpolation for KCC2D view
        
        // App-Specific
        ControllerHumanoid,
        CameraMovement,
        PhysicsRaycast,
        VisionCaster,
        VisibleObjectBoundsRefresh,
        InteractionSystem,
        FogOfWarRefresh,
        DecoratorsObstructionRefresh,
        PhysicsDiscoverableSleepTick,
        HumanoidAnimation,
        
        // Defaults
        DefaultGroupUpdate,
        DefaultGroupFixedUpdate,
        DefaultGroupLateUpdate,
        DefaultGroupTenthSecond,
        DefaultGroupHalfSecond,
        DefaultGroupFullSecond,
        DefaultGroupTwoSeconds,
        DefaultGroupFiveSeconds
    }
}