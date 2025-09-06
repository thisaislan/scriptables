using Thisaislan.Scriptables.Interfaces;
using System;

#if UNITY_EDITOR
using Thisaislan.Scriptables.Editor.Abstracts;
#else
using UnityEngine;
#endif

namespace Thisaislan.Scriptables.Abstracts
{
#if UNITY_EDITOR
    /// <summary>
    /// Base class for ScriptableObject-based runtime data containers with editor debugging support.
    /// In the Unity Editor, this class provides enhanced debugging capabilities through ScriptableEditorDebbugable.
    /// </summary>
    /// <typeparam name="T">The type of data container (must inherit from Data)</typeparam>
    /// <remarks>
    /// This implementation provides:
    /// - Runtime data management with reset capabilities
    /// - Custom inspector support for enhanced debugging
    /// - Data reset functionality through IDataResettable
    /// - Editor-time visualization of runtime data structure
    /// - Automatic data initialization on access
    /// </remarks>
    public abstract class ScriptableRuntime<T> : ScriptableEditorDebbugable, IDataResettable where T : Data
#else
    /// <summary>
    /// Base class for ScriptableObject-based runtime data containers for builds.
    /// In builds, this uses the standard ScriptableObject without editor-specific features.
    /// </summary>
    /// <typeparam name="T">The type of data container (must inherit from Data)</typeparam>
    /// <remarks>
    /// This lightweight implementation provides runtime-only functionality
    /// with automatic data initialization and reset capabilities.
    /// </remarks>
    public abstract class ScriptableRuntime<T> : ScriptableObject, IDataResettable where T : Data
#endif
    {
        [NonSerialized] // Runtime data is not serialized to prevent persistence between sessions
        private T data; // Instance of the runtime data

        /// <summary>
        /// Public accessor for the runtime data with automatic initialization
        /// </summary>
        /// <remarks>
        /// The data is automatically initialized to a default instance when first accessed
        /// if it hasn't been set already. This ensures the data is always available.
        /// </remarks>
        public T Data
        {
            get
            {
                // Initialize data if it hasn't been set yet (lazy initialization)
                if (data == null)
                {
                    ResetData();
                }

                return data;
            }

            /// <summary>
            /// Protected setter for the runtime data
            /// </summary>
            /// <remarks>
            /// Allows derived classes to set the data while preventing external modification
            /// </remarks>
            protected set
            {
                data = value;
            }
        }

        /// <summary>
        /// Resets the runtime data to a new default instance
        /// </summary>
        /// <remarks>
        /// This method is part of the IDataResettable interface and provides
        /// a way to reset the data to its initial state. Uses Activator.CreateInstance
        /// to create a new instance of the data type T.
        /// </remarks>
        public virtual void ResetData()
        {
            Data = (T)Activator.CreateInstance(typeof(T));
        }

#if UNITY_EDITOR

        internal override void SetData(Data data)
        {
            this.data = (T)data;
        }


        /// <summary>
        /// Resets the runtime data to its default state
        /// </summary>
        /// <remarks>
        /// This implementation calls ResetData() to create a new instance of the data.
        /// Used by the custom editor system to provide reset functionality.
        /// </remarks>
        internal override void ResetToDefaultState()
        {
            // For ScriptableRuntime, we want to reset the data to a fresh instance
            ResetData();
        }

        /// <summary>
        /// Gets the current runtime data instance for editor inspection
        /// </summary>
        /// <returns>The current runtime data instance</returns>
        /// <remarks>
        /// Used by the custom editor to display the runtime data in the inspector.
        /// Returns the actual runtime data instance for debugging purposes.
        /// </remarks>
        internal override object GetData()
        {
            if (data == null)
            {
                ResetToDefaultState();
            }

            return data;
        }

        /// <summary>
        /// Returns the type of ScriptableEditorDebbugable for editor categorization
        /// </summary>
        /// <returns>ScriptableEditorDebbugableType.Settings to indicate this is a settings-like object</returns>
        /// <remarks>
        /// Used by the custom editor to determine how to display and handle this object.
        /// Although this is runtime data, it's categorized as Settings for consistent editor treatment.
        /// </remarks>
        internal override ScriptableEditorDebbugableType GetScriptableEditorDebbugableType()
        {
            return ScriptableEditorDebbugableType.Runtime;
        }

        /// <summary>
        /// Gets the type of the value for editor display purposes
        /// </summary>
        internal override Type GetValueType()
        {
            return GetType();
        }
#endif
    }
}