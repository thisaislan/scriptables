#if UNITY_EDITOR
using UnityEngine;

namespace Thisaislan.Scriptables.Editor.Abstracts.Bases
{
    /// <summary>
    /// Base class for ScriptableObjects with editor-specific debugging capabilities.
    /// </summary>
    public abstract class BaseEditorDebuggableScriptable : ScriptableObject
    {
        /// <summary>
        /// Add a note to describe this Scriptable's role. Only appears in the Editor.
        /// </summary>
        [SerializeField]
        internal string description = string.Empty;

        /// <summary>
        /// When enabled, logs major scriptable events such as data changes, emissions (for reactive types), and pinning operations. 
        /// Useful for monitoring changes and events during development.
        /// </summary>
        [SerializeField]
        internal bool trackActivity = false;
    }
}
#endif