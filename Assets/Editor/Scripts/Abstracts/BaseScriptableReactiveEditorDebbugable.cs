#if UNITY_EDITOR
using System;
using Thisaislan.Scriptables.Editor.Utilities;
using UnityEngine;

namespace Thisaislan.Scriptables.Editor.Abstracts
{
    /// <summary>
    /// Non-generic base class for ScriptableReactive objects that require editor debugging capabilities.
    /// This provides a common interface for editor tools to interact with different generic implementations.
    /// </summary>
    public abstract class BaseScriptableReactiveEditorDebbugable : ScriptableObject
    {

        /// <summary>
        /// Internal constructor to prevent external inheritance
        /// </summary>
        /// <remarks>
        /// This constructor ensures that only internal classes within the assembly
        /// can inherit from BaseScriptableReactiveEditorDebbugable, maintaining control over
        /// the inheritance hierarchy.
        /// </remarks>
        internal BaseScriptableReactiveEditorDebbugable()
        {
            // Avoid external heritage - only allow internal inheritance
        }

        // These internal methods allow the custom editor to get and set values without knowing <T>.
        internal abstract object GetRuntimeValue(); // Gets the current value during Play Mode.
        internal abstract object GetEditorValue();  // Gets the current value in Edit Mode.
        internal abstract Type GetValueType();      // Returns the type (T) of the value this object holds.
        internal abstract void ResetToDefaultState(); // Resets the object to its initial value.
        internal abstract void SetRuntimeValue(object value); // Sets the value during Play Mode.
        internal abstract void SetEditorValue(object value);  // Sets the value in Edit Mode.
        internal abstract void NotifyValue();  // Notify changes in Edit Mode.

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
            string typeName = TypeNameSimplifier.SimplifyTypeName(GetRuntimeValue().GetType().Name);

            Printer.PrintMessage($"{GetType().Name}<{typeName}>:\n{GetStringValue()}");
        }

        /// <summary>
        /// Converts the object's data to a formatted string representation
        /// </summary>
        /// <returns>A string representation of the object's data</returns>
        /// <remarks>
        /// Handles different types appropriately:
        /// - Primitives: Direct string conversion
        /// - Enums: Name of the enum value
        /// - Classes: JSON serialized form
        /// </remarks>
        internal virtual string GetStringValue()
        {
            object value = GetRuntimeValue();
            if (value == null) return Consts.NullLiteral;
            
            Type valueType = value.GetType();
            
            // Handle primitives and strings
            if (valueType.IsPrimitive || valueType == typeof(string))
            {
                return value.ToString();
            }
            
            // Handle enums
            if (valueType.IsEnum)
            {
                return value.ToString();
            }
            
            // Handle Unity objects
            if (typeof(UnityEngine.Object).IsAssignableFrom(valueType))
            {
                UnityEngine.Object unityObj = (UnityEngine.Object)value;
                return unityObj != null ? unityObj.name : Consts.NullLiteral;
            }
            
            // For other types, use JSON serialization
            try
            {
                return JsonUtility.ToJson(value, true);
            }
            catch (Exception e)
            {
                return $"Serialization Error: {e.Message}";
            }
        }
    }
}
#endif