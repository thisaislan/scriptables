using Thisaislan.Scriptables.Interfaces;
using UnityEngine;

#if UNITY_EDITOR
using System.Reflection;
using Thisaislan.Scriptables.Editor;
using Thisaislan.Scriptables.Editor.Abstracts;
using System;
#endif

namespace Thisaislan.Scriptables.Abstracts
{
#if UNITY_EDITOR
    /// <summary>
    /// Base class for ScriptableObject-based settings that supports both editor and runtime data management.
    /// In the Unity Editor, this class provides enhanced debugging capabilities through ScriptableEditorDebbugable.
    /// </summary>
    /// <typeparam name="T">The type of data container (must inherit from Data)</typeparam>
    /// <remarks>
    /// This implementation provides:
    /// - Separate editor and runtime data instances
    /// - Automatic data copying from editor to runtime when entering play mode
    /// - Custom inspector support for enhanced debugging
    /// - Data initialization capabilities through IDataInitializable
    /// - Editor-only features for development and testing
    /// </remarks>
    public abstract class ScriptableSettings<T> : ScriptableEditorDebbugable, IDataInitializable where T : Data
#else
    /// <summary>
    /// Base class for ScriptableObject-based settings for runtime builds.
    /// In builds, this uses the standard ScriptableObject without editor-specific features.
    /// </summary>
    /// <typeparam name="T">The type of data container (must inherit from Data)</typeparam>
    /// <remarks>
    /// This lightweight implementation provides runtime-only functionality
    /// without the overhead of editor-specific features.
    /// </remarks>
    public abstract class ScriptableSettings<T> : ScriptableObject, IDataInitializable where T : Data
#endif
    {
        [SerializeField]
#if UNITY_EDITOR
        [Tooltip(Consts.EditorDataTooltip)]
#endif
        private T editorData; // Serialized editor data that persists between sessions

#if UNITY_EDITOR
        [NonSerialized]
        private T data; // Runtime instance of the data used during play mode
#endif        

        /// <summary>
        /// Public accessor for the settings data with proper editor/runtime separation
        /// </summary>
        /// <remarks>
        /// In the editor:
        /// - During edit mode: Returns the serialized editorData
        /// - During play mode: Returns a runtime copy of editorData
        /// 
        /// In builds:
        /// - Always returns the serialized editorData
        /// </remarks>
        public T Data
        {
            get
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    // Use runtime data during play mode for isolation from editor changes
                    if (data == null)
                    {
                        ResetToDefaultState();
                    }

                    return data;
                }
                else
                {
                    // Use editor data during edit mode for direct editing
                    return editorData;
                }
#else
                // In builds, always use editor data (no runtime/editor separation needed)
                return editorData;
#endif
            }
            protected set
            {
                InitializeData(value);
            }
        }

        /// <summary>
        /// Initializes the settings data with a new Data instance
        /// </summary>
        /// <param name="data">The data to initialize with (must be of type T)</param>
        /// <remarks>
        /// This method is part of the IDataInitializable interface and allows
        /// external systems to set the data for this ScriptableSettings instance.
        /// </remarks>
        public virtual void InitializeData(Data data)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                // Only set runtime data during play mode to maintain separation
                this.data = data as T;
            }
            else
            {
                // Set editor data during edit mode for persistence
                editorData = data as T;
            }
#else
                // In builds, set editor data directly
                editorData = data as T;
#endif
        }

#if UNITY_EDITOR

        internal override void SetData(Data data)
        {
            this.data = (T)data;
        }
        /// <summary>
        /// Resets the runtime data to match the current editor data
        /// </summary>
        /// <remarks>
        /// This is called when entering play mode or when explicitly resetting
        /// to ensure runtime data starts with a fresh copy of editor data
        /// </remarks>
        internal override void ResetToDefaultState()
        {
            // For ScriptableSettings, we want to clone the editor data for runtime
            if (editorData != null)
            {
                data = CreateCopy(editorData);
            }
            else
            {
                data = default;
            }
        }

        /// <summary>
        /// Gets the appropriate data instance based on the current mode
        /// </summary>
        /// <returns>
        /// - During play mode: The runtime data instance
        /// - During edit mode: The editor data instance
        /// </returns>
        /// <remarks>
        /// Used by the custom editor to display the correct data in the inspector
        /// </remarks>
        internal override object GetData()
        {
            if (Application.isPlaying)
            {
                if (data == null)
                {
                    ResetToDefaultState();
                }

                return data;
            }
            else
            {
                return editorData;
            }
        }

        /// <summary>
        /// Returns the type of ScriptableEditorDebbugable for editor categorization
        /// </summary>
        /// <returns>ScriptableEditorDebbugableType.Settings to indicate this is a settings object</returns>
        /// <remarks>
        /// Used by the custom editor to determine how to display and handle this object
        /// </remarks>
        internal override ScriptableEditorDebbugableType GetScriptableEditorDebbugableType()
        {
            return ScriptableEditorDebbugableType.Settings;
        }

        /// <summary>
        /// Gets the type of the value for editor display purposes
        /// </summary>
        internal override Type GetValueType()
        {
            return GetType();
        }

        /// <summary>
        /// Creates a deep copy of a Data object using reflection
        /// </summary>
        /// <typeparam name="T">The type of Data object to copy</typeparam>
        /// <param name="original">The original object to copy</param>
        /// <returns>A new instance of T with copied field and property values</returns>
        /// <remarks>
        /// This method:
        /// - Creates a new instance using the default constructor
        /// - Copies all field and property values from the original
        /// - Handles null originals by creating a default instance
        /// - Uses CopyObjectData to perform the actual copying
        /// </remarks>
        private T CreateCopy(T original)
        {
            try
            {
                // Handle null original by creating a default instance
                if (original == null) return (T)Activator.CreateInstance(typeof(T));

                // Create new instance and copy data from original
                T copy = (T)Activator.CreateInstance(typeof(T));
                CopyObjectData(original, copy);
                return copy;
            }
            catch
            {
                return default;
            }
        }
        
        /// <summary>
        /// Copies all field and property values from source object to destination object
        /// </summary>
        /// <param name="source">The source object to copy data from</param>
        /// <param name="destination">The destination object to copy data to</param>
        /// <remarks>
        /// This method performs a shallow copy of all public instance fields and properties.
        /// Only copies data if both objects are non-null and of the same type.
        /// Includes error handling to prevent failures from individual field/property access issues.
        /// </remarks>
        private void CopyObjectData(object source, object destination)
        {
            // Early return for null objects to prevent exceptions
            if (source == null || destination == null)
                return;

            Type sourceType = source.GetType();
            Type destinationType = destination.GetType();

            // Only copy if types match to ensure type safety
            if (sourceType != destinationType)
                return;

            // Copy all public instance fields
            FieldInfo[] fields = sourceType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
            {
                try
                {
                    object value = field.GetValue(source);
                    field.SetValue(destination, value);
                }
                catch
                {
                    // Log warning but continue with other fields
                }
            }

            // Copy all public instance properties with both get and set access
            PropertyInfo[] properties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo property in properties)
            {
                // Only copy properties that can be both read and written
                if (property.CanRead && property.CanWrite)
                {
                    try
                    {
                        object value = property.GetValue(source);
                        property.SetValue(destination, value);
                    }
                    catch
                    {
                        // Log warning but continue with other properties
                    }
                }
            }
        }
#endif
    }
}