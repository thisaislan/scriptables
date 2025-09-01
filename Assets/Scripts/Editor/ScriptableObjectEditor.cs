#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Reflection;
using Thisaislan.Scriptables.Editor.Consts;
using Thisaislan.Scriptables.Editor.Utilities;
using Thisaislan.Scriptables.Editor.Abstracts;
using Thisaislan.Scriptables.Abstracts;
using System.Collections.Generic;
using System;


namespace Thisaislan.Scriptables.Editor
{
    [CustomEditor(typeof(ScriptableEditorDebbugable), true)]
    internal class ScriptableObjectEditor : UnityEditor.Editor
    {
        private SerializedObject dataSerializedObject;
        private object dataObject;
        private object cachedDataObject; // To track changes
        private Dictionary<string, object> fieldValues = new Dictionary<string, object>();
        private bool isWaitingForCachedData = false;
        private DateTime lastRepaintTime;
        private const double repaintDelaySeconds = 0.5; // Delay between repaints

        public override void OnInspectorGUI()
        {
            var scriptable = (ScriptableEditorDebbugable)target;

            // Draw default inspector for serialized fields
            DrawDefaultInspector();

            EditorGUILayout.Space();
                        
            // Show appropriate sections based on type and play mode
            if (target is ScriptableSettings<Data>)
            {
                DrawSettingsScriptableGUI(scriptable);
            }
            else
            {
                DrawRuntimeScriptableGUI(scriptable);
            }
        }

        private void DrawRuntimeScriptableGUI(ScriptableEditorDebbugable scriptable)
        {
            if (Application.isPlaying)
            {
                EditorGUILayout.LabelField(MetadataEditor.ScriptableObjectEditor.RUNTIME_DATA_LABEL, EditorStyles.boldLabel);

                // Get the data object
                dataObject = scriptable.GetDataObject();
                if (dataObject != null)
                {
                    // Draw data fields using reflection
                    DrawDataFieldsWithReflection(dataObject, true);

                    // Apply changes if data was modified
                    if (cachedDataObject != null && !ReflectionUtility.AreObjectsEqual(dataObject, cachedDataObject))
                    {
                        // Data was changed in the inspector, update the original
                        ReflectionUtility.CopyObjectData(dataObject, scriptable.GetDataObject());
                        cachedDataObject = scriptable.GetDataObject();
                        EditorUtility.SetDirty(scriptable);
                    }
                    else
                    {
                        cachedDataObject = scriptable.GetDataObject();
                    }

                    // We have data, stop waiting
                    isWaitingForCachedData = false;
                }
                else
                {
                    // No data available, show waiting message
                    EditorGUILayout.HelpBox(MetadataEditor.ScriptableObjectEditor.WAITING_FOR_DATA_INITIALIZATION_LABEL, MessageType.Info);

                    // Start waiting for cached data if not already
                    if (!isWaitingForCachedData)
                    {
                        isWaitingForCachedData = true;
                        lastRepaintTime = DateTime.Now;
                    }
                }

                if (cachedDataObject != null)
                {
                    EditorGUILayout.Space();

                    if (GUILayout.Button(MetadataEditor.ScriptableObjectEditor.PRINT_RUNTIME_DATA_LABEL))
                    {
                        scriptable.DataDebugPrint();
                    }

                    EditorGUILayout.Space();

                    if (GUILayout.Button(MetadataEditor.ScriptableObjectEditor.RESET_RUNTIME_DATA_LABEL))
                    {
                        scriptable.ResetToDefaultState();
                        cachedDataObject = null;

                        // Start waiting for new data
                        isWaitingForCachedData = true;
                        lastRepaintTime = DateTime.Now;
                    }
                }
            }
            else
            {
                // Show variable names and types when not playing
                EditorGUILayout.LabelField(MetadataEditor.ScriptableObjectEditor.RUNTIME_DATA_STRUCTURE_LABEL, EditorStyles.boldLabel);

                dataObject = scriptable.GetDataObject();
                if (dataObject != null)
                {
                    Type dataType = dataObject.GetType();
                    FieldInfo[] fields = dataType.GetFields(BindingFlags.Public | BindingFlags.Instance);
                    PropertyInfo[] properties = dataType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                    EditorGUI.indentLevel++;

                    // Show fields
                    foreach (FieldInfo field in fields)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(ObjectNames.NicifyVariableName(field.Name), GUILayout.Width(150));
                        EditorGUILayout.LabelField($"({field.FieldType.Name})", GUILayout.Width(100));
                        EditorGUILayout.EndHorizontal();
                    }

                    // Show properties
                    foreach (PropertyInfo property in properties)
                    {
                        if (property.CanRead && property.CanWrite)
                        {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField(ObjectNames.NicifyVariableName(property.Name), GUILayout.Width(150));
                            EditorGUILayout.LabelField($"({property.PropertyType.Name})", GUILayout.Width(100));
                            EditorGUILayout.EndHorizontal();
                        }
                    }

                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space();
                EditorGUILayout.HelpBox(MetadataEditor.ScriptableObjectEditor.RUNTIME_DATA_MESSAGE, MessageType.Info);
            }
        }

        private void DrawSettingsScriptableGUI(ScriptableEditorDebbugable scriptable)
        {
            EditorGUILayout.LabelField(MetadataEditor.ScriptableObjectEditor.EDITOR_DATA_LABEL, EditorStyles.boldLabel);

            // Always show editor data (editable in edit mode, read-only in play mode)
            var settings = (ScriptableSettings<Data>)scriptable;
            object editorDataObject = GetEditorData(settings);

            if (editorDataObject != null)
            {
                bool wasEnabled = GUI.enabled;
                GUI.enabled = !Application.isPlaying; // Only editable when not playing
                DrawDataFieldsWithReflection(editorDataObject, !Application.isPlaying);
                GUI.enabled = wasEnabled;
            }

            // Show runtime data during play mode
            if (Application.isPlaying)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(MetadataEditor.ScriptableObjectEditor.RUNTIME_DATA_LABEL, EditorStyles.boldLabel);

                dataObject = scriptable.GetDataObject();
                if (dataObject != null)
                {
                    DrawDataFieldsWithReflection(dataObject, true);

                    // Apply changes if data was modified
                    if (cachedDataObject != null && !ReflectionUtility.AreObjectsEqual(dataObject, cachedDataObject))
                    {
                        ReflectionUtility.CopyObjectData(dataObject, scriptable.GetDataObject());
                        cachedDataObject = scriptable.GetDataObject();
                        EditorUtility.SetDirty(scriptable);
                    }
                    else
                    {
                        cachedDataObject = scriptable.GetDataObject();
                    }
                }

                EditorGUILayout.HelpBox(MetadataEditor.ScriptableObjectEditor.CHANGES_RUNTIME_MESSAGE, MessageType.Info);
            }

            if (GUILayout.Button(MetadataEditor.ScriptableEditorDebbugablCustomEditor.RAW_DATA_PRINT_LABEL))
            {
                scriptable.DataDebugPrint();
            }
        }

        // Helper method to get editor data from ScriptableSettings
        private object GetEditorData(ScriptableSettings<Data> settings)
        {
            // Use reflection to access the private editorData field
            FieldInfo editorDataField = typeof(ScriptableSettings<Data>).GetField(MetadataEditor.ScriptableObjectEditor.EDITOR_DATA_FIELD,
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (editorDataField != null)
            {
                return editorDataField.GetValue(settings);
            }

            return null;
        }

        private void DrawDataFieldsWithReflection(object data, bool editable)
        {
            if (data == null) return;

            Type dataType = data.GetType();
            FieldInfo[] fields = dataType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo[] properties = dataType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Store current values for change detection
            Dictionary<string, object> currentValues = new Dictionary<string, object>();

            GUI.enabled = editable;

            // Draw fields
            foreach (FieldInfo field in fields)
            {
                object value = field.GetValue(data);
                currentValues[field.Name] = value;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(ObjectNames.NicifyVariableName(field.Name), GUILayout.Width(150));

                object newValue = DrawFieldForType(value, field.FieldType, field.Name, editable);

                if (editable && !Equals(newValue, value))
                {
                    field.SetValue(data, newValue);
                }

                EditorGUILayout.EndHorizontal();
            }

            // Draw properties (only those with public getters and setters)
            foreach (PropertyInfo property in properties)
            {
                if (property.CanRead && property.CanWrite)
                {
                    object value = property.GetValue(data);
                    currentValues[property.Name] = value;

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(ObjectNames.NicifyVariableName(property.Name), GUILayout.Width(150));

                    object newValue = DrawFieldForType(value, property.PropertyType, property.Name, editable);

                    if (editable && !Equals(newValue, value))
                    {
                        property.SetValue(data, newValue);
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }

            GUI.enabled = true;

            // Update field values for change detection
            fieldValues = currentValues;
        }

        private object DrawFieldForType(object value, System.Type type, string fieldName, bool editable)
        {
            // Handle null values by providing defaults
            if (value == null)
            {
                // Create default instance for value types
                if (type.IsValueType && !type.IsEnum)
                {
                    value = Activator.CreateInstance(type);
                }
            }

            // Basic types
            if (type == typeof(int))
            {
                return EditorGUILayout.IntField((int)(value ?? 0));
            }
            else if (type == typeof(float))
            {
                return EditorGUILayout.FloatField((float)(value ?? 0f));
            }
            else if (type == typeof(double))
            {
                return EditorGUILayout.DoubleField((double)(value ?? 0d));
            }
            else if (type == typeof(string))
            {
                return EditorGUILayout.TextField((string)(value ?? ""));
            }
            else if (type == typeof(bool))
            {
                return EditorGUILayout.Toggle((bool)(value ?? false));
            }
            // Unity math types
            else if (type == typeof(Vector2))
            {
                return EditorGUILayout.Vector2Field("", (Vector2)(value ?? Vector2.zero));
            }
            else if (type == typeof(Vector3))
            {
                return EditorGUILayout.Vector3Field("", (Vector3)(value ?? Vector3.zero));
            }
            else if (type == typeof(Vector4))
            {
                return EditorGUILayout.Vector4Field("", (Vector4)(value ?? Vector4.zero));
            }
            else if (type == typeof(Quaternion))
            {
                Quaternion quat = (Quaternion)(value ?? Quaternion.identity);
                Vector4 vec = new Vector4(quat.x, quat.y, quat.z, quat.w);
                Vector4 result = EditorGUILayout.Vector4Field("", vec);
                return new Quaternion(result.x, result.y, result.z, result.w);
            }
            else if (type == typeof(Matrix4x4))
            {
                // Matrix is complex, show as read-only for now
                GUI.enabled = false;
                EditorGUILayout.TextField(value?.ToString() ?? MetadataEditor.ScriptableObjectEditor.MATRIX_4X4_FIELD);
                GUI.enabled = true;
                return value;
            }
            // Unity common types
            else if (type == typeof(Color))
            {
                return EditorGUILayout.ColorField((Color)(value ?? Color.white));
            }
            else if (type == typeof(Color32))
            {
                Color color = (Color)(value ?? new Color32(255, 255, 255, 255));
                Color result = EditorGUILayout.ColorField(color);
                return (Color32)result;
            }
            else if (type == typeof(Rect))
            {
                return EditorGUILayout.RectField((Rect)(value ?? new Rect(0, 0, 100, 100)));
            }
            else if (type == typeof(Bounds))
            {
                return EditorGUILayout.BoundsField((Bounds)(value ?? new Bounds(Vector3.zero, Vector3.one)));
            }
            else if (type == typeof(AnimationCurve))
            {
                return EditorGUILayout.CurveField((AnimationCurve)(value ?? new AnimationCurve()));
            }
            else if (type == typeof(Gradient))
            {
                return EditorGUILayout.GradientField((Gradient)(value ?? new Gradient()));
            }
            // Unity object types
            else if (typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                return EditorGUILayout.ObjectField((UnityEngine.Object)value, type, true);
            }
            // Enum types
            else if (type.IsEnum)
            {
                return EditorGUILayout.EnumPopup((Enum)(value ?? System.Enum.GetValues(type).GetValue(0)));
            }
            // LayerMask type (special handling)
            else if (type == typeof(LayerMask))
            {
                LayerMask mask = (LayerMask)(value ?? new LayerMask());
                int intValue = EditorGUILayout.MaskField("", mask.value, UnityEditorInternal.InternalEditorUtility.layers);
                return (LayerMask)intValue;
            }
            // Arrays and lists
            else if (type.IsArray)
            {
                DrawArrayField(value, type, fieldName, editable);
                return value;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                DrawListField(value, type, fieldName, editable);
                return value;
            }
            // Serializable structs and classes
            else if (type.IsValueType || (type.IsClass && type.IsSerializable))
            {
                return DrawSerializableField(value, type, fieldName, editable);
            }
            else
            {
                // For unsupported types, show a read-only text representation
                GUI.enabled = false;
                EditorGUILayout.TextField(value?.ToString() ?? "null");
                GUI.enabled = true;
                return value;
            }
        }

        private void DrawArrayField(object value, Type arrayType, string fieldName, bool editable)
        {
            Array array = value as Array;
            Type elementType = arrayType.GetElementType();

            EditorGUILayout.LabelField($"{fieldName} [{array?.Length ?? 0}]", EditorStyles.boldLabel);

            if (array != null)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < array.Length; i++)
                {
                    object elementValue = array.GetValue(i);
                    object newValue = DrawFieldForType(elementValue, elementType, $"{MetadataEditor.ScriptableObjectEditor.ELEMENT_FIELD} {i}", editable);

                    if (editable && !Equals(newValue, elementValue))
                    {
                        array.SetValue(newValue, i);
                    }
                }
                EditorGUI.indentLevel--;
            }
        }

        private void DrawListField(object value, Type listType, string fieldName, bool editable)
        {
            Type elementType = listType.GetGenericArguments()[0];
            System.Collections.IList list = value as System.Collections.IList;

            EditorGUILayout.LabelField($"{fieldName} [{list?.Count ?? 0}]", EditorStyles.boldLabel);

            if (list != null)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < list.Count; i++)
                {
                    object elementValue = list[i];
                    object newValue = DrawFieldForType(elementValue, elementType, $"{MetadataEditor.ScriptableObjectEditor.ELEMENT_FIELD} {i}", editable);

                    if (editable && !Equals(newValue, elementValue))
                    {
                        list[i] = newValue;
                    }
                }
                EditorGUI.indentLevel--;
            }
        }

        private object DrawSerializableField(object value, Type type, string fieldName, bool editable)
        {
            EditorGUILayout.LabelField(ObjectNames.NicifyVariableName(fieldName), EditorStyles.boldLabel);

            if (value == null)
            {
                value = Activator.CreateInstance(type);
            }

            EditorGUI.indentLevel++;

            // Use reflection to draw all fields of the serializable type
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
            {
                object fieldValue = field.GetValue(value);
                object newValue = DrawFieldForType(fieldValue, field.FieldType, field.Name, editable);

                if (editable && !Equals(newValue, fieldValue))
                {
                    field.SetValue(value, newValue);
                }
            }

            EditorGUI.indentLevel--;
            return value;
        }

        // Called at 10 frames per second to keep inspector responsive
        private void OnInspectorUpdate()
        {
            // If we're waiting for cached data and enough time has passed since last repaint
            if (isWaitingForCachedData && (DateTime.Now - lastRepaintTime).TotalSeconds >= repaintDelaySeconds)
            {
                Repaint();
                lastRepaintTime = DateTime.Now;

                // Check if we now have cached data
                var scriptable = (ScriptableEditorDebbugable)target;
                if (scriptable.GetDataObject() != null)
                {
                    cachedDataObject = scriptable.GetDataObject();
                    isWaitingForCachedData = false;
                }
            }
        }
    }
}
#endif