using UnityEngine;
using System;
using Thisaislan.Scriptables.Interfaces;
using Thisaislan.Scriptables.Statics;


#if UNITY_EDITOR
using Thisaislan.Scriptables.Editor.Utilities;
using Thisaislan.Scriptables.Editor.Abstracts;
#endif

namespace Thisaislan.Scriptables.Abstracts
{
#if UNITY_EDITOR
    /// <summary>
    /// A ScriptableObject-based reactive data container that stores a value and notifies subscribers when it changes.
    /// </summary>
    /// <typeparam name="T">The type of data stored and observed by this reactive property</typeparam>
    public abstract class ScriptableReactive<T> : ReactiveEditorDebuggableScriptable,
        ISettable<T>, ISilentSettable<T>, IResettable, IEmitable, ISubscribable<T>, IPinnable
#else
    /// <summary>
    /// A ScriptableObject-based reactive data container that stores a value and notifies subscribers when it changes.
    /// </summary>
    /// <typeparam name="T">The type of data stored and observed by this reactive property</typeparam>
    public abstract class ScriptableReactive<T> : ScriptableObject,
        ISettable<T>, ISilentSettable<T>, IResettable, IEmitable, ISubscribable<T>, IPinnable
#endif
    {   
#if UNITY_EDITOR
        private const string NoSubscribersEditorMessage = "No subscribers registered for {0}";
        private const string RuntimeDataTooltip = "Data used only at runtime. It is reset when exiting Play Mode.";
#endif
        private Action<T> callbacks;

        [SerializeField]
#if UNITY_EDITOR
        [Tooltip(RuntimeDataTooltip)]
#endif
        private T runtimeData;

        /// <summary>
        /// Gets the runtime data. Automatically initializes a new instance if null.
        /// </summary>
        public T Data
        {
            get
            {
                if (runtimeData == null)
                {
                    Reset();
                }
                
                return runtimeData;
            }
        }

        /// <summary>
        /// Assigns a new value to the reactive data without triggering the change event.
        /// </summary>
        /// <param name="data">The value to set without notifying subscribers</param>
        public void SetSilently(T data)
        {
#if UNITY_EDITOR
            if (trackActivity)
            {
                Printer.PrintData($"{name} - {nameof(SetSilently)}", data);
            }
#endif
            runtimeData = data;
        }

        /// <summary>
        /// Sets the data and triggers observer notifications.
        /// </summary>
        /// <param name="data">New data to set</param>
        public void Set(T data)
        {
#if UNITY_EDITOR
            if (trackActivity)
            {
                Printer.PrintData($"{name} - {nameof(Set)}", data);
            }
#endif
            runtimeData = data;

#if UNITY_EDITOR
            TrackableEmit(false);
#else
            Emit();
#endif            
        }

        /// <summary>
        /// Subscribes a callback to be invoked when the event is triggered.
        /// </summary>
        /// <param name="callback">
        /// The delete (callback) to register. This should be a parameterless void method 
        /// that will execute when the event is emitted.
        /// </param>
        public void Subscribe(Action<T> callback)
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
        public void Unsubscribe(Action<T> callback)
        {
            callbacks -= callback;
        }

        /// <summary>
        /// Resets the data to a new default instance.
        /// </summary>
        public virtual void Reset()
        {
            runtimeData = default;
        }

        /// <summary>
        /// Triggers the event for the current generic type, notifying all subscribed.
        /// </summary>
        public void Emit()
        {
#if UNITY_EDITOR
            TrackableEmit(true);
#else
            callbacks?.Invoke(Data);
#endif
        }

        /// <summary>
        /// Keeps the ScriptableObject alive by adding it to the reference list
        /// </summary>
        public void Pin()
        {
#if UNITY_EDITOR
            if (trackActivity)
            {
                Printer.PrintData($"{name} - {nameof(Pin)}", runtimeData);
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
                Printer.PrintData($"{name} - {nameof(Unpin)}", runtimeData);
            }
#endif
            ReferenceKeeper.Unkeep(this);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Resets runtime data to a copy of the current editor data.
        /// Called when entering play mode or when explicitly resetting.
        /// </summary>
        internal override void ResetRuntimeDataEditorOnly()
        {
            runtimeData = default;
        }

        /// <summary>
        /// Gets the current runtime data for editor display.
        /// </summary>
        internal override object GetRuntimeDataEditorOnly()
        {
            if (runtimeData == null)
            {
                ResetRuntimeDataEditorOnly();
            }

            return runtimeData;
        }

        /// <summary>
        /// Sets the runtime data for editor manipulation.
        /// </summary>
        internal override void SetRuntimeDataEditorOnly(object data)
        {
            runtimeData = (T)data;
        }

        /// <summary>
        /// Triggers a notification to all subscribers with the current runtime data.
        /// </summary>
        internal override void EmitEditorOnly()
        {
            TrackableEmit(false);
        }

        /// <summary>
        /// Cleans the object's runtime data to its default state.
        /// </summary>
        internal override void ClearRuntimeDataEditorOnly()
        {
            runtimeData = default;
        }

        private void TrackableEmit(bool checkTracking)
        {
            if (checkTracking)
            {
                if (trackActivity)
                {
                    Printer.PrintData($"{name} - {nameof(Emit)}", Data);
                }
            }

            if (callbacks == null)
            {
                Printer.PrintWarning(string.Format(NoSubscribersEditorMessage, name));
                return;
            }

            callbacks?.Invoke(Data);
        }
#endif
    }
}