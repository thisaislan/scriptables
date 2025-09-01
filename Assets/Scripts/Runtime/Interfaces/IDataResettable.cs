namespace Thisaislan.Scriptables.Interfaces
{
    /// <summary>
    /// Interface for objects that support resetting their data to a default state.
    /// </summary>
    public interface IDataResettable
    {
        /// <summary>
        /// Resets the object's data to its default state.
        /// </summary>
        public abstract void ResetData();
    }
}