#if UNITY_EDITOR
using System;
using Thisaislan.Scriptables.Editor.Utilities.DrawHelpers;
using UnityEngine;

namespace Thisaislan.Scriptables.Editor.Inspectors.Abstracts
{
    /// <summary>
    /// Custom base editor for debbugable scriptables editors.
    /// Provides base methods shared.
    /// </summary>
    internal abstract class BaseScriptableDebuggableEditor<T> : UnityEditor.Editor where T : UnityEngine.Object
    {
        private bool isEditingDescription;
        private bool showDetails;
        private T[] scriptablesArrayCache;

        /// <summary>
        /// Gets the currently selected single target cast as type <typeparamref name="T"/>.
        /// Returns null if the target is not of the expected type.
        /// </summary>
        protected T Scriptable
        {
            get
            {
                return target as T;
            }
        }

        /// <summary>
        /// Enumerates all selected target objects in the inspector cast as type <typeparamref name="T"/>.
        /// Used to iterate safely over single and multiple selections in Unity's editor.
        /// Invalid or mismatched types are returned as null.
        /// </summary>
        protected T[] ScriptablesArray
        {
            get
            {
                if (scriptablesArrayCache == null || scriptablesArrayCache.Length != targets.Length)
                {
                    scriptablesArrayCache = new T[targets.Length];

                    for (int i = 0; i < targets.Length; i++)
                    {
                        scriptablesArrayCache[i] = targets[i] as T;
                    }
                }

                return scriptablesArrayCache;
            }
        }

        /// <summary>
        /// Indicates whether the inspector is currently editing multiple target objects.
        /// This is determined by checking Unity's SerializedObject multi-editing state.
        /// </summary>
        protected bool IsMultipleTargets
        {
            get
            {
                return targets != null && targets.Length > 1;
            }
        }

        /// <summary>
        /// Draws the basic start layout for all scriptables in that lib.
        /// </summary>
        public override void OnInspectorGUI()
        {
            try
            {
                serializedObject.Update();

                DrawPropertyEditorHelper.DrawDetailsCard(serializedObject, IsMultipleTargets, ref showDetails);

                DrawEditorHelper.DrawSpaceBetweenCards();

                DrawPropertyEditorHelper.DrawScriptProperties(serializedObject, IsMultipleTargets, ref isEditingDescription);

                if (IsMultipleTargets)
                {
                    OnMultipleTargetsInspectorGUI();
                }
                else
                {
                    OnSingleTargetInspectorGUI();
                }

                serializedObject.ApplyModifiedProperties();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"BaseScriptableDebuggableEditor.OnInspectorGUI encountered an error: {e.Message}");
            }
        }

        /// <summary>
        /// Called when the editor is enabled. Invalidates the cached scriptables array.
        /// </summary>
        protected virtual void OnEnable()
        {
            scriptablesArrayCache = null;
        }

        /// <summary>
        /// Called when the editor is disabled. Unsubscribes from editor update events and releases hot control.
        /// </summary>
        protected virtual void OnDisable()
        {
            DrawEditorHelper.EnableGuiEnableState();
            GUIUtility.hotControl = 0;
        }

        /// <summary>
        /// Draws the inspector GUI when a single target object is selected.
        /// Override this method to implement custom inspector logic for single-object editing.
        /// </summary>
        protected abstract void OnSingleTargetInspectorGUI();

        /// <summary>
        /// Draws the inspector GUI when multiple target objects are selected.
        /// Override this method to implement custom inspector logic for multi-object editing.
        /// </summary>
        protected abstract void OnMultipleTargetsInspectorGUI();

        /// <summary>
        /// Executes the specified action on all selected target scriptable objects in the Unity inspector.
        /// When multiple objects are selected, the action is applied to each valid target in the selection.
        /// When only one object is selected, the action is applied to the single target.
        /// Null references are ignored safely.
        /// </summary>
        /// <param name="action">
        /// The action to execute for each target scriptable instance.
        /// </param>
        protected void ExecuteOnTargets(Action<T> action)
        {
            T[] currentScriptables = ScriptablesArray;

            for (int i = 0; i < currentScriptables.Length; i++)
            {
                T scriptableInstance = currentScriptables[i];

                if (scriptableInstance != null)
                {
                    action.Invoke(scriptableInstance);
                }
            }
        }
    }
}
#endif