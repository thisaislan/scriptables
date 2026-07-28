namespace Thisaislan.Scriptables.Interfaces
{
    /// <summary>
    /// Defines a method for triggering a notification to all subscribers.
    /// </summary>
    public interface IEmitable
    {
        /// <summary>
        /// Triggers the event, notifying all registered subscribers.
        /// </summary>
        public void Emit();
    }
}
