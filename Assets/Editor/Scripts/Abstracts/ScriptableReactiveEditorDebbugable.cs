#if UNITY_EDITOR
using System;
using UnityEngine;

namespace Thisaislan.Scriptables.Editor.Abstracts
{
    /// <summary>
    /// Generic base class for scriptable reactive objects with editor debugging capabilities.
    /// This provides common implementation for types that need to be observed and debugged in the editor.
    /// </summary>
    /// <typeparam name="T">The type of value this ScriptableObject holds and reacts to.</typeparam>
    public abstract class ScriptableReactiveEditorDebbugable<T> : BaseScriptableReactiveEditorDebbugable
    {

        /// <summary>
        /// Internal constructor to prevent external inheritance
        /// </summary>
        /// <remarks>
        /// This constructor ensures that only internal classes within the assembly
        /// can inherit from ScriptableReactiveEditorDebbugable, maintaining control over
        /// the inheritance hierarchy.
        /// </remarks>
        internal ScriptableReactiveEditorDebbugable()
        {
            // Avoid external heritage - only allow internal inheritance
        }

        /// <summary>
        /// Unity message called when the scriptable object is loaded or created.
        /// Ensures the object is reset to its default state when entering Edit Mode.
        /// </summary>
        protected virtual void OnEnable()
        {
            // Only reset if NOT in Play Mode. This prevents overwriting runtime values when the editor recompiles.
            if (!Application.isPlaying)
            {
                ResetToDefaultState();
            }
        }

        /// <summary>
        /// Creates a copy of the value, handling reference types appropriately
        /// </summary>
        protected T CreateCopy(T original)
        {
            // For value types and strings, just return the value
            if (typeof(T).IsValueType || typeof(T) == typeof(string))
            {
                return original;
            }
            
            // For UnityEngine.Object types, return the reference directly (no copying)
            if (typeof(UnityEngine.Object).IsAssignableFrom(typeof(T)))
            {
                return original;
            }
            
            // For other reference types, try to create a copy using JSON serialization
            try
            {
                if (original != null && original.GetType().IsSerializable)
                {
                    // Use JsonUtility as a simple way to create a deep copy for serializable types
                    string json = JsonUtility.ToJson(original);
                    return JsonUtility.FromJson<T>(json);
                }
            }
            catch
            {
            }
            
            return original;
        }

        /// <summary>
        /// Return value type
        /// </summary>
        internal override Type GetValueType()
        {
            return typeof(T);
        }
    }
}
#endif