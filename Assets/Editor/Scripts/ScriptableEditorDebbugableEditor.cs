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
        /// <summary>
        /// Initializes the editor
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            ScriptableEditorDebbugable scriptable = GetTarget();

            if (scriptable.GetScriptableEditorDebbugableType() == ScriptableEditorDebbugable.ScriptableEditorDebbugableType.Settings)
            {
                EditorGUIUtility.SetIconForObject(target, Resources.Load<Texture2D>(Consts.ScriptableSettingsIconName));
            }
            else
            {
                EditorGUIUtility.SetIconForObject(target, Resources.Load<Texture2D>(Consts.ScriptableRuntimeIconName));
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
                ScriptableEditorDebbugable scriptable = GetTarget();
                scriptableEditorHelper.DrawCleanDefaultInspector(serializedObject);

                serializedObject.Update();

                if (scriptable.GetScriptableEditorDebbugableType() == ScriptableEditorDebbugable.ScriptableEditorDebbugableType.Settings)
                {
                    scriptableEditorHelper.DrawEditorDataAsExpanded(serializedObject, Consts.EditorDataField, Consts.EditorDataLabel);
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
        /// Method for drawing the runtime buttons
        /// </summary>
        protected override void DrawRuntimeButtons()
        {
            if (GUILayout.Button(Consts.PrintRuntimeDataLabel))
            {
                GetTarget().PrintDataDebugEditor();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button(Consts.ResetRuntimeDataLabel))
            {
                ResetRuntimeData();
            }
        }
        
        /// <summary>
        /// Get target
        /// </summary>
        private ScriptableEditorDebbugable GetTarget()
        {
            return (ScriptableEditorDebbugable)target;
        }

        // Implementation of abstract methods
        protected override object GetData() { return GetTarget().GetData(); }
        protected override object GetEditorData() { return GetTarget().GetData(); }
        protected override void SetData(object data) { GetTarget().SetData((Data)data); }
        protected override void NotifyValue() { /* Not used in this editor */ }
        protected override void ResetToDefaultState() { GetTarget().ResetToDefaultState(); }
        protected override Type GetValueType() { return GetTarget().GetData()?.GetType(); }
    }
}
#endif