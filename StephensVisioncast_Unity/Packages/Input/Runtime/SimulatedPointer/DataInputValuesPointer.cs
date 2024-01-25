using UnityEngine;

namespace Stephens.Input
{
    public struct DataInputValuesPointer
    {
        public Vector2 Position;
        public Vector2 Delta;
        public Vector2 Scroll;
        
        public bool SelectStarted;
        public bool SelectIsPressed;
        public bool SelectReleased;
        
        public bool SelectAlternateStarted;
        public bool SelectAlternateIsPressed;
        public bool SelectAlternateReleased;
    }
}