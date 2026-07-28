#if UNITY_EDITOR
namespace Thisaislan.Scriptables.Editor.Abstracts.Bases
{
    /// <summary>
    /// Base class for settings ScriptableObjects with editor debugging capabilities.
    /// Provides data access, serialization, and debug printing functionality for settings.
    /// </summary>
    public abstract class SettingsEditorDebuggableScriptable : BaseEditorDebuggableDualDataScriptable
    {
        /// <summary>
        /// Internal constructor to prevent external inheritance.
        /// Only internal classes within the assembly can inherit from this class.
        /// </summary>
        internal SettingsEditorDebuggableScriptable()
        {
            // Avoid external heritage - only allow internal inheritance
        }
    }
}
#endif