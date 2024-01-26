using UnityEngine;

namespace Stephens.Sensors
{
    public interface IVisibleObject
    {
        #region PROPERTIES

        Vector3 Position { get; }
        Collider Collider { get; }
        /// Required to resolve cases where the visible object is seen, but then destroyed before it can be acted upon
        bool IsValidForInteraction { get; } 
        
        /// <summary>
        /// We define a collection of "visible points" to which a vision source can raycast (in order to detect object visibility)
        /// </summary>
        Vector3[] VisiblePoints { get; }
        
        /// <summary>
        /// We need to know the world-space bounds of the object in order to display the interaction tooltip
        /// </summary>
        Bounds WorldBounds { get; }
        
        #endregion PROPERTIES


        #region METHODS

        void Seen();

        #endregion METHODS
    }
}