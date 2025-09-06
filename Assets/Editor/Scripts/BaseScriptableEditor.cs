#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Thisaislan.Scriptables.Editor.Utilities;
using Thisaislan.Scriptables.Editor.Helpers;

namespace Thisaislan.Scriptables.Editor
{
    /// <summary>
    /// Base class for ScriptableObject editors with debugging capabilities
    /// </summary>
    internal abstract class BaseScriptableEditor : UnityEditor.Editor
    {
        protected object dataObject;
        protected object cachedDataObject;
        protected bool isWaitingForCachedData;
        protected DateTime lastRepaintTime;
        protected const double RepaintDelaySeconds = 0.5;
        protected ScriptableEditorHelper scriptableEditorHelper;
        protected HashSet<object> drawnObjects = new HashSet<object>();
        protected bool isSimpleType;
        private Dictionary<int, CustomDragState> dragStates = new Dictionary<int, CustomDragState>();

        /// <summary>
        /// Initializes the editor
        /// </summary>
        protected virtual void OnEnable()
        {
            dataObject = GetData();
            scriptableEditorHelper = new ScriptableEditorHelper();
            dragStates = new Dictionary<int, CustomDragState>();
        }

        /// <summary>
        /// Clean up when the editor is disabled
        /// </summary>
        protected virtual void OnDisable()
        {
            // Clean up drag states when the inspector is closed
            dragStates.Clear();
            GUIUtility.hotControl = 0;
        }

        /// <summary>
        /// Main method for drawing the custom inspector GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            try
            {
                serializedObject.Update();
                DrawScriptableGUI();
                serializedObject.ApplyModifiedProperties();
            }
            catch (Exception e) when (e is not ExitGUIException)
            {

            }
            finally
            {
                drawnObjects.Clear();
            }
        }

        /// <summary>
        /// Called periodically to refresh the inspector view when waiting for data
        /// </summary>
        protected virtual void OnInspectorUpdate()
        {
            if (isWaitingForCachedData && (DateTime.Now - lastRepaintTime).TotalSeconds >= RepaintDelaySeconds)
            {
                Repaint();
                lastRepaintTime = DateTime.Now;

                if (GetData() != null)
                {
                    cachedDataObject = GetData();
                    isWaitingForCachedData = false;
                }
            }
        }

        /// <summary>
        /// Main method for drawing scriptable object GUI based on current state
        /// </summary>
        protected virtual void DrawScriptableGUI()
        {

            if (Application.isPlaying)
            {
                DrawRuntimeState();
            }
            else
            {
                DrawEditTimeState();
            }
        }

        /// <summary>
        /// Draws the runtime state UI when the application is playing
        /// </summary>
        protected virtual void DrawRuntimeState()
        {
            dataObject = GetData();

            EditorGUILayout.LabelField(Consts.RuntimeDataLabel, EditorStyles.boldLabel);

            if (!isSimpleType)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.LabelField(Consts.TypeLiteral, TypeNameSimplifier.SimplifyTypeName(dataObject.GetType().Name));
            }

            if (dataObject == null)
            {
                EditorGUILayout.HelpBox(Consts.WaitingForDataInitializationLabel, MessageType.Info);
                isWaitingForCachedData = true;
                lastRepaintTime = DateTime.Now;
                return;
            }

            EditorGUI.indentLevel++;

            Type valueType = dataObject.GetType();
            
            // Check if this is a Unity type with a native drawer
            if (IsUnityTypeWithNativeDrawer(valueType))
            {
                DrawUnityTypeValue(dataObject, valueType);
            }
            else if (isSimpleType)
            {
                DrawSimpleTypeValue(dataObject, valueType);
            }
            else
            {
                // Check if this is a supported serializable type
                if (IsSupportedType(valueType))
                {
                    scriptableEditorHelper.DrawDataFieldsWithCircularProtection(dataObject, true, valueType, drawnObjects);
                }
                else
                {
                    EditorGUILayout.Space();
                    // Show message for unsupported types
                    EditorGUILayout.HelpBox(
                        $"{Consts.TypeLiteral} '{TypeNameSimplifier.SimplifyTypeName(valueType.Name)}' {Consts.UnsupportedTypeMessage}.", 
                        MessageType.Info
                    );

                    EditorGUILayout.Space();
                    
                    EditorGUILayout.LabelField(Consts.ReactiveVariableName, TypeNameSimplifier.SimplifyTypeName(valueType.Name));
                }
            }

            EditorGUI.indentLevel--;

            if (!isSimpleType)
            {
                EditorGUI.indentLevel--;
            }

            HandleDataChanges();

            EditorGUILayout.Space();

            DrawRuntimeButtons();
        }

        /// <summary>
        /// Draws the edit-time state UI when the application is not playing
        /// </summary>
        protected virtual void DrawEditTimeState()
        {
            EditorGUILayout.LabelField(Consts.RuntimeDataStructureLabel, EditorStyles.boldLabel);

            if (!isSimpleType)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.LabelField(Consts.TypeLiteral, TypeNameSimplifier.SimplifyTypeName(dataObject.GetType().Name));
            }

            dataObject = GetEditorData();

            if (dataObject != null)
            {
                if (isSimpleType)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField(Consts.TypeLiteral, TypeNameSimplifier.SimplifyTypeName(dataObject.GetType().Name));
                    EditorGUI.indentLevel--;
                }
                else
                {
                    scriptableEditorHelper.DrawDataStructureSimple(GetRightTypeToDraw(dataObject.GetType()));
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(Consts.RuntimeDataMessage, MessageType.Info);

            if (!isSimpleType)
            {
                EditorGUI.indentLevel--;
            }
        }

        /// <summary>
        /// Draws a field for simple types with appropriate Unity editor controls
        /// </summary>
        protected virtual Type GetRightTypeToDraw(Type valueType)
        {
            if (valueType == typeof(Quaternion))
            {
                return typeof(Vector3); // Just to show in a nice way in the inspector
            }

            return valueType;
        }

        /// <summary>
        /// Draws a field for simple types with appropriate Unity editor controls
        /// </summary>
        protected virtual void DrawSimpleTypeValue(object value, Type valueType)
        {
            EditorGUILayout.BeginHorizontal();

            // Create a rect for the label to enable click-and-drag functionality
            Rect labelRect = EditorGUILayout.GetControlRect();
            labelRect.width = 100;
            EditorGUI.LabelField(labelRect, Consts.ReactiveVariableName);

            // Create a rect for the value field
            Rect valueRect = EditorGUILayout.GetControlRect();

            object newValue = value;
            bool valueChanged = false;

            // Get or create a control ID for this field
            int controlId = GUIUtility.GetControlID(FocusType.Passive);

            // Handle different types with appropriate EditorGUI fields
            if (valueType == typeof(int))
            {
                newValue = EditorGUI.IntField(valueRect, (int)value);
                valueChanged = !newValue.Equals(value);
            }
            else if (valueType == typeof(float))
            {
                newValue = EditorGUI.FloatField(valueRect, (float)value);
                valueChanged = !newValue.Equals(value);
            }
            else if (valueType == typeof(double))
            {
                newValue = EditorGUI.DoubleField(valueRect, (double)value);
                valueChanged = !newValue.Equals(value);
            }
            else if (valueType == typeof(bool))
            {
                newValue = EditorGUI.Toggle(valueRect, (bool)value);
                valueChanged = !newValue.Equals(value);
            }
            else if (valueType == typeof(string))
            {
                newValue = EditorGUI.TextField(valueRect, (string)value);
                valueChanged = !newValue.Equals(value);
            }
            else if (valueType.IsEnum)
            {
                newValue = EditorGUI.EnumPopup(valueRect, (Enum)value);
                valueChanged = !newValue.Equals(value);
            }
            else
            {
                EditorGUI.LabelField(valueRect, value != null ? value.ToString() : Consts.NullLiteral);
            }

            if (valueChanged)
            {
                SetData(newValue);
                dataObject = newValue;
                EditorUtility.SetDirty(target);
            }

            EditorGUILayout.EndHorizontal();

            // Handle drag functionality for numeric types (only in runtime)
            if (Application.isPlaying && IsNumericType(valueType))
            {
                HandleNumericDrag(controlId, labelRect, valueType, ref value);
            }
        }

        /// <summary>
        /// Checks if a type is supported for runtime editing
        /// </summary>
        protected virtual bool IsSupportedType(Type type)
        {
            return scriptableEditorHelper.IsUnitySerializable(type) && 
                !typeof(Component).IsAssignableFrom(type) &&
                !type.IsAbstract &&
                !type.IsInterface;
        }

        /// <summary>
        /// Draws a field for Unity types with native editor controls
        /// </summary>
        protected virtual void DrawUnityTypeValue(object value, Type valueType)
        {
            EditorGUILayout.BeginHorizontal();

            // Create a rect for the label
            Rect labelRect = EditorGUILayout.GetControlRect();
            labelRect.width = 100;
            EditorGUI.LabelField(labelRect, Consts.ReactiveVariableName);

            // Create a rect for the value field
            Rect valueRect = EditorGUILayout.GetControlRect();
            
            // Use the shared helper
            var result = scriptableEditorHelper.DrawUnityTypeFieldInternal(value, valueType, null, false, valueRect);
            
            if (result.valueChanged)
            {
                SetData(result.newValue);
                dataObject = result.newValue;
                EditorUtility.SetDirty(target);
            }

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Checks if a type is a Unity type with a native editor drawer
        /// </summary>
        protected virtual bool IsUnityTypeWithNativeDrawer(Type type)
        {
            return type == typeof(Vector2) ||
                   type == typeof(Vector3) ||
                   type == typeof(Vector4) ||
                   type == typeof(Quaternion) ||
                   type == typeof(Color) ||
                   type == typeof(Color32) ||
                   type == typeof(Rect) ||
                   type == typeof(Bounds) ||
                   type == typeof(AnimationCurve) ||
                   type == typeof(Gradient) ||
                   typeof(UnityEngine.Object).IsAssignableFrom(type);
        }

        /// <summary>
        /// Handles the click-and-drag functionality for numeric types
        /// </summary>
        private void HandleNumericDrag(int controlId, Rect labelRect, Type valueType, ref object value)
        {
            Event currentEvent = Event.current;
            
            if (!dragStates.ContainsKey(controlId))
            {
                dragStates[controlId] = new CustomDragState();
            }
            
            CustomDragState dragState = dragStates[controlId];
            
            switch (currentEvent.type)
            {
                case EventType.MouseDown:
                    if (currentEvent.button == 0 && labelRect.Contains(currentEvent.mousePosition))
                    {
                        dragState.mouseDownPosition = currentEvent.mousePosition;
                        dragState.originalValue = value;
                        dragState.isDragging = false;
                        dragState.controlId = controlId;
                        GUIUtility.hotControl = controlId;
                        currentEvent.Use();
                    }
                    break;
                    
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlId && !dragState.isDragging)
                    {
                        // Start dragging after a minimum movement threshold
                        if (Vector2.Distance(currentEvent.mousePosition, dragState.mouseDownPosition) > 3f)
                        {
                            dragState.isDragging = true;
                        }
                    }
                    
                    if (GUIUtility.hotControl == controlId && dragState.isDragging)
                    {
                        float dragSensitivity = GetDragSensitivity(valueType);
                        float delta = (currentEvent.mousePosition.x - dragState.mouseDownPosition.x) * dragSensitivity;
                        object newValue = CalculateNewValue(value, valueType, delta);
                        
                        if (!newValue.Equals(value))
                        {
                            SetData(newValue);
                            value = newValue;
                            dataObject = newValue;
                            EditorUtility.SetDirty(target);
                            GUI.changed = true;
                        }
                        
                        currentEvent.Use();
                    }
                    break;
                    
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlId)
                    {
                        dragState.isDragging = false;
                        GUIUtility.hotControl = 0;
                        currentEvent.Use();
                    }
                    break;
                    
                case EventType.Repaint:
                    if (labelRect.Contains(currentEvent.mousePosition) && IsNumericType(valueType))
                    {
                        EditorGUIUtility.AddCursorRect(labelRect, MouseCursor.SlideArrow);
                    }
                    break;
            }
        }
        
        /// <summary>
        /// Gets the appropriate drag sensitivity for different numeric types
        /// </summary>
        private float GetDragSensitivity(Type valueType)
        {
            if (valueType == typeof(float) || valueType == typeof(double) || valueType == typeof(decimal))
            {
                return 0.1f;
            }
            return 1f; // Default sensitivity for integer types
        }
        
        /// <summary>
        /// Calculates new value based on drag delta
        /// </summary>
        private object CalculateNewValue(object currentValue, Type valueType, float delta)
        {
            try
            {
                if (valueType == typeof(int)) return (int)currentValue + (int)delta;
                else if (valueType == typeof(float)) return (float)currentValue + delta;
                else if (valueType == typeof(double)) return (double)currentValue + delta;
                else if (valueType == typeof(byte)) return (byte)Mathf.Clamp((byte)currentValue + (byte)delta, byte.MinValue, byte.MaxValue);
                else if (valueType == typeof(sbyte)) return (sbyte)Mathf.Clamp((sbyte)currentValue + (sbyte)delta, sbyte.MinValue, sbyte.MaxValue);
                else if (valueType == typeof(short)) return (short)Mathf.Clamp((short)currentValue + (short)delta, short.MinValue, short.MaxValue);
                else if (valueType == typeof(ushort)) return (ushort)Mathf.Clamp((ushort)currentValue + (ushort)delta, ushort.MinValue, ushort.MaxValue);
                else if (valueType == typeof(uint)) return (uint)Mathf.Clamp((uint)currentValue + (uint)delta, uint.MinValue, uint.MaxValue);
                else if (valueType == typeof(long)) return (long)currentValue + (long)delta;
                else if (valueType == typeof(ulong)) return (ulong)Mathf.Clamp((ulong)currentValue + (ulong)delta, ulong.MinValue, ulong.MaxValue);
                else if (valueType == typeof(decimal)) return (decimal)currentValue + (decimal)((double)delta);
                else if (valueType == typeof(char)) 
                {
                    int charValue = (char)currentValue;
                    return (char)Mathf.Clamp(charValue + (int)delta, char.MinValue, char.MaxValue);
                }
                else if (valueType == typeof(Vector2)) 
                {
                    Vector2 vec = (Vector2)currentValue;
                    return new Vector2(vec.x + delta, vec.y);
                }
                else if (valueType == typeof(Vector3)) 
                {
                    Vector3 vec = (Vector3)currentValue;
                    return new Vector3(vec.x + delta, vec.y, vec.z);
                }
                else if (valueType == typeof(Vector4)) 
                {
                    Vector4 vec = (Vector4)currentValue;
                    return new Vector4(vec.x + delta, vec.y, vec.z, vec.w);
                }
                
                return currentValue;
            }
            catch
            {
                return currentValue;
            }
        }
        
        /// <summary>
        /// Checks if a type is numeric and supports drag operations
        /// </summary>
        private bool IsNumericType(Type type)
        {
            return type == typeof(int) || 
                   type == typeof(float) || 
                   type == typeof(double) ||
                   type == typeof(byte) ||
                   type == typeof(sbyte) ||
                   type == typeof(short) ||
                   type == typeof(ushort) ||
                   type == typeof(uint) ||
                   type == typeof(long) ||
                   type == typeof(ulong) ||
                   type == typeof(decimal) ||
                   type == typeof(char) ||
                   type == typeof(Vector2) ||
                   type == typeof(Vector3) ||
                   type == typeof(Vector4);
        }

        /// <summary>
        /// Handles data changes by comparing cached and current values
        /// </summary>
        protected virtual void HandleDataChanges()
        {
            if (cachedDataObject != null && !Equals(dataObject, cachedDataObject))
            {
                SetData(dataObject);
                cachedDataObject = dataObject;
                EditorUtility.SetDirty(target);
            }
            else
            {
                cachedDataObject = GetData();
            }
            isWaitingForCachedData = false;
        }

        /// <summary>
        /// Resets the runtime data to default values
        /// </summary>
        protected virtual void ResetRuntimeData()
        {
            GUI.FocusControl(null);
            ResetToDefaultState();
            cachedDataObject = null;
            isWaitingForCachedData = true;
            lastRepaintTime = DateTime.Now;
            Repaint();
        }

        /// <summary>
        /// Prints data debug information
        /// </summary>
        protected virtual void PrintDataDebug()
        {
            object data = GetData();

            if (data != null)
            {
                string json = JsonUtility.ToJson(data, true);
                if (string.IsNullOrEmpty(json) || json == "{}")
                {
                    json = FormatObjectManually(data);
                }

                Printer.PrintMessage($"{data.GetType().Name}:\n{json}");
            }
            else
            {
                Printer.PrintMessage(Consts.NoDataAvailableMessage);
            }
        }

        /// <summary>
        /// Formats an object manually for debugging
        /// </summary>
        protected virtual string FormatObjectManually(object data)
        {
            if (data == null) { return Consts.NullLiteral; }

            Type type = data.GetType();

            if (type.IsPrimitive || type == typeof(string))
            {
                return data.ToString();
            }

            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            if (fields.Length == 0) { return "{}"; }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("{");

            foreach (FieldInfo field in fields)
            {
                try
                {
                    object value = field.GetValue(data);
                    sb.AppendLine($"  {field.Name}: {value ?? Consts.NullLiteral}");
                }
                catch
                {
                    sb.AppendLine($"  {field.Name}: {Consts.AppendError}");
                }
            }

            sb.Append("}");
            return sb.ToString();
        }

        
        // Abstract methods that must be implemented by derived classes
        protected abstract object GetData();
        protected abstract object GetEditorData();
        protected abstract void SetData(object data);
        protected abstract void NotifyValue();
        protected abstract void ResetToDefaultState();
        protected abstract Type GetValueType();
        protected abstract void DrawRuntimeButtons();
    }
}
#endif