using System;

namespace Thisaislan.Scriptables.Interfaces
{
    /// <summary>
    /// Defines methods for subscribing to and unsubscribing from a parameterless notification.
    /// </summary>
    public interface ISubscribable
    {
        /// <summary>
        /// Registers a callback to be invoked when the event is triggered.
        /// </summary>
        /// <param name="callback">The callback to register.</param>
        public void Subscribe(Action callback);

        /// <summary>
        /// Unregisters a previously subscribed callback.
        /// </summary>
        /// <param name="callback">The callback to remove.</param>
        public void Unsubscribe(Action callback);
    }

    /// <summary>
    /// Defines methods for subscribing to and unsubscribing from a typed notification.
    /// </summary>
    /// <typeparam name="T">The type of data carried by the notification.</typeparam>
    public interface ISubscribable<T>
    {
        /// <summary>
        /// Registers a callback to be invoked when the event is triggered with data of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="callback">The callback to register.</param>
        public void Subscribe(Action<T> callback);

        /// <summary>
        /// Unregisters a previously subscribed callback.
        /// </summary>
        /// <param name="callback">The callback to remove.</param>
        public void Unsubscribe(Action<T> callback);
    }
}
