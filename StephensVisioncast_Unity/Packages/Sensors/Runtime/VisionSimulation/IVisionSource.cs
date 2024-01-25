using Stephens.Utility;
using UnityEngine;

namespace Stephens.Sensors
{
    public interface IVisionSource : ISystemComponent
    {
        #region PROPERTIES

        LayerMask VisionLayer { get; }
        Vector3 Position { get; }
        Vector3 Heading { get; }
        float Range { get; }
        Vector2 FieldOfViewRange { get; } // FoV Increases to Y (max) as the view object is closer
        DataVisioncastResult LastResults { get; }

        #endregion PROPERTIES


        #region METHODS

        void ReceiveResults(DataVisioncastResult data);

        #endregion METHODS
    }
}