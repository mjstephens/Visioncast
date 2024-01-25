namespace Stephens.Tick
{
    /// <summary>
    /// Default tickable interface. Inheritors who register or unregister with the TickRouter will be added to a queue, which will
    /// add/remove the tickable on the next late update cycle. To ensure tickables are immediately added or removed, use the
    /// ITickableStrict interface.
    /// </summary>
    public interface ITickable
    {
        #region PROPERTIES

        TickGroup TickGroup { get; }

        #endregion PROPERTIES


        #region METHODS

        /// <summary>
        /// Ticks the update for this tickable instance.
        /// </summary>
        /// <param name="delta">Time elapsed (seconds) since previous tick.</param>
        void Tick(float delta);

        #endregion METHODS
    }
}