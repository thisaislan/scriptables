#if UNITY_EDITOR
using System;
using Thisaislan.Scriptables.Abstracts;
using Thisaislan.Scriptables.Editor.Abstracts;
using UnityEditor;
using UnityEngine;

namespace Thisaislan.Scriptables.Editor
{
    /// <summary>
    /// Custom editor for ScriptableEditorDebbugable objects
    /// </summary>
    [CustomEditor(typeof(ScriptableEditorDebbugable), true)]
    internal class ScriptableEditorDebbugableEditor : BaseScriptableEditor
    {
        protected ScriptableEditorDebbugable scriptable;

        /// <summary>
        /// Initializes the editor
        /// </summary>
        protected override void OnEnable()
        {
            scriptable = target as ScriptableEditorDebbugable;

            base.OnEnable();

            if (scriptable.GetScriptableEditorDebbugableType() == ScriptableEditorDebbugable.ScriptableEditorDebbugableType.Settings)
            {
                EditorGUIUtility.SetIconForObject(target, Resources.Load<Texture2D>(EditorConsts.ScriptableSettingsIconName));
            }
            else
            {
                EditorGUIUtility.SetIconForObject(target, Resources.Load<Texture2D>(EditorConsts.ScriptableRuntimeIconName));

                EditorApplication.update += RepaintIfNotNull;
            }

            isSimpleType = scriptableEditorHelper.IsSimpleType(scriptable.GetData()?.GetType());
        }

        /// <summary>
        /// Main method for drawing the custom inspector GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            try
            {
                scriptableEditorHelper.DrawCleanDefaultInspector(serializedObject);

                serializedObject.Update();

                if (scriptable.GetScriptableEditorDebbugableType() == ScriptableEditorDebbugable.ScriptableEditorDebbugableType.Settings)
                {
                    scriptableEditorHelper.DrawEditorDataAsExpanded(serializedObject, EditorConsts.EditorDataField, EditorConsts.EditorDataLabel);
                }

                EditorGUILayout.Space();

                serializedObject.ApplyModifiedProperties();
                base.OnInspectorGUI();
            }
            catch
            { 
                
            }
        }

         /// <summary>
        /// Checks if this editor requires constant repaints in its current state.
        /// </summary>
        public override bool RequiresConstantRepaint()
        {
            return scriptable.GetScriptableEditorDebbugableType() != ScriptableEditorDebbugable.ScriptableEditorDebbugableType.Settings;
        }

        /// <summary>
        /// Clean up when the editor is disabled
        /// </summary>
        protected override void OnDisable()
        {
            if (scriptable.GetScriptableEditorDebbugableType() != ScriptableEditorDebbugable.ScriptableEditorDebbugableType.Settings)
            {
                EditorApplication.update -= RepaintIfNotNull;
            }

            base.OnDisable();
        }
        
        /// <summary>
        /// Method for drawing the bottom
        /// </summary>
        protected override void DrawRuntimeBottom()
        {
            if (GUILayout.Button(EditorConsts.PrintRuntimeDataLabel))
            {
                scriptable.PrintDataDebugEditor();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button(EditorConsts.ResetRuntimeDataLabel))
            {
                ResetRuntimeData();
            }
        }

        // Implementation of abstract methods
        protected override object GetData()
        {
            return scriptable.GetData();
        }

        protected override object GetEditorData()
        {
            return scriptable.GetData();
        }

        protected override void SetData(object data)
        {
            scriptable.SetData((Data)data);
        }
        
        protected override void ResetToDefaultState()
        { 
            scriptable.ResetToDefaultState();
        }

        protected override Type GetValueType()
        {
            return scriptable.GetData()?.GetType();
        }
    }
}
#endif