#if UNITY_EDITOR
using System;
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

        internal event Action onDataChange;

        internal abstract void DataDebugPrint();
        internal abstract string GetStringData();
        internal abstract void ResetToDefaultState();
        internal abstract object GetDataObject();

        internal void OnDataChange()
        {
            onDataChange?.Invoke();
        }
    }
}
#endif