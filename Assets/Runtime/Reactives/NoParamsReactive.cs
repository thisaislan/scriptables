using System;
using Thisaislan.Scriptables.Interfaces;
using Thisaislan.Scriptables.Statics;

#if UNITY_EDITOR
using Thisaislan.Scriptables.Editor.Abstracts;
using Thisaislan.Scriptables.Editor.Utilities;
#else
using UnityEngine;
#endif

namespace Thisaislan.Scriptables.Reactives
{
#if UNITY_EDITOR
    /// <summary>
    /// A lightweight event system that broadcasts parameterless notifications to registered subscribers.
    /// </summary>
    public class NoParamsReactive : ReactiveNoParamsEditorDebuggableScriptable,
        IEmitable, ISubscribable, IPinnable
#else
    /// <summary>
    /// A lightweight event system that broadcasts parameterless notifications to registered subscribers.
    /// </summary>
    public class NoParamsReactive : ScriptableObject,
        IEmitable, ISubscribable, IPinnable
#endif
    {

#if UNITY_EDITOR
        private const string NoSubscribersEditorMessage = "No subscribers registered for {0}";
#endif
        private Action callbacks;

        /// <summary>
        /// Subscribes a callback to be invoked when the event is triggered.
        /// </summary>
        /// <param name="callback">
        /// The delete (callback) to register. This should be a parameterless void method 
        /// that will execute when the event is emitted.
        /// </param>
        public void Subscribe(Action callback)
        {
            callbacks += callback;
        }

        /// <summary>
        /// Unregisters a previously subscribed callback from the event notification system.
        /// </summary>
        /// <param name="callback">
        /// The delegate (callback) to remove from the invocation list.
        /// This should match exactly the method that was originally subscribed.
        /// </param>
        public void Unsubscribe(Action callback)
        {
            callbacks -= callback;
        }

        /// <summary>
        /// Triggers the event notifying all subscribers.
        /// </summary>
        public void Emit()
        {
#if UNITY_EDITOR
            if (trackActivity)
            {
                Printer.PrintMessage($"{name} - {nameof(Emit)}");
            }

            if (callbacks == null)
            {
                Printer.PrintWarning(string.Format(NoSubscribersEditorMessage, name));
                return;
            }
#endif
            callbacks?.Invoke();
        }

        /// <summary>
        /// Keeps the ScriptableObject alive by adding it to the reference list
        /// </summary>
        public void Pin()
        {
#if UNITY_EDITOR
            if (trackActivity)
            {
                Printer.PrintMessage($"{name} - {nameof(Pin)}");
            }
#endif
            ReferenceKeeper.Keep(this);
        }
        
        /// <summary>
        /// Releases the ScriptableObject by removing it from the reference list
        /// </summary>
        public void Unpin()
        {
#if UNITY_EDITOR
            if (trackActivity)
            {
                Printer.PrintMessage($"{name} - {nameof(Unpin)}");
            }
#endif
            ReferenceKeeper.Unkeep(this);
        }
        
#if UNITY_EDITOR
        /// <summary>
        /// Triggers a notification to all subscribers with the current runtime data.
        /// </summary>
        internal override void EmitEditorOnly()
        {
            Emit();
        }
#endif
     }
}