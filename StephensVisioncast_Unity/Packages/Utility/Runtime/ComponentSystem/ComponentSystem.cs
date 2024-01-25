using System.Collections.Generic;

namespace Stephens.Utility
{
    /// <summary>
    /// Defines a generic "system" that collects and iterates over a collection of components
    /// </summary>
    public class ComponentSystem<T> where T : ISystemComponent
    {
        #region VARIABLES

        protected readonly List<T> _components = new List<T>();

        #endregion VARIABLES


        #region REGISTRATION

        public virtual void RegisterComponent(T component)
        {
            _components.Add(component);
        }

        public virtual void RemoveComponent(T component)
        {
            _components.Remove(component);
        }

        #endregion REGISTRATION


        #region UTILITY

        protected void ClearAllComponents()
        {
            for (int i = _components.Count - 1; i >= 0; i--)
            {
                _components.RemoveAt(i);
            }
        }

        #endregion UTILITY
    }
}