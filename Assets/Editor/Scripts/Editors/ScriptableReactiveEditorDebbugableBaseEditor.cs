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
        private Vector2 scrollPos;

        BaseScriptableReactiveEditorDebbugable scriptable;
        
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
            scriptable = target as BaseScriptableReactiveEditorDebbugable;

            base.OnEnable();

            EditorGUIUtility.SetIconForObject(target, Resources.Load<Texture2D>(EditorConsts.ScriptableReactiveIconName));
            isSimpleType = scriptableEditorHelper.IsSimpleType(scriptable.GetValueType());
            serialized = true;

            EditorApplication.update += RepaintIfNotNull;
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
                    scriptableEditorHelper.DrawEditorDataAsExpanded(serializedObject, EditorConsts.EditorValueField, EditorConsts.EditorValueLabel);
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
        /// Checks if this editor requires constant repaints in its current state.
        /// </summary>
        public override bool RequiresConstantRepaint()
        {
            return Application.isPlaying;
        }

        /// <summary>
        /// Clean up when the editor is disabled
        /// </summary>
        protected override void OnDisable()
        {
            EditorApplication.update -= RepaintIfNotNull;

            base.OnDisable();
        }

        /// <summary>
        /// Draws the editor value field
        /// </summary>
        private void DrawEditorValueField()
        {
            SerializedProperty editorValueProperty = serializedObject.FindProperty(EditorConsts.EditorValueField);
            Type valueType = scriptable.GetValueType();
            
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

                    EditorGUILayout.PropertyField(editorValueProperty, new GUIContent(EditorConsts.ReactiveVariableName), true);

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
                    $"{TypeNameSimplifier.SimplifyTypeName(valueType.Name)} - {EditorConsts.TypeNotSerialized}" :
                    $"{EditorConsts.ScriptableReactiveName}<{TypeNameSimplifier.SimplifyTypeName(valueType.Name)}> - {EditorConsts.TypeNotSerialized}";

                string combinedText = $"{EditorConsts.EditorValueLabel} ({message})";
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
            Vector3 newEuler = EditorGUILayout.Vector3Field(EditorConsts.ReactiveVariableName, euler);
            
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
                float x = property.FindPropertyRelative(EditorConsts.XCoordinatesPropertyName)?.floatValue ?? 0;
                float y = property.FindPropertyRelative(EditorConsts.YCoordinatesPropertyName)?.floatValue ?? 0;
                float z = property.FindPropertyRelative(EditorConsts.ZCoordinatesPropertyName)?.floatValue ?? 0;
                float w = property.FindPropertyRelative(EditorConsts.WCoordinatesPropertyName)?.floatValue ?? 1;
                
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
                SerializedProperty xProp = property.FindPropertyRelative(EditorConsts.XCoordinatesPropertyName);
                SerializedProperty yProp = property.FindPropertyRelative(EditorConsts.YCoordinatesPropertyName);
                SerializedProperty zProp = property.FindPropertyRelative(EditorConsts.ZCoordinatesPropertyName);
                SerializedProperty wProp = property.FindPropertyRelative(EditorConsts.WCoordinatesPropertyName);
                
                if (xProp != null) xProp.floatValue = quaternion.x;
                if (yProp != null) yProp.floatValue = quaternion.y;
                if (zProp != null) zProp.floatValue = quaternion.z;
                if (wProp != null) wProp.floatValue = quaternion.w;
            }
        }

        /// <summary>
        /// Draws runtime bottom
        /// </summary>
        protected override void DrawRuntimeBottom()
        {
            DrawRuntimeObservers();
            EditorGUILayout.Space();
            DrawRuntimeButtons();
        }

        /// <summary>
        /// Prints data debug information
        /// </summary>
        protected override void PrintDataDebug()
        {
            scriptable.PrintDataDebugEditor();
        }

        // Implementation of abstract methods
        protected override object GetData()
        { 
            return scriptable.GetRuntimeValue(); 
        }

        protected override object GetEditorData()
        { 
            return scriptable.GetEditorValue(); 
        }

        protected override void SetData(object data)
        { 
            scriptable.SetRuntimeValue(data);
        }

        protected override void NotifyValue()
        {
            scriptable.NotifyValue();
        }

        protected override void ResetToDefaultState()
        {
            scriptable.ResetToDefaultState();
        }

        protected override Type GetValueType()
        {
            return scriptable.GetValueType();
        }

        /// <summary>
        /// Draws runtime observers
        /// </summary>
        protected virtual void DrawRuntimeObservers()
        {
            EditorGUI.indentLevel++;

            Type type = scriptable.GetType();

            // Traverse the inheritance chain to find the private "action" field
            System.Reflection.FieldInfo actionField = null;
            
            while (type != null)
            {
                actionField = type.GetField(EditorConsts.ActionField, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                
                if (actionField != null)
                {
                    break;
                }
                
                type = type.BaseType;
            }

            Delegate actionDelegate = actionField?.GetValue(scriptable) as Delegate;

            EditorGUILayout.LabelField(EditorConsts.RuntimeObserversSectionTitle, EditorStyles.boldLabel);
            EditorGUILayout.Space(2);

            if (actionDelegate == null || actionDelegate.GetInvocationList().Length == 0)
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.LabelField(EditorConsts.NoObserversRegisteredMessage, GUILayout.Height(20));
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
                return;
            }

            DrawRuntimeObserversList(actionDelegate);

            EditorGUI.indentLevel--;
        }

        /// <summary>
        /// Draws the complete runtime observers list including header, scrollable body, and all observer rows.
        /// </summary>
        /// <param name="actionDelegate">The delegate containing all registered observers to display.</param>
        protected virtual void DrawRuntimeObserversList(Delegate actionDelegate)
        {
            Delegate[] invocationList = actionDelegate.GetInvocationList();

            float rowHeight = 18f;
            float headerHeight = 25f;
            float maxHeight = 180f;

            float minTargetWidth = 200f;
            float methodPreferredWidth = 200f;
            float minMethodWidth = 60f;

            float buttonWidth = 50f;
            float spacing = 2f;

            float contentHeight = rowHeight * invocationList.Length;
            bool needsScroll = contentHeight > maxHeight;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            DrawObserversListHeader(headerHeight, buttonWidth, spacing, minTargetWidth, methodPreferredWidth, minMethodWidth);

            BeginObserversListBody(needsScroll, maxHeight, contentHeight);

            DrawObserversListRows(
                invocationList,
                rowHeight,
                buttonWidth,
                spacing,
                minTargetWidth,
                methodPreferredWidth,
                minMethodWidth
            );

            EndObserversListBody(needsScroll);

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Draws the header row of the observers list with column labels (Observer and Method).
        /// </summary>
        /// <param name="headerHeight">Height of the header row.</param>
        /// <param name="buttonWidth">Fixed width reserved for the notify button column.</param>
        /// <param name="spacing">Horizontal spacing between columns.</param>
        /// <param name="minTargetWidth">Minimum width for the target (observer) column.</param>
        /// <param name="methodPreferredWidth">Preferred width for the method name column.</param>
        /// <param name="minMethodWidth">Minimum width for the method name column before it forces the target column to shrink.</param>
        private void DrawObserversListHeader(
            float headerHeight,
            float buttonWidth,
            float spacing,
            float minTargetWidth,
            float methodPreferredWidth,
            float minMethodWidth)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

            Rect headerRect = EditorGUILayout.GetControlRect(false, headerHeight);

            float availableWidth = headerRect.width - buttonWidth - spacing;

            ObserversListCalculateWidths(
                availableWidth,
                minTargetWidth,
                methodPreferredWidth,
                minMethodWidth,
                out float targetWidth,
                out float methodWidth
            );

            Rect targetRect = new Rect(headerRect.x, headerRect.y, targetWidth, headerRect.height);
            Rect methodRect = new Rect(targetRect.xMax, headerRect.y, methodWidth, headerRect.height);

            EditorGUI.LabelField(targetRect, EditorConsts.ObserverLabel, EditorStyles.boldLabel);
            EditorGUI.LabelField(methodRect, EditorConsts.MethodLabel, EditorStyles.boldLabel);

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Begins the scrollable or fixed-height container for the list of observer rows.
        /// </summary>
        /// <param name="needsScroll">If true, enables scrolling; otherwise uses a fixed-height layout.</param>
        /// <param name="maxHeight">Maximum height of the scrollable area when scrolling is enabled.</param>
        /// <param name="contentHeight">Total height of the content when scrolling is not needed.</param>
        private void BeginObserversListBody(bool needsScroll, float maxHeight, float contentHeight)
        {
            if (needsScroll)
            {
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(maxHeight));
            }
            else
            {
                EditorGUILayout.BeginVertical(GUILayout.Height(contentHeight));
            }
        }

        /// <summary>
        /// Ends the scrollable or fixed-height container for the observer list.
        /// </summary>
        /// <param name="needsScroll">If true, ends the scroll view; otherwise ends the vertical layout.</param>
        private void EndObserversListBody(bool needsScroll)
        {
            if (needsScroll)
                EditorGUILayout.EndScrollView();
            else
                EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Draws all observer rows from the provided delegate array.
        /// </summary>
        /// <param name="invocationList">Array of delegates representing each observer.</param>
        /// <param name="rowHeight">Height of each observer row.</param>
        /// <param name="buttonWidth">Fixed width of the notify button column.</param>
        /// <param name="spacing">Horizontal spacing between columns.</param>
        /// <param name="minTargetWidth">Minimum width for the target (observer) column.</param>
        /// <param name="methodPreferredWidth">Preferred width for the method name column.</param>
        /// <param name="minMethodWidth">Minimum width for the method name column.</param>
        private void DrawObserversListRows(
            Delegate[] invocationList,
            float rowHeight,
            float buttonWidth,
            float spacing,
            float minTargetWidth,
            float methodPreferredWidth,
            float minMethodWidth)
        {
            foreach (var del in invocationList)
            {
                if (del == null) continue;

                DrawObserversListRow(
                    del,
                    rowHeight,
                    buttonWidth,
                    spacing,
                    minTargetWidth,
                    methodPreferredWidth,
                    minMethodWidth
                );
            }
        }

        /// <summary>
        /// Draws a single observer row with target name, method name, and a select button.
        /// </summary>
        /// <param name="del">The delegate representing this observer.</param>
        /// <param name="rowHeight">Height of the row.</param>
        /// <param name="buttonWidth">Fixed width of the notify button column.</param>
        /// <param name="spacing">Horizontal spacing between columns.</param>
        /// <param name="minTargetWidth">Minimum width for the target (observer) column.</param>
        /// <param name="methodPreferredWidth">Preferred width for the method name column.</param>
        /// <param name="minMethodWidth">Minimum width for the method name column.</param>
        private void DrawObserversListRow(
            Delegate del,
            float rowHeight,
            float buttonWidth,
            float spacing,
            float minTargetWidth,
            float methodPreferredWidth,
            float minMethodWidth)
        {
            string targetName = del.Target != null
                ? del.Target.ToString()
                : EditorConsts.DefaultReactiveMethodName;

            string methodName = del.Method.Name;

            Rect rowRect = EditorGUILayout.GetControlRect(false, rowHeight);

            float availableWidth = rowRect.width - buttonWidth - spacing;

            ObserversListCalculateWidths(
                availableWidth,
                minTargetWidth,
                methodPreferredWidth,
                minMethodWidth,
                out float targetWidth,
                out float methodWidth
            );

            Rect contentRect = new Rect(rowRect.x, rowRect.y, availableWidth, rowHeight);

            Rect targetRect = new Rect(contentRect.x, contentRect.y, targetWidth, contentRect.height);
            Rect methodRect = new Rect(targetRect.xMax, contentRect.y, methodWidth, contentRect.height);
            Rect buttonRect = new Rect(contentRect.xMax + spacing, rowRect.y, buttonWidth, rowRect.height);

            DrawObserversListRowButton(contentRect, del);
            DrawObserversListRowLabels(targetRect, methodRect, targetName, methodName);
            DrawNotifyButton(buttonRect, del);
        }

        /// <summary>
        /// Calculates the target and method column widths based on available space and constraints.
        /// </summary>
        /// <param name="availableWidth">Total width available for both columns (excluding button).</param>
        /// <param name="minTargetWidth">Minimum width allowed for the target column.</param>
        /// <param name="methodPreferredWidth">Preferred width for the method column.</param>
        /// <param name="minMethodWidth">Minimum width allowed for the method column.</param>
        /// <param name="targetWidth">Calculated width for the target column (output).</param>
        /// <param name="methodWidth">Calculated width for the method column (output).</param>
        private void ObserversListCalculateWidths(
            float availableWidth,
            float minTargetWidth,
            float methodPreferredWidth,
            float minMethodWidth,
            out float targetWidth,
            out float methodWidth)
        {
            targetWidth = Mathf.Max(minTargetWidth, availableWidth - methodPreferredWidth);
            methodWidth = availableWidth - targetWidth;

            if (methodWidth < minMethodWidth)
            {
                methodWidth = minMethodWidth;
                targetWidth = availableWidth - methodWidth;
            }
        }

        /// <summary>
        /// Draws the selection button for a row, which highlights and pings the target object in the editor.
        /// </summary>
        /// <param name="rect">Rectangle where the button should be drawn.</param>
        /// <param name="del">The delegate whose target will be selected.</param>
        private void DrawObserversListRowButton(Rect rect, Delegate del)
        {
            GUIContent content = new GUIContent(string.Empty, EditorConsts.SelectToolTipMessage);

            if (GUI.Button(rect, content, EditorStyles.miniButton))
            {
                if (del.Target is UnityEngine.Object obj)
                {
                    Selection.activeObject = obj;
                    EditorGUIUtility.PingObject(obj);
                }
            }
        }

        /// <summary>
        /// Draws the target name and method name labels for an observer row.
        /// </summary>
        /// <param name="targetRect">Rectangle for the target name label.</param>
        /// <param name="methodRect">Rectangle for the method name label.</param>
        /// <param name="targetName">The name of the target object.</param>
        /// <param name="methodName">The name of the method.</param>
        private void DrawObserversListRowLabels(Rect targetRect, Rect methodRect, string targetName, string methodName)
        {
            EditorGUI.LabelField(targetRect, targetName);
            EditorGUI.LabelField(methodRect, methodName);
        }

        /// <summary>
        /// Draws a button that invokes the observer delegate with the current reactive value.
        /// </summary>
        /// <param name="rect">Rectangle where the button should be drawn.</param>
        /// <param name="del">The delegate to invoke when the button is pressed.</param>
        private void DrawNotifyButton(Rect rect, Delegate del)
        {
            if (GUI.Button(rect, EditorConsts.NotifyButtonLabel))
            {
                try
                {
                    del.DynamicInvoke(scriptable.GetEditorValue());
                }
                catch
                {
                    // Silently catch any exceptions during dynamic invocation
                }
            }
        }

        /// <summary>
        /// Draws runtime buttons (Print, Notify, Reset)
        /// </summary>
        protected virtual void DrawRuntimeButtons()
        {
            if (GUILayout.Button(EditorConsts.PrintRuntimeDataLabel))
            {
                PrintDataDebug();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button(EditorConsts.NotifyRuntimeDataLabel))
            {
                NotifyValue();
            }

            if (serialized && !NonSerializedTypes.Contains(target.GetType()))
            {
                EditorGUILayout.Space();

                if (GUILayout.Button(EditorConsts.ResetRuntimeDataLabel))
                {
                    ResetRuntimeData();
                }
            }
        }
    }
}
#endif