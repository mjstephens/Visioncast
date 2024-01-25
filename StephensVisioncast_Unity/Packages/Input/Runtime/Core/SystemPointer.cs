using UnityEngine;
using UnityEngine.EventSystems;

namespace Stephens.Input
{
    public class SystemPointer : IInputPointer
    {
        #region VARIABLES

        Vector3 IInputPointer.Position => UnityEngine.Input.mousePosition;
        bool IInputPointer.IsOverUI => EventSystem.current.IsPointerOverGameObject();
        Camera IInputPointer.Camera => Camera.current;
        InputPointerType IInputPointer.Type => InputPointerType.System;

        #endregion VARIABLES
    }
}