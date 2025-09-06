using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using Thisaislan.Scriptables.Editor.Abstracts;
using Thisaislan.Scriptables.Editor;
using System;
#endif

namespace Thisaislan.Scriptables.Abstracts
{
#if UNITY_EDITOR
    /// <summary>
    /// Base class for creating reactive scriptable objects that combine data storage with event notifications.
    /// Provides editor/runtime value separation and change notification capabilities.
    /// </summary>
    /// <typeparam name="T">The type of value to store and observe</typeparam>
    /// <remarks>
    /// In the editor, this class provides enhanced debugging capabilities through ScriptableReactiveEditorDebbugable.
    /// In builds, it functions as a standard ScriptableObject with reactive event functionality.
    /// </remarks>
    public abstract class ScriptableReactive<T> : ScriptableReactiveEditorDebbugable<T>
#else
    /// <summary>
    /// Base class for creating reactive scriptable objects that combine data storage with event notifications.
    /// </summary>
    /// <typeparam name="T">The type of value to store and observe</typeparam>
    public abstract class ScriptableReactive<T> : ScriptableObject
#endif
    {
        [SerializeField]
        private UnityEvent<T> evt;

        [SerializeField]
#if UNITY_EDITOR
        [Tooltip(Consts.EditorValueTooltip)]
#endif
        private T editorValue;

#if UNITY_EDITOR
        [NonSerialized]
        private T value;
#endif

        /// <summary>
        /// Gets or sets the current value, automatically notifying all observers when changed
        /// </summary>
        /// <remarks>
        /// In the editor:
        /// - During edit mode: Uses and modifies the serialized editorValue
        /// - During play mode: Uses and modifies a runtime copy, preserving editorValue
        /// 
        /// In builds:
        /// - Always uses the serialized editorValue
        /// 
        /// Setting this property automatically triggers notifications to all registered observers.
        /// </remarks>
        public T Value
        {
            get
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    if (value == null)
                    {
                        ResetToDefaultState();
                    }

                    return value;
                }
                else
                {
                    return editorValue;
                }
#else
                return editorValue;
#endif
            }
            set
            {
                SetWithoutNotify(value);
                Notify(value);
            }
        }

        /// <summary>
        /// Set value without notify
        /// </summary>
        /// <param name="value">New value to be set</param>
        public void SetWithoutNotify(T value)
        {
#if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    this.value = value;
                }
                else
                {
                    editorValue = value;
                }
#else
                editorValue = value;
#endif
        }

        /// <summary>
        /// Set value with notification
        /// </summary>
        /// <param name="value">New value to be set</param>
        public void Set(T value)
        {
            Value = value;
        }

        /// <summary>
        /// Registers a callback to be invoked when the value changes
        /// </summary>
        /// <param name="call">The method to call when the value changes</param>
        /// <example>
        /// <code>
        /// myReactive.AddObserver(OnValueChanged);
        /// 
        /// private void OnValueChanged(MyType newValue)
        /// {
        ///     Debug.Log($"Value changed to: {newValue}");
        /// }
        /// </code>
        /// </example>
        public void AddObserver(UnityAction<T> call)
        {
            evt.AddListener(call);
        }

        /// <summary>
        /// Unregisters a callback from value change notifications
        /// </summary>
        /// <param name="call">The method to remove from notifications</param>
        public void RemoveObserver(UnityAction<T> call)
        {
            evt.RemoveListener(call);
        }

        /// <summary>
        /// Notifies all registered observers with the specified value
        /// </summary>
        /// <param name="value">The value to send to all observers</param>
        /// <remarks>
        /// This method is automatically called when the Value property is set.
        /// It can also be called manually to trigger notifications without changing the stored value.
        /// </remarks>
        public void Notify(T value)
        {
            evt.Invoke(value);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Resets the runtime value to match the current editor value
        /// </summary>
        /// <remarks>
        /// This method is called automatically when entering play mode to ensure
        /// runtime data starts with a fresh copy of editor data.
        /// </remarks>
        internal override void ResetToDefaultState()
        {
            if (editorValue != null)
            {
                value = CreateCopy(editorValue);
            }
            else
            {
                value = default;
            }
        }

        /// <summary>
        /// Gets the current runtime value for editor display purposes
        /// </summary>
        /// <returns>The runtime value instance</returns>
        /// <remarks>
        /// If the runtime value hasn't been initialized, this method will create
        /// a copy of the editor value before returning.
        /// </remarks>
        internal override object GetRuntimeValue()
        {
            if (value == null)
            {
                ResetToDefaultState();
            }

            return value;
        }

        /// <summary>
        /// Gets the current editor value for editor display purposes
        /// </summary>
        /// <returns>The editor value instance</returns>
        internal override object GetEditorValue()
        {
            return editorValue;
        }

        /// <summary>
        /// Sets the runtime value for editor manipulation
        /// </summary>
        /// <param name="value">The new runtime value</param>
        internal override void SetRuntimeValue(object value)
        {
            this.value = (T)value;
        }

        /// <summary>
        /// Sets the editor value for editor manipulation
        /// </summary>
        /// <param name="value">The new editor value</param>
        internal override void SetEditorValue(object value)
        {
            editorValue = (T)value;
        }

        /// <summary>
        /// Used to call Notify method using the runtime value
        /// </summary>
        internal override void NotifyValue()
        {
            Notify(Value);
        }
#endif
    }
}