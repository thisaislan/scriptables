#if UNITY_EDITOR
using System.Collections.Generic;
using Thisaislan.Scriptables.Editor.Utilities.Styles;
using Thisaislan.Scriptables.Editor.Utilities.Widgets;
using Thisaislan.Scriptables.Statics;
using UnityEditor;
using UnityEngine;

namespace Thisaislan.Scriptables.Editor.Utilities.DrawHelpers
{
    /// <summary>
    /// Utility class for drawing editor property elements.
    /// </summary>
    internal static class DrawPropertyEditorHelper
    {
        private const float DescriptionButtonWidthSize = 38;
        private const float DescriptionButtonHeightSize = 38;
        private const float PinIconSize = 20;
        private const float SmallSpaceSize = 2;
        private const float BigSpaceSize = 10;
        private const float FieldDescriptionReadyOnlyHeightSize = 38;
        private const float FieldDescriptionEditableHeightSize = 30;
        private const string ScriptPropertyName = "m_Script";
        private const string ScriptPropertyDescription = "description";
        private const string ScriptPropertyTrackActivity = "trackActivity";
        private const string PropertiesLabel = "Properties";
        private const string PropertiesMessage = " - Editor only";
        private const string EmptyDescriptionLabel = "No description";
        private const string DescriptionSaveButtonTooltip = "Save";
        private const string DescriptionEditButtonTooltip = "Edit";
        private const string DetailsLabel = "Details";
        private const string AssemblyLabel = "Assembly";
        private const string NamespaceLabel = "Namespace";
        private const string AssemblyExtension = "asmdef";
        private const string LocationLabel = "Location";
        private const string ClassHierarchyLabel = " Class Hierarchy";
        private const string NoDebugClasses = " - No debug classes      ";
        private const string NoneLabel = "None";
        private const string StopTrackActivityLabel = "Stop Tracking";
        private const string StartTrackActivityLabel = "Start Tracking";
        private const string NamespaceKeyToFilter ="Thisaislan.Scriptables.Editor";
        private const string DescriptionReadyOnlyTooltip = "A brief description that helps distinguish this ScriptableObject from others.";
        private const string TrackActivityTooltip = " When enabled, logs major scriptable events such as data changes, emissions (for reactive types), and pinning operations.";

        private static string editDescriptionBuffer;

        /// <summary>
        /// Draws a styled, foldable "Details" card in the Unity Inspector.
        /// </summary>
        /// <param name="serializedObject">The <see cref="SerializedObject"/> containing the properties to be displayed.</param>
        /// <param name="isMultipleTargets">Whether multiple targets are selected (as opposed to a single target).</param>
        /// <param name="showDetails">A reference boolean controlling the foldout state of the details section.</param>
        internal static void DrawDetailsCard(SerializedObject serializedObject, bool isMultipleTargets, ref bool showDetails)
        {
            DrawEditorHelper.BeginVerticalCard();

            EditorGUI.indentLevel++;

            showDetails = EditorGUILayout.Foldout(
                showDetails,
                DetailsLabel,
                true,
                ScriptablesStyles.FoldoutStyle
            );

            if (showDetails)
            {
                DrawDetails(isMultipleTargets, serializedObject);
            }

            EditorGUI.indentLevel--;

            DrawEditorHelper.EndVerticalCard();
        }

        /// <summary>
        /// Renders an editable description field for the script property, including
        /// a toggleable edit mode with a text area and a read-only display state and
        /// log checkbox .
        /// </summary>
        /// <param name="serializedObject">The SerializedObject that contains the description property.</param>
        /// <param name="isMultipleTargets">Whether multiple targets are selected (as opposed to a single target).</param>
        /// <param name="isEditingDescription">Indicates whether the description is currently in edit mode.
        /// This value may be updated based on user interaction.</param>
        internal static void DrawScriptProperties(SerializedObject serializedObject, bool isMultipleTargets, ref bool isEditingDescription)
        {
            SerializedProperty serializedDescriptionProperty = serializedObject.FindProperty(ScriptPropertyDescription);

            SerializedProperty serializedTrackActivityProperty = serializedObject.FindProperty(ScriptPropertyTrackActivity);

            string currentDescription = serializedDescriptionProperty.stringValue;
            string displayText = GetDisplayText(currentDescription);

            DrawEditorHelper.BeginVerticalCard();

            DrawPropertyHeader(isMultipleTargets, serializedObject);

            EditorGUILayout.Space();

            if (!isMultipleTargets)
            {
                DrawDescriptionContent(serializedDescriptionProperty, isEditingDescription, displayText, ref isEditingDescription);

                EditorGUILayout.Space(SmallSpaceSize);
            }

            DrawTrackActivity(serializedTrackActivityProperty);

            DrawEditorHelper.EndVerticalCard();
        }

        private static void DrawPropertyHeader(bool isMultipleTargets, SerializedObject serializedObject)
        {
            EditorGUILayout.BeginHorizontal();

            DrawEditorHelper.DrawHeaderWithInfo(PropertiesLabel, PropertiesMessage);

            if (Application.isPlaying && !isMultipleTargets)
            {
                bool isKept = ReferenceKeeper.IsKept(serializedObject.targetObject as ScriptableObject);

                if (isKept)
                {
                    EditorGUILayout.LabelField(
                        EditorGUIUtility.IconContent(ScriptablesStylesIcons.GetIconName(ScriptablesStylesIcons.IconType.Pin)),
                        GUILayout.Width(PinIconSize), GUILayout.Height(PinIconSize));
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private static void DrawDetails(bool isMultipleTargets, SerializedObject serializedObject)
        {
            SerializedProperty scriptProperty = serializedObject.FindProperty(ScriptPropertyName);
            MonoScript monoScript = GetMonoScript(scriptProperty);

            string assemblyName = GetAssemblyName(monoScript);
            string targetObjectPath = GetObjectPath(serializedObject.targetObject);

            DrawDisabledObjectField(scriptProperty.displayName, scriptProperty.objectReferenceValue, typeof(MonoScript));

            DrawAssemblyField(assemblyName);

            if (!isMultipleTargets)
            {
                DrawFolderField(targetObjectPath);
            }

            DrawNamespace(monoScript);

            EditorGUILayout.Space(BigSpaceSize);

            DrawClassHierarchy(monoScript);
        }

        private static void DrawNamespace(MonoScript monoScript)
        {
            string namespaceName = monoScript.GetClass().Namespace;

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(NamespaceLabel, ScriptablesStyles.LabelFieldStyle);

            DrawEditorHelper.DisableGuiEnableState();
            EditorGUILayout.TextField(namespaceName ?? NoneLabel);
            DrawEditorHelper.RestoreGuiEnableState();

            EditorGUILayout.EndHorizontal();
        }

        private static void DrawClassHierarchy(MonoScript monoScript)
        {   
            EditorGUI.indentLevel--;

            EditorGUILayout.BeginVertical(ScriptablesStyles.DarkHelpBox);

            System.Type monoScriptType = monoScript.GetClass();

            List<System.Type> hierarchy = GetInheritanceChain(monoScriptType);
            
            EditorGUILayout.BeginVertical();

            DrawEditorHelper.DrawHeaderWithInfo(ClassHierarchyLabel, NoDebugClasses);
            ScriptablesStyles.DrawLine();
            
            EditorGUILayout.EndVertical();
            
            EditorGUI.indentLevel++;
            
            for (int i = 0; i < hierarchy.Count; i++)
            {
                System.Type type = hierarchy[i];

                EditorGUILayout.BeginHorizontal();

                GUILayout.Space(i > 0 ? i * 10 : 2);

                GUIStyle style = (i == hierarchy.Count - 1) ?
                    ScriptablesStyles.LabelHighlightedInfoFieldStyle :
                    ScriptablesStyles.LabelInfoFieldStyle;

                string typeNameSimplifiedName = TypeNameSimplifier.SimplifyTypeName(type.Name);

                string displayName = i > 0 ? "↳ " + typeNameSimplifiedName : typeNameSimplifiedName;

                EditorGUILayout.LabelField(displayName, style);

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }

        private static readonly List<System.Type> inheritanceChainCache = new List<System.Type>();

        private static List<System.Type> GetInheritanceChain(System.Type type)
        {
            inheritanceChainCache.Clear();

            while (type != null)
            {
                bool isFilteredNamespace = type.Namespace != null && type.Namespace.Contains(NamespaceKeyToFilter);

                if (!isFilteredNamespace)
                {
                    inheritanceChainCache.Insert(0, type);
                }

                type = type.BaseType;
            }

            return inheritanceChainCache;
        }

        private static void DrawFolderField(string scriptPath)
        {
            string folderPath = System.IO.Path.GetDirectoryName(scriptPath);

            Object folder = AssetDatabase.LoadAssetAtPath<Object>(folderPath);

            DrawDisabledObjectField(LocationLabel, folder, typeof(Object));
        }

        private static MonoScript GetMonoScript(SerializedProperty property)
        {
            return property.objectReferenceValue as MonoScript;
        }

        private static string GetAssemblyName(MonoScript monoScript)
        {
            return monoScript.GetClass().Assembly.GetName().Name;
        }

        private static string GetObjectPath(Object obj)
        {
            return AssetDatabase.GetAssetPath(obj);
        }

        private static void DrawAssemblyField(string assemblyName)
        {
            Object asmdef = FindAsmDef(assemblyName);

            if (asmdef != null)
            {
                DrawDisabledObjectField(AssemblyLabel, asmdef, typeof(Object));
            }
            else
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(AssemblyLabel, ScriptablesStyles.LabelFieldStyle);

                DrawEditorHelper.DisableGuiEnableState();
                EditorGUILayout.TextField(assemblyName);
                DrawEditorHelper.RestoreGuiEnableState();

                EditorGUILayout.EndHorizontal();
            }
        }

        private static Object FindAsmDef(string assemblyName)
        {
            string[] guids = AssetDatabase.FindAssets(assemblyName + " t:" + AssemblyExtension);

            if (guids.Length == 0)
            {
                return null;
            }

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);

            return AssetDatabase.LoadAssetAtPath<Object>(path);
        }

        private static void DrawDisabledObjectField(string label, Object value, System.Type type)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(label, ScriptablesStyles.LabelFieldStyle);

            DrawEditorHelper.DisableGuiEnableState();
            EditorGUILayout.ObjectField(value, type, false);
            DrawEditorHelper.RestoreGuiEnableState();

            EditorGUILayout.EndHorizontal();
        }

        private static string GetDisplayText(string currentDescription)
        {
            if (string.IsNullOrWhiteSpace(currentDescription))
            {
                return EmptyDescriptionLabel;
            }

            return currentDescription;
        }

        private static void DrawDescriptionContent(SerializedProperty property, bool isEditing, string displayText, ref bool isEditingDescription)
        {

            EditorGUILayout.BeginHorizontal();

            if (isEditing)
            {
                DrawEditableFieldDescription(property);
            }
            else
            {
                DrawReadOnlyDescription(displayText);
            }

            isEditingDescription = DrawDescriptionEditSaveButton(property, isEditingDescription);

            EditorGUILayout.EndHorizontal();
        }

        private static void DrawTrackActivity(SerializedProperty property)
        {
            bool propertyValue = property.boolValue;

            string label = propertyValue ? StopTrackActivityLabel : StartTrackActivityLabel;

            ScriptablesStylesColors.ButtonColorStyle style = propertyValue ? 
                ScriptablesStylesColors.ButtonColorStyle.Urgent :
                ScriptablesStylesColors.ButtonColorStyle.Neutral;

            ScriptablesStylesIcons.ButtonIcon buttonIcon = propertyValue ?
                ScriptablesStylesIcons.ButtonIcon.StopIcon :
                ScriptablesStylesIcons.ButtonIcon.PlayIcon;

            ButtonPalette.DrawButton(
                label: label,
                buttonIcon: buttonIcon,
                style: style,
                tooltip: TrackActivityTooltip,
                action: () =>
                {
                    property.boolValue = !propertyValue;
                }
            );
        }

        private static void DrawReadOnlyDescription(string displayText)
        {
            EditorGUILayout.BeginHorizontal(ScriptablesStyles.DarkHelpBox);

            Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(FieldDescriptionEditableHeightSize));
            EditorGUI.LabelField(rect, displayText, ScriptablesStyles.LabelInfoFieldStyle);
            TooltipHelper.DrawTooltip(rect, DescriptionReadyOnlyTooltip);

            EditorGUILayout.EndHorizontal();
        }

        private static void DrawEditableFieldDescription(SerializedProperty property)
        {
            editDescriptionBuffer = EditorGUILayout.TextArea(
                editDescriptionBuffer,
                ScriptablesStyles.WrappedTextAreaStyle,
                GUILayout.Height(FieldDescriptionReadyOnlyHeightSize)
            );
        }

        private static bool DrawDescriptionEditSaveButton(SerializedProperty property, bool isEditing)
        {
            string tooltip;
            ScriptablesStylesIcons.ButtonIcon buttonIcon;

            if (isEditing)
            {
                tooltip = DescriptionSaveButtonTooltip;
                buttonIcon = ScriptablesStylesIcons.ButtonIcon.SaveIcon;
            }
            else
            {
                tooltip = DescriptionEditButtonTooltip;
                buttonIcon = ScriptablesStylesIcons.ButtonIcon.EditIcon;
            }

            bool nextState = isEditing;

            ButtonPalette.DrawButton(
                label: string.Empty,
                buttonIcon: buttonIcon,
                fixedWidth: DescriptionButtonWidthSize,
                customHeight: DescriptionButtonHeightSize,
                style: ScriptablesStylesColors.ButtonColorStyle.Alert,
                tooltip: tooltip,
                action: () =>
                {
                    if (isEditing)
                    {
                        nextState = SaveDescription(property);
                    }
                    else
                    {
                        nextState = StartEditingDescription(property);
                    }
                }
            );

            return nextState;
        }

        private static bool StartEditingDescription(SerializedProperty property)
        {
            editDescriptionBuffer = property.stringValue;

            return true;
        }

        private static bool SaveDescription(SerializedProperty property)
        {
            string trimmed = editDescriptionBuffer;

            if (!string.IsNullOrEmpty(trimmed))
            {
                trimmed = trimmed.Trim();
            }

            property.stringValue = trimmed;

            GUI.FocusControl(null);

            return false;
        }
    }
}
#endif