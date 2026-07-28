namespace Thisaislan.Scriptables.Interfaces
{
    /// <summary>
    /// Defines a method for setting a value of type <typeparamref name="T"/> without notifying subscribers.
    /// </summary>
    /// <typeparam name="T">The type of data to set silently.</typeparam>
    public interface ISilentSettable<T>
    {
        /// <summary>
        /// Assigns a new value without triggering change notifications.
        /// </summary>
        /// <param name="data">The value to set silently.</param>
        public void SetSilently(T data);
    }
}
