using UnityEngine;

namespace GalaxyGourd.Visioncast
{
    /// <summary>
    /// Describes an object that is exposed as "visible" to the visioncast system.
    /// </summary>
    public interface IVisioncastTargetable
    {
        #region PROPERTIES

        Vector3 Position { get; }
        Collider Collider { get; }
        /// <summary>
        /// A collection of "visible points" to which a vision source can raycast (in order to detect object visibility)
        /// </summary>
        Vector3[] VisiblePoints { get; }
        
        #endregion PROPERTIES


        #region METHODS

        void Seen();

        #endregion METHODS
    }
}