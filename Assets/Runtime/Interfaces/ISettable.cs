namespace Thisaislan.Scriptables.Interfaces
{
    /// <summary>
    /// Defines a method for setting a value of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of data to set.</typeparam>
    public interface ISettable<T>
    {
        /// <summary>
        /// Assigns a new value.
        /// </summary>
        /// <param name="data">The value to set.</param>
        public void Set(T data);
    }
}
