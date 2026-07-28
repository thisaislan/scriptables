namespace Thisaislan.Scriptables.Interfaces
{
    /// <summary>
    /// Defines a method for initializing an object with data of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of data used for initialization.</typeparam>
    public interface IInitializable<T>
    {
        /// <summary>
        /// Initializes the object with the specified data.
        /// </summary>
        /// <param name="data">The data to initialize with.</param>
        public void Initialize(T data);
    }
}
