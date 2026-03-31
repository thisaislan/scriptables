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
    [CreateAssetMenu(fileName = nameof(NoParametersScriptableReactive), menuName = RuntimeConsts.ScriptableReactiveMenuPath + nameof(NoParametersScriptableReactive), order = 1)]
    /// <summary>
    /// A scriptable event system that broadcasts notifications without parameters.
    /// Register observers using AddObserver, remove them with RemoveObserver, 
    /// and trigger notifications using Notify.
    /// </summary>
#endif
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