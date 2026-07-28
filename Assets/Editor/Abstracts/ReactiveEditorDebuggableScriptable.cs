#if UNITY_EDITOR
using Thisaislan.Scriptables.Editor.Abstracts.Bases;

namespace Thisaislan.Scriptables.Editor.Abstracts
{
    /// <summary>
    /// Generic base class for reactive ScriptableObjects with editor debugging capabilities.
    /// Provides common implementation for types that need to be observed and debugged in the editor.
    /// </summary>
    public abstract class ReactiveEditorDebuggableScriptable : BaseEditorDebuggableTransientScriptable
    {
        /// <summary>
        /// Internal constructor to prevent external inheritance.
        /// Only internal classes within the assembly can inherit from this class.
        /// </summary>
        internal ReactiveEditorDebuggableScriptable()
        {
            // Avoid external heritage - only allow internal inheritance
        }

        /// <summary>
        /// Triggers the event notifying all subscribers with current data.
        /// </summary>
        internal abstract void EmitEditorOnly();
    }
}
#endif