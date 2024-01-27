namespace Stephens.Tick
{
    public static partial class TickRouter
    {
        #region DEFAULT CALLBACKS

        // These groups are called in order every Update()
        private static readonly TickGroup[] _orderedGroupsUpdate =
        {
            TickGroup.Input,                    // Ticks the Unity input system
            TickGroup.InputCollection,        // Forwards collected input data from listeners to receivers
            TickGroup.InputTransmission,        // Forwards collected input data from listeners to receivers
            TickGroup.HumanoidAnimation,
            
            TickGroup.DefaultGroupUpdate
        };
        
        // These groups are called in order every FixedUpdate()
        private static readonly TickGroup[] _orderedGroupsFixedUpdate =
        {
            TickGroup.PhysicsRaycast,           // Ticks the ordered raycast system
            TickGroup.DefaultGroupFixedUpdate, 
            TickGroup.ControllerHumanoid,
            TickGroup.InteractionSystem,

            TickGroup.Physics
        };
        
        // These groups are called in order every LateUpdate()
        private static readonly TickGroup[] _orderedGroupsLateUpdate =
        {
            TickGroup.CameraMovement,
            TickGroup.Debug,
            TickGroup.UIUpdate,
            TickGroup.ViewKCC2D,
            
            TickGroup.DefaultGroupLateUpdate
        };

        #endregion DEFAULT CALLBACKS


        #region TIMED CALLBACKS

        // These groups are called in order every 0.1 seconds
        private static readonly TickGroup[] _orderedGroupsCSTTenthSecond =
        {
            TickGroup.VisibleObjectBoundsRefresh,
            TickGroup.VisionCaster,
            TickGroup.FogOfWarRefresh,
            TickGroup.DecoratorsObstructionRefresh,

            TickGroup.DefaultGroupTenthSecond
        };
        
        // These groups are called in order every 0.5 seconds
        private static readonly TickGroup[] _orderedGroupsCSTHalfSecond =
        {
            TickGroup.PhysicsDiscoverableSleepTick, 
            
            TickGroup.DefaultGroupHalfSecond
        };
        
        // These groups are called in order every second
        private static readonly TickGroup[] _orderedGroupsCSTFullSecond =
        {
            TickGroup.DefaultGroupFullSecond
        };
        
        // These groups are called in order every two seconds
        private static readonly TickGroup[] _orderedGroupsCSTTwoSeconds =
        {
            TickGroup.DefaultGroupTwoSeconds
        };
        
        // These groups are called in order every five seconds
        private static readonly TickGroup[] _orderedGroupsCSTFiveSeconds =
        {
            TickGroup.DefaultGroupFiveSeconds
        };

        #endregion TIMED CALLBACKS


        #region ALTERNATE CALLBACKS

        private static readonly TickGroup[] _orderedGroupsAltUpdateA =
        {
            
        };
        
        private static readonly TickGroup[] _orderedGroupsAltUpdateB =
        {
            
        };

        private static readonly TickGroup[] _orderedGroupsAltFixedUpdateA =
        {
            
        };
        
        private static readonly TickGroup[] _orderedGroupsAltFixedUpdateB =
        {
            
        };
        
        private static readonly TickGroup[] _orderedGroupsAltLateUpdateA =
        {
            
        };
        
        private static readonly TickGroup[] _orderedGroupsAltLateUpdateB =
        {
            
        };

        #endregion ALTERNATE CALLBACKS
    }
}