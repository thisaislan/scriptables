#if UNITY_EDITOR
using UnityEngine;

namespace Thisaislan.Scriptables.Editor.Abstracts
{
    /// <summary>
    /// Base class for scriptable objects with editor-specific debugging capabilities.
    /// </summary>
    public abstract class ScriptableEditorDebbugable : ScriptableObject
    {
        internal ScriptableEditorDebbugable()
        {
            // Avoid external heritage
        }

        public  abstract void PrintDataDebugEditor();
        internal abstract string GetStringData();
        internal abstract void ResetToDefaultState();
        internal abstract object GetDataObject();
    }
}
#endif