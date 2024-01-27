using System;
using UnityEngine;

namespace Stephens.Tick
{
    /// <summary>
    /// Basic MonoBehaviour tickable implementation, eliminates boilerplate for simple tickables.
    /// Use this as a base class if you don't require any advanced or custom implementation.
    /// </summary>
    public abstract class TickableMonoBehaviour : MonoBehaviour, ITickable
    {
        #region VARIABLES

        public abstract TickGroup TickGroup { get; }

        #endregion VARIABLES


        #region INITIALIZATION

        protected virtual void OnEnable()
        {
            TickRouter.Register(this);
        }

        protected virtual void OnDisable()
        {
            TickRouter.Unregister(this);
        }

        #endregion INITIALIZATION


        #region TICK

        public abstract void Tick(float delta);

        #endregion TICK
    }
}