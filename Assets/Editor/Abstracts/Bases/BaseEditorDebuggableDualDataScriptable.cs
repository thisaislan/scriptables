#if UNITY_EDITOR
using UnityEngine;

namespace Thisaislan.Scriptables.Editor.Abstracts.Bases
{
    /// <summary>
    /// Base class for ScriptableObjects with editor-specific debugging capabilities with dual data scriptable.
    /// </summary>
    public abstract class BaseEditorDebuggableDualDataScriptable : BaseEditorDebuggableTransientScriptable
    {
        /// <summary>
        /// Internal constructor to prevent external inheritance.
        /// Only internal classes within the assembly can inherit from this class.
        /// </summary>
        /// 
        internal BaseEditorDebuggableDualDataScriptable()
        {
            // Avoid external heritage - only allow internal inheritance
        }

        /// <summary>
        /// Gets the current data object for editor inspection and modification.
        /// </summary>
        /// <returns>The data object contained by this ScriptableObject</returns>
        internal abstract object GetDataEditorOnly();

        /// <summary>
        /// Resets the object's data to its default state.
        /// </summary>
        internal abstract void ResetDataEditorOnly();

        /// <summary>
        /// Creates a shallow copy of a value.
        /// Returns the original for value types, strings, and UnityEngine.Objects.
        /// Uses JSON serialization for serializable reference types.
        /// </summary>
        /// <typeparam name="T">The type of value to copy</typeparam>
        /// <param name="original">The original value to copy</param>
        /// <returns>A copy of the value, or the original if copying is not possible</returns>
        protected T CreateCopy<T>(T original)
        {
            if (typeof(T).IsValueType || typeof(T) == typeof(string))
            {
                return original;
            }
            
            if (typeof(Object).IsAssignableFrom(typeof(T)))
            {
                return original;
            }
            
            try
            {
                if (original != null && original.GetType().IsSerializable)
                {
                    string json = JsonUtility.ToJson(original);
                    
                    return JsonUtility.FromJson<T>(json);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Failed to create copy via JSON serialization: {e.Message}");

            }
            
            return original;
        }
    }
}
#endif