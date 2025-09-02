#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Reflection;
using Thisaislan.Scriptables.Editor.Utilities;
using Thisaislan.Scriptables.Editor.Abstracts;
using Thisaislan.Scriptables.Abstracts;
using System.Collections.Generic;
using System;

namespace Thisaislan.Scriptables.Editor
{
    [CustomEditor(typeof(ScriptableEditorDebbugable), true)]
    internal class ScriptableEditorDebbugableEditor : UnityEditor.Editor
    {
        private object dataObject;
        private object cachedDataObject;
        private bool isWaitingForCachedData = false;
        private DateTime lastRepaintTime;
        private const double repaintDelaySeconds = 0.5;
        
        // Track objects and types being drawn to prevent infinite recursion
        private HashSet<object> objectsBeingDrawn = new HashSet<object>();
        private HashSet<Type> typesBeingDrawn = new HashSet<Type>();
        private int recursionDepth = 0;
        private const int maxRecursionDepth = 10;

        // Dictionary to map system type names to simplified names
        private static readonly Dictionary<string, string> typeNameMap = new Dictionary<string, string>
        {
            { "Int32", "int" },
            { "Single", "float" },
            { "Double", "double" },
            { "Boolean", "bool" },
            { "String", "string" },
            { "Char", "char" },
            { "Byte", "byte" },
            { "SByte", "sbyte" },
            { "Int16", "short" },
            { "Int64", "long" },
            { "UInt16", "ushort" },
            { "UInt32", "uint" },
            { "UInt64", "ulong" },
            { "Decimal", "decimal" }
        };

        public override void OnInspectorGUI()
        {
            try
            {
                var scriptable = (ScriptableEditorDebbugable)target;

                serializedObject.Update();

                // Draw script field and other properties as disabled
                DrawDefaultInspectorWithoutEditorData();

                EditorGUILayout.Space();
                
                // Draw the editor data with a bold header and all fields expanded
                DrawEditorDataAsExpanded(serializedObject);

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
                
                serializedObject.ApplyModifiedProperties();
            }
            catch (Exception e)
            {
                // Check if it's an ExitGUIException (which is normal for object pickers)
                if (e is ExitGUIException)
                {
                    // Re-throw ExitGUIException to let Unity handle it properly
                    throw;
                }
                else
                {
                    EditorGUILayout.HelpBox($"{Consts.CUSTOM_INSPECTOR_ERROR}: {e.Message}", MessageType.Error);
                    
                    Printer.PrintWarning($"{Consts.EDITOR_DEBUG_ERROR}: {e}\n{e.StackTrace}");
                }
            }
            finally
            {
                // Clear the tracking after each full draw cycle
                objectsBeingDrawn.Clear();
                typesBeingDrawn.Clear();
                recursionDepth = 0;
            }
        }

        private void DrawEditorDataAsExpanded(SerializedObject serializedObject)
        {
            SerializedProperty editorDataProperty = serializedObject.FindProperty(Consts.EDITOR_DATA_FIELD);
            
            if (editorDataProperty != null)
            {
                Type dataType = GetEditorDataType(serializedObject);
                string label = dataType != null ? dataType.Name : Consts.EDITOR_DATA_LABEL;

                // Draw bold header
                EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
                
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
                            break;
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
                Printer.PrintWarning($"{Consts.CUSTOM_INSPECTOR_ERROR}: {e}");
            }
        }

        private void DrawDefaultInspectorWithoutEditorData()
        {
            SerializedProperty property = serializedObject.GetIterator();
            bool enterChildren = true;

            while (property.NextVisible(enterChildren))
            {
                enterChildren = false;

                // Skip the editorData field as we'll handle it separately
                if (property.name == Consts.EDITOR_DATA_FIELD)
                    continue;

                GUI.enabled = false;
                EditorGUILayout.PropertyField(property, true);
                GUI.enabled = true;
            }
        }

        // Helper method to get editorData type robustly
        private Type GetEditorDataType(SerializedObject serializedObject)
        {
            var targetObj = serializedObject.targetObject;
            if (targetObj == null) return null;

            // Try to get the value of the private field editorData
            var type = targetObj.GetType();
            var editorDataField = type.GetField(Consts.EDITOR_DATA_FIELD, BindingFlags.NonPublic | BindingFlags.Instance);

            if (editorDataField != null)
            {
                var value = editorDataField.GetValue(targetObj);
                if (value != null)
                    return value.GetType();
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

        // Keep your existing DrawDefaultInspectorWithCustomEditorDataLabel method as is
        public void DrawDefaultInspectorWithCustomEditorDataLabel(SerializedObject serializedObject)
        {
            SerializedProperty property = serializedObject.GetIterator();
            bool enterChildren = true;

            while (property.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (property.name == Consts.EDITOR_DATA_FIELD)
                {
                    Type dataType = GetEditorDataType(serializedObject);

                    string label = dataType != null ? dataType.Name : Consts.EDITOR_DATA_LABEL;

                    EditorGUILayout.PropertyField(property, new GUIContent(label), true);
                }
                else
                {
                    GUI.enabled = false;
                    EditorGUILayout.PropertyField(property, true);
                    GUI.enabled = true;
                }
            }
        }

        private void DrawRuntimeScriptableGUI(ScriptableEditorDebbugable scriptable)
        {
            if (Application.isPlaying)
            {
                EditorGUILayout.LabelField(Consts.RUNTIME_DATA_LABEL, EditorStyles.boldLabel);

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
                    EditorGUILayout.HelpBox(Consts.WAITING_FOR_DATA_INITIALIZATION_LABEL, MessageType.Info);

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

                    if (GUILayout.Button(Consts.PRINT_RUNTIME_DATA_LABEL))
                    {
                        scriptable.PrintDataDebugEditor();
                    }

                    EditorGUILayout.Space();

                    if (GUILayout.Button(Consts.RESET_RUNTIME_DATA_LABEL))
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
                EditorGUILayout.LabelField(Consts.RUNTIME_DATA_STRUCTURE_LABEL, EditorStyles.boldLabel);

                dataObject = scriptable.GetDataObject();
                if (dataObject != null)
                {
                    Type dataType = dataObject.GetType();
                    FieldInfo[] fields = dataType.GetFields(BindingFlags.Public | BindingFlags.Instance);
                    PropertyInfo[] properties = dataType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                    EditorGUI.indentLevel++;

                    // Use a fixed ratio for the layout (e.g., 40% for names, 60% for types)
                    float totalWidth = EditorGUIUtility.currentViewWidth - 50f; // Account for margins
                    float nameWidth = totalWidth * 0.4f;
                    float typeWidth = totalWidth * 0.6f;

                    // Show fields
                    foreach (FieldInfo field in fields)
                    {
                        EditorGUILayout.BeginHorizontal();
                        
                        // Field name with proportional width
                        EditorGUILayout.LabelField(ObjectNames.NicifyVariableName(field.Name), GUILayout.Width(nameWidth));
                        
                        // Simplified type name
                        string typeName = SimplifyTypeName(field.FieldType.Name);

                        // Add self reference information
                        if (field.FieldType == dataType)
                        {
                            typeName += $" - {Consts.SELF_REFERENCE_NOT_SUPPORTED}";
                        }

                        // Type name with proportional width
                        EditorGUILayout.LabelField(typeName, GUILayout.Width(typeWidth));
                        
                        EditorGUILayout.EndHorizontal();
                    }

                    // Show properties
                    foreach (PropertyInfo property in properties)
                    {
                        if (property.CanRead && property.CanWrite)
                        {
                            EditorGUILayout.BeginHorizontal();
                            
                            // Property name with proportional width
                            EditorGUILayout.LabelField(ObjectNames.NicifyVariableName(property.Name), GUILayout.Width(nameWidth));
                            
                            // Simplified type name
                            string typeName = SimplifyTypeName(property.PropertyType.Name);

                            // Add self reference information
                            if (property.PropertyType == dataType)
                            {
                                typeName += $" - {Consts.SELF_REFERENCE_NOT_SUPPORTED}";
                            }

                            // Type name with proportional width
                            EditorGUILayout.LabelField(typeName, GUILayout.Width(typeWidth));
                            
                            EditorGUILayout.EndHorizontal();
                        }
                    }

                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space();
                EditorGUILayout.HelpBox(Consts.RUNTIME_DATA_MESSAGE, MessageType.Info);
            }
        }

        // Helper method to simplify type names
        private string SimplifyTypeName(string originalName)
        {
            // Check if we have a simplified name for this type
            if (typeNameMap.TryGetValue(originalName, out string simplifiedName))
            {
                return simplifiedName;
            }
            
            // For generic types, we need to handle them specially
            if (originalName.Contains("`"))
            {
                int backtickIndex = originalName.IndexOf('`');
                string baseName = originalName.Substring(0, backtickIndex);
                
                // Try to simplify the base name
                if (typeNameMap.TryGetValue(baseName, out string simplifiedBaseName))
                {
                    return simplifiedBaseName;
                }
                
                return baseName;
            }
            
            // Return the original name if no simplification is available
            return originalName;
        }

        private void DrawSettingsScriptableGUI(ScriptableEditorDebbugable scriptable)
        {
            EditorGUILayout.LabelField(Consts.EDITOR_DATA_LABEL, EditorStyles.boldLabel);

            // Always show editor data (editable in edit mode, read-only in play mode)
            var settings = (ScriptableSettings<Data>)scriptable;
            object editorDataObject = GetEditorData(settings);

            if (editorDataObject != null)
            {
                bool wasEnabled = GUI.enabled;
                GUI.enabled = !Application.isPlaying; // Only editable when not playing
                
                // For ScriptableSettings in edit mode, we need to handle self-references differently
                if (!Application.isPlaying)
                {
                    DrawSettingsDataFieldsWithReflection(editorDataObject, true, settings.GetType());
                }
                else
                {
                    DrawDataFieldsWithReflection(editorDataObject, !Application.isPlaying);
                }
                
                GUI.enabled = wasEnabled;
            }

            // Show runtime data during play mode
            if (Application.isPlaying)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(Consts.RUNTIME_DATA_LABEL, EditorStyles.boldLabel);

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

                EditorGUILayout.HelpBox(Consts.CHANGES_RUNTIME_MESSAGE, MessageType.Info);
            }

            if (GUILayout.Button(Consts.RAW_DATA_PRINT_LABEL))
            {
                scriptable.PrintDataDebugEditor();
            }
        }

        // Special method for drawing ScriptableSettings data that handles self-references
        private void DrawSettingsDataFieldsWithReflection(object data, bool editable, Type settingsType)
        {
            if (data == null) return;
            
            Type dataType = data.GetType();
            
            // Check for circular references to prevent infinite recursion
            if (objectsBeingDrawn.Contains(data) || typesBeingDrawn.Contains(dataType))
            {
                EditorGUILayout.HelpBox($"{Consts.CIRCULAR_REFERENCE_DETECTED} ({dataType.Name}). {Consts.CANNOT_DISPLAY_THIS_OBJECT}.", MessageType.Warning);
                return;
            }
            
            // Check recursion depth to prevent stack overflow
            if (recursionDepth > maxRecursionDepth)
            {
                EditorGUILayout.HelpBox(Consts.MAX_RECURSION_EXCEEDED_MESSAGE, MessageType.Error);
                return;
            }
            
            recursionDepth++;
            objectsBeingDrawn.Add(data);
            typesBeingDrawn.Add(dataType);

            try
            {
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
                    EditorGUILayout.LabelField(ObjectNames.NicifyVariableName(field.Name), GUILayout.Width(200));

                    // For ScriptableSettings, we need to check if this is a self-reference to the same data type
                    object newValue = DrawSettingsFieldForType(value, field.FieldType, field.Name, editable, dataType, settingsType);

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
                        EditorGUILayout.LabelField(ObjectNames.NicifyVariableName(property.Name), GUILayout.Width(200));

                        object newValue = DrawSettingsFieldForType(value, property.PropertyType, property.Name, editable, dataType, settingsType);

                        if (editable && !Equals(newValue, value))
                        {
                            property.SetValue(data, newValue);
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                }

                GUI.enabled = true;
            }
            finally
            {
                objectsBeingDrawn.Remove(data);
                typesBeingDrawn.Remove(dataType);
                recursionDepth--;
            }
        }

        // Special method for drawing fields in ScriptableSettings that handles self-references
        private object DrawSettingsFieldForType(object value, System.Type type, string fieldName, bool editable, Type parentType, Type settingsType)
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

            // Check if this is a recursive type that could cause infinite loops
            if (value != null && (objectsBeingDrawn.Contains(value) || typesBeingDrawn.Contains(type)))
            {
                GUI.enabled = false;
                EditorGUILayout.TextField(Consts.CIRCULAR_REFERENCE_DETECTED);
                GUI.enabled = editable;
                return value;
            }
            
            // Special check for ScriptableSettings: if the field type is the same as the parent type, treat as self-reference
            if (parentType != null && type == parentType)
            {
                GUI.enabled = false;
                EditorGUILayout.TextField(Consts.SELF_REFERENCE_NOT_SUPPORTED);
                GUI.enabled = editable;
                return value;
            }
            
            // Special check for ScriptableSettings: if the field type is the same as the settings data type
            Type settingsDataType = GetSettingsDataType(settingsType);
            if (settingsDataType != null && type == settingsDataType)
            {
                GUI.enabled = false;
                EditorGUILayout.TextField(Consts.SELF_REFERENCE_NOT_SUPPORTED);
                GUI.enabled = editable;
                return value;
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
                EditorGUILayout.TextField(value?.ToString() ?? Consts.MATRIX_4X4_FIELD);
                GUI.enabled = editable;
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
                DrawSettingsArrayField(value, type, fieldName, editable, parentType, settingsType);
                return value;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                DrawSettingsListField(value, type, fieldName, editable, parentType, settingsType);
                return value;
            }
            // Serializable structs and classes
            else if (type.IsValueType || (type.IsClass && type.IsSerializable))
            {
                // Check if we're about to enter a circular reference
                if (typesBeingDrawn.Contains(type))
                {
                    GUI.enabled = false;
                    EditorGUILayout.TextField(Consts.CIRCULAR_REFERENCE_PREVENTED);
                    GUI.enabled = editable;
                    return value;
                }
                
                return DrawSettingsSerializableField(value, type, fieldName, editable, settingsType);
            }
            else
            {
                // For unsupported types, show a read-only text representation
                GUI.enabled = false;
                EditorGUILayout.TextField(value?.ToString() ?? "null");
                GUI.enabled = editable;
                return value;
            }
        }

        // Helper method to get the data type from a ScriptableSettings type
        private Type GetSettingsDataType(Type settingsType)
        {
            // ScriptableSettings<T> inherits from ScriptableEditorDebbugable
            // We need to find the generic argument T
            Type baseType = settingsType.BaseType;
            while (baseType != null && baseType != typeof(object))
            {
                if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(ScriptableSettings<>))
                {
                    return baseType.GetGenericArguments()[0];
                }
                baseType = baseType.BaseType;
            }
            return null;
        }

        private void DrawSettingsArrayField(object value, Type arrayType, string fieldName, bool editable, Type parentType, Type settingsType)
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
                    object newValue = DrawSettingsFieldForType(elementValue, elementType, $"{Consts.ELEMENT_FIELD} {i}", editable, parentType, settingsType);

                    if (editable && !Equals(newValue, elementValue))
                    {
                        array.SetValue(newValue, i);
                    }
                }
                EditorGUI.indentLevel--;
            }
        }

        private void DrawSettingsListField(object value, Type listType, string fieldName, bool editable, Type parentType, Type settingsType)
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
                    object newValue = DrawSettingsFieldForType(elementValue, elementType, $"{Consts.ELEMENT_FIELD} {i}", editable, parentType, settingsType);

                    if (editable && !Equals(newValue, elementValue))
                    {
                        list[i] = newValue;
                    }
                }
                EditorGUI.indentLevel--;
            }
        }

        private object DrawSettingsSerializableField(object value, Type type, string fieldName, bool editable, Type settingsType)
        {
            EditorGUILayout.LabelField(ObjectNames.NicifyVariableName(fieldName), EditorStyles.boldLabel);

            if (value == null)
            {
                // Only create instances for value types, not for classes to prevent circular references
                if (type.IsValueType)
                {
                    value = Activator.CreateInstance(type);
                }
                else
                {
                    GUI.enabled = false;
                    EditorGUILayout.TextField("null");
                    GUI.enabled = editable;
                    return null;
                }
            }

            EditorGUI.indentLevel++;

            // Use reflection to draw all fields of the serializable type
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
            {
                object fieldValue = field.GetValue(value);
                object newValue = DrawSettingsFieldForType(fieldValue, field.FieldType, field.Name, editable, type, settingsType);

                if (editable && !Equals(newValue, fieldValue))
                {
                    field.SetValue(value, newValue);
                }
            }

            EditorGUI.indentLevel--;
            return value;
        }

        // Helper method to get editor data from ScriptableSettings
        private object GetEditorData(ScriptableSettings<Data> settings)
        {
            // Use reflection to access the private editorData field
            FieldInfo editorDataField = typeof(ScriptableSettings<Data>).GetField(Consts.EDITOR_DATA_FIELD,
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
            
            // Check for circular references to prevent infinite recursion
            if (objectsBeingDrawn.Contains(data) || typesBeingDrawn.Contains(dataType))
            {
                EditorGUILayout.HelpBox($"{Consts.CIRCULAR_REFERENCE_DETECTED} ({dataType.Name}). {Consts.CANNOT_DISPLAY_THIS_OBJECT}.", MessageType.Warning);
                return;
            }
            
            // Check recursion depth to prevent stack overflow
            if (recursionDepth > maxRecursionDepth)
            {
                EditorGUILayout.HelpBox(Consts.MAX_RECURSION_EXCEEDED_MESSAGE, MessageType.Error);
                return;
            }
            
            recursionDepth++;
            objectsBeingDrawn.Add(data);
            typesBeingDrawn.Add(dataType);

            try
            {
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
                    EditorGUILayout.LabelField(ObjectNames.NicifyVariableName(field.Name), GUILayout.Width(200));

                    object newValue = DrawFieldForType(value, field.FieldType, field.Name, editable, dataType);

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
                        EditorGUILayout.LabelField(ObjectNames.NicifyVariableName(property.Name), GUILayout.Width(200));

                        object newValue = DrawFieldForType(value, property.PropertyType, property.Name, editable, dataType);

                        if (editable && !Equals(newValue, value))
                        {
                            property.SetValue(data, newValue);
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                }

                GUI.enabled = true;
            }
            finally
            {
                objectsBeingDrawn.Remove(data);
                typesBeingDrawn.Remove(dataType);
                recursionDepth--;
            }
        }

        private object DrawFieldForType(object value, System.Type type, string fieldName, bool editable, Type parentType = null)
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

            // Check if this is a recursive type that could cause infinite loops
            if (value != null && (objectsBeingDrawn.Contains(value) || typesBeingDrawn.Contains(type)))
            {
                GUI.enabled = false;
                EditorGUILayout.TextField(Consts.CIRCULAR_REFERENCE_DETECTED);
                GUI.enabled = editable;
                return value;
            }
            
            // Special check: if the field type is the same as the parent type, treat as circular reference
            if (parentType != null && type == parentType)
            {
                GUI.enabled = false;
                EditorGUILayout.TextField(Consts.SELF_REFERENCE_NOT_SUPPORTED);
                GUI.enabled = editable;
                return value;
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
                EditorGUILayout.TextField(value?.ToString() ?? Consts.MATRIX_4X4_FIELD);
                GUI.enabled = editable;
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
                DrawArrayField(value, type, fieldName, editable, parentType);
                return value;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                DrawListField(value, type, fieldName, editable, parentType);
                return value;
            }
            // Serializable structs and classes
            else if (type.IsValueType || (type.IsClass && type.IsSerializable))
            {
                // Check if we're about to enter a circular reference
                if (typesBeingDrawn.Contains(type))
                {
                    GUI.enabled = false;
                    EditorGUILayout.TextField(Consts.CIRCULAR_REFERENCE_PREVENTED);
                    GUI.enabled = editable;
                    return value;
                }
                
                return DrawSerializableField(value, type, fieldName, editable);
            }
            else
            {
                // For unsupported types, show a read-only text representation
                GUI.enabled = false;
                EditorGUILayout.TextField(value?.ToString() ?? "null");
                GUI.enabled = editable;
                return value;
            }
        }

        private void DrawArrayField(object value, Type arrayType, string fieldName, bool editable, Type parentType)
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
                    object newValue = DrawFieldForType(elementValue, elementType, $"{Consts.ELEMENT_FIELD} {i}", editable, parentType);

                    if (editable && !Equals(newValue, elementValue))
                    {
                        array.SetValue(newValue, i);
                    }
                }
                EditorGUI.indentLevel--;
            }
        }

        private void DrawListField(object value, Type listType, string fieldName, bool editable, Type parentType)
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
                    object newValue = DrawFieldForType(elementValue, elementType, $"{Consts.ELEMENT_FIELD} {i}", editable);

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