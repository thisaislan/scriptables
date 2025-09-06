using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using Thisaislan.Scriptables.Editor;
#endif

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR
    /// <summary>
    /// Creates a menu entry for creating NoParametersScriptableReactive assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(NoParametersScriptableReactive), menuName = Consts.ScriptablesMenuPath + nameof(NoParametersScriptableReactive), order = 1)]
#endif
    /// <summary>
    /// A scriptable event system that broadcasts notifications without parameters.
    /// Register observers using AddObserver, remove them with RemoveObserver, 
    /// and trigger notifications using Notify.
    /// </summary>
    /// <remarks>
    /// This implementation provides a lightweight event system without payload data.
    /// Ideal for simple notification systems and game state changes where only the
    /// event occurrence matters, not specific data.
    /// </remarks>
    public class NoParametersScriptableReactive : ScriptableObject
    {
        [SerializeField]
        private UnityEvent evt;

        /// <summary>
        /// Registers a callback to be invoked when the event is triggered
        /// </summary>
        /// <param name="call">The method to call when the event is triggered</param>
        /// <example>
        /// <code>
        /// myEvent.AddObserver(OnEventTriggered);
        /// 
        /// private void OnEventTriggered()
        /// {
        ///     Debug.Log("Event was triggered!");
        /// }
        /// </code>
        /// </example>
        public void AddObserver(UnityAction call)
        {
            evt.AddListener(call);
        }

        /// <summary>
        /// Unregisters a callback from event notifications
        /// </summary>
        /// <param name="call">The method to remove from notifications</param>
        public void RemoveObserver(UnityAction call)
        {
            evt.RemoveListener(call);
        }

        /// <summary>
        /// Notifies all registered observers that the event has occurred
        /// </summary>
        /// <remarks>
        /// This method triggers all registered callbacks without passing any data.
        /// Use this for events where only the notification matters, not specific values.
        /// </remarks>
        public void Notify()
        {
            evt.Invoke();
        }
     }
}