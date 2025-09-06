#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Thisaislan.Scriptables.Abstracts;
using Thisaislan.Scriptables.Editor.Utilities;

namespace Thisaislan.Scriptables.Editor.Helpers
{
    /// <summary>
    /// Helper class for drawing custom editor UI for ScriptableObjects with debugging capabilities
    /// Provides methods for drawing clean inspectors, handling editor data, and preventing circular references
    /// </summary>
    internal class ScriptableEditorHelper
    {
        /// <summary>
        /// Draws the default inspector but excludes the editorData field
        /// </summary>
        /// <param name="serializedObject">The SerializedObject to draw the inspector for</param>
        internal void DrawCleanDefaultInspector(SerializedObject serializedObject)
        {
            SerializedProperty property = serializedObject.GetIterator();
            bool enterChildren = true;

            while (property.NextVisible(enterChildren))
            {
                enterChildren = false;

                // Skip the editorData field as it's handled separately
                if (property.name.Equals(Consts.ScriptPropertyName))
                {
                    using (new EditorGUI.DisabledScope(true))
                    {
                        EditorGUILayout.PropertyField(property, true);                        
                    }

                    return;
                }
            }
        }

        /// <summary>
        /// Draws the editor data property in an expanded view
        /// </summary>
        /// <param name="serializedObject">The SerializedObject containing the editor data</param>
        internal void DrawEditorDataAsExpanded(SerializedObject serializedObject, string propertyFieldToFound, string label)
        {
            SerializedProperty editorDataProperty = serializedObject.FindProperty(propertyFieldToFound);

            if (editorDataProperty != null)
            {
                // Draw bold header

                EditorGUILayout.Space();

                string combinedText = $"{label} ({TypeNameSimplifier.CleanTypeName(editorDataProperty.type)})";

                EditorGUILayout.LabelField(combinedText, EditorStyles.boldLabel);

                // Store the original expanded state
                bool wasExpanded = editorDataProperty.isExpanded;

                // Force the property to be expanded
                editorDataProperty.isExpanded = true;

                try
                {
                    EditorGUI.indentLevel++;

                    // Iterate through all child properties
                    SerializedProperty endProperty = editorDataProperty.GetEndProperty();
                    SerializedProperty childProperty = editorDataProperty.Copy();
                    childProperty.NextVisible(true); // Move to first child

                    while (!SerializedProperty.EqualContents(childProperty, endProperty))
                    {
                        // Use a safe method to draw property fields
                        DrawPropertyFieldSafe(childProperty);

                        if (!childProperty.NextVisible(false))
                        {
                            break;
                        }
                    }

                    EditorGUI.indentLevel--;
                }
                finally
                {
                    // Restore the original expanded state
                    editorDataProperty.isExpanded = wasExpanded;
                }
            }
        }

        /// <summary>
        /// Draws a Unity type field with appropriate control based on type
        /// </summary>
        /// <param name="value">The current value</param>
        /// <param name="valueType">The type of the value</param>
        /// <param name="fieldName">The name of the field (optional)</param>
        /// <param name="useLayout">Whether to use GUILayout (true) or GUI (false)</param>
        /// <param name="rect">The rect to draw in (only used when useLayout is false)</param>
        /// <returns>A tuple containing the new value and whether it changed</returns>
        internal (object newValue, bool valueChanged) DrawUnityTypeFieldInternal(
            object value, 
            Type valueType, 
            string fieldName = null,
            bool useLayout = true,
            Rect rect = default)
        {
            object newValue = value;
            bool valueChanged = false;

            // Use Unity's native field drawers for supported types
            if (valueType == typeof(Vector2))
            {
                if (useLayout)
                    newValue = EditorGUILayout.Vector2Field(fieldName, (Vector2)value);
                else
                    newValue = EditorGUI.Vector2Field(rect, GUIContent.none, (Vector2)value);
                valueChanged = !newValue.Equals(value);
            }
            else if (valueType == typeof(Vector3))
            {
                if (useLayout)
                    newValue = EditorGUILayout.Vector3Field(fieldName, (Vector3)value);
                else
                    newValue = EditorGUI.Vector3Field(rect, GUIContent.none, (Vector3)value);
                valueChanged = !newValue.Equals(value);
            }
            else if (valueType == typeof(Vector4))
            {
                if (useLayout)
                    newValue = EditorGUILayout.Vector4Field(fieldName, (Vector4)value);
                else
                    newValue = EditorGUI.Vector4Field(rect, GUIContent.none, (Vector4)value);
                valueChanged = !newValue.Equals(value);
            }
            else if (valueType == typeof(Quaternion))
            {
                Quaternion quat = (Quaternion)value;
                Vector3 euler = quat.eulerAngles;
                Vector3 result;

                if (useLayout)
                {
                    result = EditorGUILayout.Vector3Field(fieldName, euler);
                }
                else
                {
                    result = EditorGUI.Vector3Field(rect, GUIContent.none, euler);
                }
                    
                newValue = Quaternion.Euler(result);
                valueChanged = !newValue.Equals(value);
            }
            else if (valueType == typeof(Color))
            {
                if (useLayout)
                    newValue = EditorGUILayout.ColorField(fieldName, (Color)value);
                else
                    newValue = EditorGUI.ColorField(rect, (Color)value);
                valueChanged = !newValue.Equals(value);
            }
            else if (valueType == typeof(Color32))
            {
                if (useLayout)
                    newValue = EditorGUILayout.ColorField(fieldName, (Color32)value);
                else
                    newValue = EditorGUI.ColorField(rect, (Color32)value);
                valueChanged = !newValue.Equals(value);
            }
            else if (valueType == typeof(Rect))
            {
                if (useLayout)
                    newValue = EditorGUILayout.RectField(fieldName, (Rect)value);
                else
                    newValue = EditorGUI.RectField(rect, (Rect)value);
                valueChanged = !newValue.Equals(value);
            }
            else if (valueType == typeof(Bounds))
            {
                if (useLayout)
                    newValue = EditorGUILayout.BoundsField(fieldName, (Bounds)value);
                else
                    newValue = EditorGUI.BoundsField(rect, (Bounds)value);
                valueChanged = !newValue.Equals(value);
            }
            else if (valueType == typeof(AnimationCurve))
            {
                if (useLayout)
                    newValue = EditorGUILayout.CurveField(fieldName, (AnimationCurve)value);
                else
                    newValue = EditorGUI.CurveField(rect, (AnimationCurve)value);
                valueChanged = !newValue.Equals(value);
            }
            else if (valueType == typeof(Gradient))
            {
                if (useLayout)
                    newValue = EditorGUILayout.GradientField(fieldName, (Gradient)value);
                else
                    newValue = EditorGUI.GradientField(rect, (Gradient)value);
                valueChanged = !newValue.Equals(value);
            }
            else if (typeof(UnityEngine.Object).IsAssignableFrom(valueType))
            {
                if (useLayout)
                    newValue = EditorGUILayout.ObjectField(fieldName, (UnityEngine.Object)value, valueType, true);
                else
                    newValue = EditorGUI.ObjectField(rect, (UnityEngine.Object)value, valueType, true);
                valueChanged = !newValue.Equals(value);
            }
            else
            {
                // Fallback for unsupported Unity types
                if (useLayout)
                    EditorGUILayout.LabelField(fieldName, value != null ? value.ToString() : Consts.NullLiteral);
                else
                    EditorGUI.LabelField(rect, value != null ? value.ToString() : Consts.NullLiteral);
            }

            return (newValue, valueChanged);
        }
        
        /// <summary>
        /// Draws a field with appropriate control based on type
        /// </summary>
        /// <param name="fieldName">The name of the field to draw</param>
        /// <param name="value">The current value of the field</param>
        /// <param name="fieldType">The type of the field</param>
        /// <param name="parentType">The parent type (for self-reference detection)</param>
        /// <param name="drawnObjects">Set of objects currently being drawn (for circular reference detection)</param>
        /// <returns>The new value from the editor field</returns>
        internal object DrawField(string fieldName, object value, Type fieldType, Type parentType, HashSet<object> drawnObjects)
        {
            // Check for self-reference
            if (fieldType == parentType)
            {
                EditorGUILayout.LabelField(fieldName, $"{TypeNameSimplifier.SimplifyTypeName(fieldType.Name)} - {Consts.SelfReferenceNotSupported}");
                return value;
            }
            
            try
            {
                // Use the shared helper for Unity types
                if (fieldType == typeof(int))
                {
                    return EditorGUILayout.IntField(fieldName, (int)value);
                }
                else if (fieldType == typeof(float))
                {
                    return EditorGUILayout.FloatField(fieldName, (float)value);
                }
                else if (fieldType == typeof(bool))
                {
                    return EditorGUILayout.Toggle(fieldName, (bool)value);
                }
                else if (fieldType == typeof(string))
                {
                    return EditorGUILayout.TextField(fieldName, (string)value);
                }
                else if (fieldType.IsEnum)
                {
                    return EditorGUILayout.EnumPopup(fieldName, (Enum)value);
                }
                else if (IsUnitySerializable(fieldType) && fieldType != typeof(string))
                {
                    var result = DrawUnityTypeFieldInternal(value, fieldType, fieldName, true);
                    return result.newValue;
                }
                else
                {
                    // For complex types, draw a foldout with nested fields
                    EditorGUILayout.LabelField(fieldName, TypeNameSimplifier.SimplifyTypeName(fieldType.Name));
                    EditorGUI.indentLevel++;
                    DrawDataFieldsWithCircularProtection(value, true, fieldType, drawnObjects);
                    EditorGUI.indentLevel--;
                    return value;
                }
            }
            catch
            {
                // Handle any errors during field drawing
                EditorGUILayout.LabelField(fieldName, $"{TypeNameSimplifier.SimplifyTypeName(fieldType.Name)} - {Consts.TypeNotSupported}");
                return value;
            }
        }

        /// <summary>
        /// Draws data fields with protection against circular references
        /// </summary>
        /// <param name="data">The data object to draw fields for</param>
        /// <param name="editable">Whether the fields should be editable</param>
        /// <param name="parentType">The parent type (for self-reference detection)</param>
        /// <param name="drawnObjects">Set of objects currently being drawn (for circular reference detection)</param>
        internal void DrawDataFieldsWithCircularProtection(object data, bool editable, Type parentType, HashSet<object> drawnObjects)
        {
            if (data == null)
            {
                return;
            }
            
            // Check for circular references
            if (drawnObjects.Contains(data))
            {
                EditorGUILayout.HelpBox(Consts.CircularReferenceDetected, MessageType.Warning);
                return;
            }
            
            drawnObjects.Add(data);
            
            Type dataType = data.GetType();
            
            // For complex types, use a simplified approach
            using (new EditorGUI.DisabledScope(!editable))
            {
                foreach (FieldInfo field in dataType.GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    object value = field.GetValue(data);
                    object newValue = DrawField(TypeNameSimplifier.CapitalizeFirstLetter(field.Name), value, field.FieldType, parentType, drawnObjects);
                    
                    if (!Equals(newValue, value))
                    {
                        field.SetValue(data, newValue);
                    }
                }
            }
            
            drawnObjects.Remove(data);
        }
        
        /// <summary>
        /// Draws a simplified data structure visualization
        /// </summary>
        /// <param name="dataType">The data type to visualize</param>
        internal void DrawDataStructureSimple(Type dataType)
        {
            EditorGUI.indentLevel++;

            if (IsSimpleType(dataType))
            {
                EditorGUILayout.LabelField(Consts.ReactiveVariableName, TypeNameSimplifier.SimplifyTypeName(dataType.Name));
            }
            else
            {
                foreach (FieldInfo field in dataType.GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    Type fieldType = field.FieldType;
                    string typeName = TypeNameSimplifier.SimplifyTypeName(fieldType.Name);

                    // Check for self-reference
                    if (fieldType == dataType)
                    {
                        typeName += $" - {Consts.SelfReferenceNotSupported}";
                    }

                    EditorGUILayout.LabelField(
                        ObjectNames.NicifyVariableName(field.Name),
                        typeName
                    );
                }
            }

            EditorGUI.indentLevel--;
        }
        
        /// <summary>
        /// Checks if a type is a simple type (primitives, strings, etc.)
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns>True if the type is a simple type</returns>
        internal bool IsSimpleType(Type type)
        {
            if (type == null)
            {
                return false;
            }

            return type.IsPrimitive ||
                       type == typeof(string) ||
                       type == typeof(decimal) ||
                       type.IsEnum;
        }

        /// <summary>
        /// Checks if a type is serializable by Unity
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns>True if the type is serializable by Unity</returns>
        internal bool IsUnitySerializable(Type type)
        {
            // Unity can serialize these types
            if (type == typeof(Vector2) ||
                type == typeof(Vector3) ||
                type == typeof(Vector4) ||
                type == typeof(Quaternion) ||
                type == typeof(Color) ||
                type == typeof(Color32) ||
                type == typeof(Rect) ||
                type == typeof(Bounds) ||
                type == typeof(AnimationCurve) ||
                type == typeof(Gradient) ||
                typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                return true;
            }

            // Check if it's a serializable class with the [Serializable] attribute
            if (type.IsClass && Attribute.IsDefined(type, typeof(SerializableAttribute)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the type of the editor data property
        /// </summary>
        /// <param name="serializedObject">The SerializedObject to inspect</param>
        /// <returns>The type of the editor data, or null if not found</returns>
        private Type GetEditorDataType(SerializedObject serializedObject)
        {
            var targetObj = serializedObject.targetObject;
            if (targetObj == null)
            {
                return null;
            }

            // Try to get the value of the private field editorData
            var type = targetObj.GetType();
            var editorDataField = type.GetField(Consts.EditorDataField, BindingFlags.NonPublic | BindingFlags.Instance);

            if (editorDataField != null)
            {
                var value = editorDataField.GetValue(targetObj);
                if (value != null)
                {
                    return value.GetType();
                }
            }

            // Fallback: get generic type argument T from ScriptableSettings<T>
            var baseType = type.BaseType;
            while (baseType != null)
            {
                if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(ScriptableSettings<>))
                {
                    return baseType.GetGenericArguments()[0];
                }
                baseType = baseType.BaseType;
            }

            return null;
        }
        
        /// <summary>
        /// Safely draws a property field with error handling
        /// </summary>
        /// <param name="property">The property to draw</param>
        private void DrawPropertyFieldSafe(SerializedProperty property)
        {
            try
            {
                EditorGUILayout.PropertyField(property, true);
            }
            catch (ExitGUIException)
            {
                // Re-throw ExitGUIException to let Unity handle it properly
                throw;
            }
            catch (Exception e)
            {
                Printer.PrintWarning($"{Consts.CustomInspectorError}: {e}");
            }
        }
    }
}
#endif