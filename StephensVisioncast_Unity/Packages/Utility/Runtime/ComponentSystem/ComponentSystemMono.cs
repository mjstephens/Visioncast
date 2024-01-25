using System.Collections.Generic;
using UnityEngine;

namespace Stephens.Utility
{
    public class ComponentSystemMono<T> : MonoBehaviour
        where T : ISystemComponent
    {
        #region VARIABLES

        /// <summary>
        /// The list of currently registered components for this system
        /// </summary>
        protected readonly List<T> _components = new List<T>();

        #endregion VARIABLES


        #region REGISTRATION

        public void RegisterComponent(T component)
        {
            _components.Add(component);
        }

        public void RemoveComponent(T component)
        {
            _components.Remove(component);
        }

        #endregion REGISTRATION
    }
}