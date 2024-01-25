using System.Collections.Generic;

namespace Stephens.Sensors
{
    /// <summary>
    /// Class in charge of receiving and resolving visioncast results into DataAIVisionSeenObject data
    /// </summary>
    public static class VisioncastResultsResolver
    {
        #region RESOLVE

        public static List<DataVisionSeenObject> Resolve<T>(
            DataVisioncastResult visioncast, 
            List<DataVisionSeenObject> previous) 
            where T : IVisibleObject
        {
            List<DataVisionSeenObject> resolved = new();
            
            // Cycle through all objects in visioncast
            for (int i = 0; i < visioncast.Objects.Count; i++)
            {
                // Filter by object type
                if (visioncast.Objects[i] is not T)
                    continue;
                
                // Was this object seen?
                if (visioncast.VisiblePoints[i].Count > 0)
                {
                    DataVisionSeenObject? objPreviousData = GetSeenDataForObject(previous, visioncast.Objects[i]);
                    resolved.Add(new DataVisionSeenObject()
                    {
                        ResultObject = visioncast.Objects[i],
                        IsVisible = true,
                        JustBecameVisible = 
                            !DataSeenContainsObject(previous, visioncast.Objects[i]) || objPreviousData is { IsVisible: false },
                        Angle = visioncast.Angles[i],
                        Distance = visioncast.Distances[i]
                    });
                }
                else
                {
                    // This object was not seen...
                    resolved.Add(new DataVisionSeenObject()
                    {
                        ResultObject = visioncast.Objects[i],
                        IsVisible = false,
                        JustBecameVisible = false,
                        Angle = visioncast.Angles[i],
                        Distance = visioncast.Distances[i]
                    });
                }
            }
            
            //
            return resolved;
        }

        public static bool DataSeenContainsObject(List<DataVisionSeenObject> data, IVisibleObject visibleObject)
        {
            foreach (DataVisionSeenObject item in data)
            {
                if (item.ResultObject == visibleObject)
                    return true;
            }

            return false;
        }

        private static DataVisionSeenObject? GetSeenDataForObject(List<DataVisionSeenObject> data, IVisibleObject obj)
        {
            foreach (DataVisionSeenObject item in data)
            {
                if (item.ResultObject == obj)
                    return item;
            }

            return null;
        }

        #endregion RESOLVE
    }
}