using System.Collections.Generic;

namespace GalaxyGourd.Tick
{
    internal class TickCollection
    {
        #region VARIABLES
        
        internal readonly List<Dictionary<int, List<ITickable>>> Tickables = new();

        #endregion VARIABLES


        #region INITIALIZATION

        internal TickCollection(IEnumerable<int> orderedGroups)
        {
            foreach (int group in orderedGroups)
            {
                Tickables.Add(new Dictionary<int, List<ITickable>>
                {
                    { group, new() }
                });
            }
        }

        #endregion INITIALIZATION


        #region TICK

        internal void Tick(float delta)
        {
            foreach (Dictionary<int, List<ITickable>> group in Tickables)
            {
                foreach (KeyValuePair<int, List<ITickable>> pair in group)
                {
                    foreach (ITickable tickable in pair.Value)
                    {
                        tickable.Tick(delta);
                    }
                }
            }
        }

        #endregion TICK
    }
}