namespace Stephens.Tick
{
    /// <summary>
    /// Defines how frequently a tickable is ticked
    /// </summary>
    public enum TickInterval
    {
        Update,
        LateUpdate,
        FixedUpdate,
        
        Timed_100MS, // 0.1 second
        Timed_500MS, // 0.5 seconds
        Timed_1S,
        Timed_2S,
        Timed_5S,
        
        AltUpdate_A,
        AltUpdate_B,
        AltLateUpdate_A,
        AltLateUpdate_B,
        AltFixedUpdate_A,
        AltFixedUpdate_B
    }
}