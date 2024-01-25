using UnityEngine;

namespace Stephens.Input
{
    public interface IInputPointer
    {
        #region PROPERTIES

        Vector3 Position { get; }
        bool IsOverUI { get; }
        Camera Camera { get; }
        InputPointerType Type { get; }

        #endregion PROPERTIES


        #region METHODS

        Vector2 GetPointerPosForViewportRaycast()
        {
            return Camera.WorldToViewportPoint(Position);
        }

        Vector3 GetPointerPosForScreen()
        {
            return Camera.WorldToScreenPoint(Position);
        }

        #endregion METHODS
    }
}