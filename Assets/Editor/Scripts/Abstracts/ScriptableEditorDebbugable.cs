#if UNITY_EDITOR
using System;
using Thisaislan.Scriptables.Abstracts;
using Thisaislan.Scriptables.Editor.Utilities;
using UnityEngine;

namespace Thisaislan.Scriptables.Editor.Abstracts
{
    /// <summary>
    /// Base class for scriptable objects with editor-specific debugging capabilities.
    /// Provides a foundation for enhanced editor inspection and debugging features.
    /// </summary>
    /// <remarks>
    /// This abstract class enables:
    /// - Custom inspector rendering with debugging capabilities
    /// - Runtime data visualization and editing
    /// - Data reset functionality
    /// - JSON serialization for debugging
    /// - Type categorization for different editor behaviors
    /// </remarks>
    public abstract class ScriptableEditorDebbugable : ScriptableObject
    {
        /// <summary>
        /// Enumeration defining the types of debuggable scriptable objects
        /// </summary>
        /// <remarks>
        /// Used by the custom editor to determine appropriate display and behavior:
        /// - Settings: Typically used for configuration data with editor/runtime separation
        /// - Runtime: Used for runtime-only data containers
        /// </remarks>
        internal enum ScriptableEditorDebbugableType
        {
            Settings,
            Runtime
        }

        /// <summary>
        /// Internal constructor to prevent external inheritance
        /// </summary>
        /// <remarks>
        /// This constructor ensures that only internal classes within the assembly
        /// can inherit from ScriptableEditorDebbugable, maintaining control over
        /// the inheritance hierarchy.
        /// </remarks>
        internal ScriptableEditorDebbugable()
        {
            // Avoid external heritage - only allow internal inheritance
        }

        /// <summary>
        /// Unity callback called when the ScriptableObject is enabled
        /// </summary>
        /// <remarks>
        /// Ensures proper state management when the object is loaded or created.
        /// Resets the data to default state when not in play mode to provide
        /// a clean starting point for editor inspection.
        /// </remarks>
        protected virtual void OnEnable()
        {
            // Reset runtime data when not in play mode to ensure clean state for editor inspection
            if (!Application.isPlaying)
            {
                ResetToDefaultState();
            }
        }

        /// <summary>
        /// Prints the object's data to the Unity console in a formatted JSON representation
        /// </summary>
        /// <remarks>
        /// This method provides a convenient way to debug the current state of the object's data.
        /// The output is formatted as JSON for readability and includes type information.
        /// Uses the Printer utility for clean console output without stack traces.
        /// </remarks>
        public virtual void PrintDataDebugEditor()
        {
            // Format type name for better readability and print with JSON data
            string formattedTypeName = GetData().GetType().ToString().Replace("+", "<");
            Printer.PrintMessage($"{formattedTypeName}>\n{GetStringData()}");
        }

        /// <summary>
        /// Converts the object's data to a formatted JSON string
        /// </summary>
        /// <returns>A JSON string representation of the object's data</returns>
        /// <remarks>
        /// This method uses Unity's JsonUtility for serialization with pretty-print formatting.
        /// Override this method in derived classes for custom serialization behavior.
        /// </remarks>
        internal virtual string GetStringData()
        {
            return JsonUtility.ToJson(GetData(), true);
        }

        /// <summary>
        /// Resets the object's data to its default state
        /// </summary>
        /// <remarks>
        /// Abstract method that must be implemented by derived classes.
        /// Typically used to initialize or revert data to a known default state.
        /// </remarks>
        internal abstract void ResetToDefaultState();

        /// <summary>
        /// Retrieves the object's data for inspection and editing
        /// </summary>
        /// <returns>The data object contained by this scriptable object</returns>
        /// <remarks>
        /// Abstract method that must be implemented by derived classes.
        /// Used by the custom editor to access the data for display and modification.
        /// </remarks>
        internal abstract object GetData();

        /// <summary>
        /// Gets the type of the value for editor display purposes
        /// </summary>
        internal abstract Type GetValueType();

        /// <summary>
        /// Returns the type of debuggable scriptable object
        /// </summary>
        /// <returns>The ScriptableEditorDebbugableType categorization</returns>
        /// <remarks>
        /// Abstract method that must be implemented by derived classes.
        /// Used by the custom editor to determine appropriate display and behavior.
        /// </remarks>
        internal abstract ScriptableEditorDebbugableType GetScriptableEditorDebbugableType();

        /// <summary>
        /// Sets the internal data for the implementing class.
        /// </summary>
        /// <param name="data">The <see cref="Data"/> object containing the information to be set.</param>
        /// <remarks>
        /// This method is abstract and must be implemented by a derived class.
        /// It is intended for internal use only.
        /// </remarks>
        internal abstract void SetData(Data data);
    }
}
#endif