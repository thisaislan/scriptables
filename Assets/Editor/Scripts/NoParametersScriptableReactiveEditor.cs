#if UNITY_EDITOR
using UnityEditor;
using Thisaislan.Scriptables.Editor.Helpers;
using UnityEngine;

namespace Thisaislan.Scriptables.Editor
{
    // The `CustomEditor` attribute tells Unity to use this class to draw the inspector
    // for the `NoParametersScriptableReactive` type. The `true` argument means it will
    // also work for any class that derives from it.
    [CustomEditor(typeof(NoParametersScriptableReactive), true)]
    internal class NoParametersScriptableReactiveEditor : UnityEditor.Editor
    {
        /// <summary>
        /// Set icon when the inspector is enabled
        /// </summary>
        private void OnEnable()
        {
            EditorGUIUtility.SetIconForObject(target, Resources.Load<Texture2D>(Consts.ScriptableReactiveIconName));
        }

        /// <summary>
        /// Main method for drawing the custom inspector GUI. Called by Unity whenever the inspector is drawn.
        /// </summary>
        public override void OnInspectorGUI()
        {
            // First, draw the clean inspector
            new ScriptableEditorHelper().DrawCleanDefaultInspector(serializedObject);

            EditorGUILayout.Space();

            // Then, add a helpful info box below the default inspector to provide context or usage instructions.
            // `Consts.NoParametersScriptableReactiveEditorMessage` is likely a constant string like "This ScriptableObject reacts to events without parameters."
            EditorGUILayout.HelpBox(Consts.NoParametersScriptableReactiveEditorMessage, MessageType.Info);

            if (Application.isPlaying)
            {
                EditorGUILayout.Space();

                if (GUILayout.Button(Consts.NotifyRuntimeDataLabel))
                {
                    ((NoParametersScriptableReactive)target).Notify();
                }
            }
        }
    }
}
#endif