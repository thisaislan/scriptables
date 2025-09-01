using Thisaislan.Scriptables.Abstracts;

namespace Thisaislan.Scriptables.Interfaces
{
    /// <summary>
    /// Interface for objects that can be initialized with a <see cref="Data"/> instance.
    /// </summary>
    public interface IDataInitializable
    {
        /// <summary>
        /// Initializes the object using the provided <see cref="Data"/> instance.
        /// </summary>
        /// <param name="data">The data used for initialization.</param>
        public abstract void InitializeData(Data data);
    }
}