#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Thisaislan.Scriptables.Editor.Abstracts;
using UnityEditor;
using UnityEngine;

namespace Thisaislan.Scriptables.Editor
{
    /// <summary>
    /// Custom editor for BaseScriptableReactiveEditorDebbugable objects
    /// </summary>
    [CustomEditor(typeof(BaseScriptableReactiveEditorDebbugable), true)]
    internal class ScriptableReactiveEditorDebbugableBaseEditor : BaseScriptableEditor
    {
        private bool serialized;
        
        // List of types that should be shown as not serialized
        private static readonly HashSet<Type> NonSerializedTypes = new HashSet<Type>
        {
            typeof(Transform),
            typeof(Rigidbody),
            typeof(Rigidbody2D),
            typeof(Collider),
            typeof(Collider2D),
            typeof(MeshRenderer),
            typeof(SkinnedMeshRenderer),
            typeof(Animator),
            typeof(Animation),
            typeof(AudioSource),
            typeof(Camera),
            typeof(Light),
            typeof(ParticleSystem)
        };

        /// <summary>
        /// Initializes the editor
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            BaseScriptableReactiveEditorDebbugable scriptable = GetTarget();
            EditorGUIUtility.SetIconForObject(target, Resources.Load<Texture2D>(Consts.ScriptableReactiveIconName));
            isSimpleType = scriptableEditorHelper.IsSimpleType(scriptable.GetValueType());
            serialized = true;
        }

        /// <summary>
        /// Main method for drawing the custom inspector GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            try
            {
                serializedObject.Update();
                scriptableEditorHelper.DrawCleanDefaultInspector(serializedObject);

                EditorGUILayout.Space();

                if (!isSimpleType && !scriptableEditorHelper.IsUnitySerializable(GetData().GetType()))
                {
                    scriptableEditorHelper.DrawEditorDataAsExpanded(serializedObject, Consts.EditorValueField, Consts.EditorValueLabel);
                }
                else
                {
                    DrawEditorValueField();
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
        /// Draws the editor value field
        /// </summary>
        private void DrawEditorValueField()
        {
            SerializedProperty editorValueProperty = serializedObject.FindProperty(Consts.EditorValueField);
            Type valueType = GetTarget().GetValueType();
            
            // Check if the type is in our non-serialized list
            bool isNonSerializedType = NonSerializedTypes.Contains(valueType);

            if (editorValueProperty != null && !isNonSerializedType)
            {
                serialized = true;
                string typeName = TypeNameSimplifier.SimplifyTypeName(valueType.Name);
                typeName = TypeNameSimplifier.CapitalizeFirstLetter(typeName);
                
                // For Quaternion, use a custom drawer to reduce space
                if (valueType == typeof(Quaternion))
                {
                    EditorGUILayout.LabelField(typeName, EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    DrawQuaternionFieldCompact(editorValueProperty);
                    EditorGUI.indentLevel--;
                }
                else
                {
                    EditorGUILayout.LabelField(typeName, EditorStyles.boldLabel);
                    
                    if (isSimpleType)
                    {
                        EditorGUI.indentLevel++;
                    }

                    EditorGUILayout.PropertyField(editorValueProperty, new GUIContent(Consts.ReactiveVariableName), true);

                    if (isSimpleType)
                    {
                        EditorGUI.indentLevel--;
                    }
                }
            }
            else
            {
                EditorGUILayout.Space();

                serialized = false;
                string message = isNonSerializedType ?
                    $"{TypeNameSimplifier.SimplifyTypeName(valueType.Name)} - {Consts.TypeNotSerialized}" :
                    $"{Consts.ScriptableReactiveName}<{TypeNameSimplifier.SimplifyTypeName(valueType.Name)}> - {Consts.TypeNotSerialized}";

                string combinedText = $"{Consts.EditorValueLabel} ({message})";
                EditorGUILayout.LabelField(combinedText, EditorStyles.boldLabel);
            }
        }

        /// <summary>
        /// Draws a compact Quaternion field using Euler angles
        /// </summary>
        private void DrawQuaternionFieldCompact(SerializedProperty property)
        {
            // Get the current quaternion value
            Quaternion quat = GetQuaternionFromProperty(property);
            
            // Convert to Euler angles for more compact editing
            Vector3 euler = quat.eulerAngles;
            
            // Draw a compact Vector3 field
            Vector3 newEuler = EditorGUILayout.Vector3Field(Consts.ReactiveVariableName, euler);
            
            // Update if changed
            if (newEuler != euler)
            {
                Quaternion newQuat = Quaternion.Euler(newEuler);
                SetQuaternionToProperty(property, newQuat);
            }
        }

        /// <summary>
        /// Extracts a Quaternion value from a SerializedProperty
        /// </summary>
        private Quaternion GetQuaternionFromProperty(SerializedProperty property)
        {
            if (property.propertyType == SerializedPropertyType.Quaternion)
            {
                return property.quaternionValue;
            }
            
            // For serialized Quaternion structs
            if (property.hasChildren)
            {
                float x = property.FindPropertyRelative("x")?.floatValue ?? 0;
                float y = property.FindPropertyRelative("y")?.floatValue ?? 0;
                float z = property.FindPropertyRelative("z")?.floatValue ?? 0;
                float w = property.FindPropertyRelative("w")?.floatValue ?? 1;
                
                return new Quaternion(x, y, z, w);
            }
            
            return Quaternion.identity;
        }

        /// <summary>
        /// Sets a Quaternion value to a SerializedProperty
        /// </summary>
        private void SetQuaternionToProperty(SerializedProperty property, Quaternion quaternion)
        {
            if (property.propertyType == SerializedPropertyType.Quaternion)
            {
                property.quaternionValue = quaternion;
                return;
            }
            
            // For serialized Quaternion structs
            if (property.hasChildren)
            {
                SerializedProperty xProp = property.FindPropertyRelative("x");
                SerializedProperty yProp = property.FindPropertyRelative("y");
                SerializedProperty zProp = property.FindPropertyRelative("z");
                SerializedProperty wProp = property.FindPropertyRelative("w");
                
                if (xProp != null) xProp.floatValue = quaternion.x;
                if (yProp != null) yProp.floatValue = quaternion.y;
                if (zProp != null) zProp.floatValue = quaternion.z;
                if (wProp != null) wProp.floatValue = quaternion.w;
            }
        }

        /// <summary>
        /// Get the target
        /// </summary>
        private BaseScriptableReactiveEditorDebbugable GetTarget()
        {
            return (BaseScriptableReactiveEditorDebbugable)target;
        }

        /// <summary>
        /// Draws runtime buttons (Print, Notify, Reset)
        /// </summary>
        protected override void DrawRuntimeButtons()
        {
            if (GUILayout.Button(Consts.PrintRuntimeDataLabel))
            {
                PrintDataDebug();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button(Consts.NotifyRuntimeDataLabel))
            {
                NotifyValue();
            }

            if (serialized && !NonSerializedTypes.Contains(target.GetType()))
            {
                EditorGUILayout.Space();

                if (GUILayout.Button(Consts.ResetRuntimeDataLabel))
                {
                    ResetRuntimeData();
                }
            }
        }
        /// <summary>
        /// Prints data debug information
        /// </summary>
        protected override void PrintDataDebug()
        {
            GetTarget().PrintDataDebugEditor();
        }

        // Implementation of abstract methods
        protected override object GetData() { return GetTarget().GetRuntimeValue(); }
        protected override object GetEditorData() { return GetTarget().GetEditorValue(); }
        protected override void SetData(object data) { GetTarget().SetRuntimeValue(data); }
        protected override void NotifyValue() { GetTarget().NotifyValue(); }
        protected override void ResetToDefaultState() { GetTarget().ResetToDefaultState(); }
        protected override Type GetValueType() { return GetTarget().GetValueType(); }
    }
}
#endif