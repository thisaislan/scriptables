#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Thisaislan.Scriptables.Editor.Abstracts.Bases
{
    /// <summary>
    /// Base class for ScriptableObjects with editor-specific debugging capabilities with transient.
    /// </summary>
    public abstract class BaseEditorDebuggableTransientScriptable : BaseEditorDebuggableScriptable
    {
        /// <summary>
        /// Internal constructor to prevent external inheritance.
        /// Only internal classes within the assembly can inherit from this class.
        /// </summary>
        /// 
        internal BaseEditorDebuggableTransientScriptable()
        {
            // Avoid external heritage - only allow internal inheritance
        }

        /// <summary>
        /// Called when the ScriptableObject is enabled. Resets runtime data when not in play mode
        /// and subscribes to play mode state changes.
        /// </summary>
        protected virtual void OnEnable()
        {
            if (!Application.isPlaying)
            {
                ResetRuntimeDataEditorOnly();
            }

            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        /// <summary>
        /// Called when the ScriptableObject is disabled. Cleans runtime data and unsubscribes
        /// from play mode state changes.
        /// </summary>
        protected virtual void OnDisable()
        {
            if (!Application.isPlaying)
            {
                CleanRuntimeDataAfterRun();
            }

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        /// <summary>
        /// Sets the runtime data value.
        /// </summary>
        /// <param name="data">The data object to set</param>
        internal abstract void SetRuntimeDataEditorOnly(object data);

        /// <summary>
        /// Resets the object's runtime data to its default state.
        /// </summary>
        internal abstract void ResetRuntimeDataEditorOnly();

        /// <summary>
        /// Cleans the object's runtime data to its default state.
        /// </summary>
        internal abstract void ClearRuntimeDataEditorOnly();

        /// <summary>
        /// Gets the current runtime data value for editor display.
        /// </summary>
        /// <returns>The current runtime data object</returns>
        internal abstract object GetRuntimeDataEditorOnly();

        /// <summary>
        /// Called when play mode state changes. Cleans runtime data when exiting play mode.
        /// </summary>
        /// <param name="state">The new play mode state</param>
        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                CleanRuntimeDataAfterRun();
            }
        }

        /// <summary>
        /// Cleans runtime data after play mode ends and marks the object as dirty.
        /// </summary>
        private void CleanRuntimeDataAfterRun()
        {
            try
            {
                ResetRuntimeDataEditorOnly();
                EditorUtility.SetDirty(this);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"BaseEditorDebuggableTransientScriptable.CleanRuntimeDataAfterRun encountered an error: {e.Message}");
            }
        }
    }
}
#endif