using System.Collections.Generic;

namespace Stephens.Tick
{
    internal class TickCustom
    {
        internal readonly float UpdateRate;
        internal readonly Dictionary<TickGroup, List<ITickable>> Tickables;
        internal float ElapsedSincePreviousUpdate;
        
        internal TickCustom(float updateRate)
        {
            UpdateRate = updateRate;
            Tickables = new Dictionary<TickGroup, List<ITickable>>();
        }

        internal void Reset()
        {
            Tickables.Clear();
            ElapsedSincePreviousUpdate = 0;
        }

        internal bool TickHasElapsed(float delta)
        {
            ElapsedSincePreviousUpdate += delta;
            if (ElapsedSincePreviousUpdate >= UpdateRate)
            {
                ElapsedSincePreviousUpdate -= UpdateRate;
                return true;
            }
            
            return false;
        }
    }
}