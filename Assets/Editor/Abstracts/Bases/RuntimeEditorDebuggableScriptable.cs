#if UNITY_EDITOR
namespace Thisaislan.Scriptables.Editor.Abstracts.Bases
{
    /// <summary>
    /// Base class for runtime ScriptableObjects with editor debugging capabilities.
    /// Provides data access, serialization, and debug printing functionality.
    /// </summary>
    public abstract class RuntimeEditorDebuggableScriptable : BaseEditorDebuggableTransientScriptable
    {
        /// <summary>
        /// Internal constructor to prevent external inheritance.
        /// Only internal classes within the assembly can inherit from this class.
        /// </summary>
        internal RuntimeEditorDebuggableScriptable()
        {
            // Avoid external heritage - only allow internal inheritance
        }
    }
}
#endif