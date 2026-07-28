#if UNITY_EDITOR
using System.Collections.Generic;
using Thisaislan.Scriptables.Editor.MonoBehaviours;
using Thisaislan.Scriptables.Editor.Utilities.DrawHelpers;
using Thisaislan.Scriptables.Editor.Utilities.Styles;
using UnityEditor;
using UnityEngine;

namespace Thisaislan.Scriptables.Editor.Inspectors
{
    /// <summary>
    /// Custom inspector for <see cref="ScriptableCache"/>. Displays an info card
    /// explaining the editor-only purpose and a read-only list of all pinned
    /// ScriptableObjects with the ability to select them via click.
    /// </summary>
    [CustomEditor(typeof(ScriptableCache))]
    internal class ScriptableCacheEditor : UnityEditor.Editor
    {
        private const string CardTitle = "Scriptable Cache";
        private const string InfoMessage = "      - Editor-only object that shows references to all pinned Scriptables.\n" +
            "      - Pinned Scriptables show a Pin icon in the Inspector property info area.";

        private const string PinnedListTitle = "Pinned Scriptables";
        private const string TotalLabel = "  Total: ";
        private const string NoPinnedMessage = "No pinned Scriptables.";

        private const float RowHeight = 26f;
        private const float MaxVisibleHeight = RowHeight * 16f;
        private const float IconSize = 16f;
        private const float IconLabelSpacing = 4f;
        private const float IconAreaWidth = 20f;
        private const float IconAreaHeight = 20f;
        private const float InfoCardHeaderSpacing = 4f;
        private const float InfoCardBottomrSpacing = 10f;
        private const float InfoCardTopSpacing = 4f;
        
        private const float ScrollAreaTopSpacing = 2f;
        private const float TotalLabelHeight = 20f;
        private const float RowLeftPadding = 4f;
        private const float RowRightPadding = 8f;

        private Vector2 scrollPos;

        /// <summary>
        /// Draws the custom inspector GUI for ScriptableCache.
        /// </summary>
        public override void OnInspectorGUI()
        {
            DrawInfoCard();

            DrawEditorHelper.DrawSpaceBetweenCards();

            DrawPinnedListCard();
        }

        private void DrawInfoCard()
        {

            EditorGUILayout.Space(InfoCardTopSpacing);

            DrawEditorHelper.BeginVerticalCard();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(
                EditorGUIUtility.IconContent(ScriptablesStylesIcons.GetIconName(ScriptablesStylesIcons.IconType.Info)),
                GUILayout.Width(IconAreaWidth), GUILayout.Height(IconAreaHeight));

            EditorGUILayout.LabelField(CardTitle, ScriptablesStyles.LabelTitleFieldStyle);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(InfoCardHeaderSpacing);

            EditorGUILayout.LabelField(InfoMessage, ScriptablesStyles.LabelInfoFieldStyle);

            EditorGUILayout.Space(InfoCardBottomrSpacing);

            DrawEditorHelper.EndVerticalCard();
        }

        private void DrawPinnedListCard()
        {
            DrawEditorHelper.BeginVerticalCard();

            EditorGUILayout.LabelField(PinnedListTitle, ScriptablesStyles.LabelTitleFieldStyle);

            EditorGUILayout.Space(ScrollAreaTopSpacing);

            ScriptableCache cache = target as ScriptableCache;
            List<ScriptableObject> list = cache != null
                ? cache.GetScriptableObjects()
                : new List<ScriptableObject>();

            int count = list.Count;

            if (count == 0)
            {
                DrawEditorHelper.DrawMessage(NoPinnedMessage);
            }
            else
            {
                EditorGUILayout.BeginVertical(ScriptablesStyles.DarkHelpBox);

                float contentHeight = RowHeight * count;
                bool needsScroll = contentHeight > MaxVisibleHeight;

                if (needsScroll)
                {
                    scrollPos = EditorGUILayout.BeginScrollView(
                        scrollPos, GUILayout.Height(MaxVisibleHeight));
                }
                else
                {
                    EditorGUILayout.BeginVertical(GUILayout.Height(contentHeight));
                }

                EditorGUILayout.Space(ScrollAreaTopSpacing);

                for (int i = 0; i < count; i++)
                {
                    ScriptableObject obj = list[i];

                    if (obj != null)
                    {
                        DrawPinnedRow(obj);
                    }
                }

                if (needsScroll)
                {
                    EditorGUILayout.EndScrollView();
                }
                else
                {
                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.Space(ScrollAreaTopSpacing);

                EditorGUILayout.LabelField(
                    $"{TotalLabel}{count}",
                    ScriptablesStyles.LabelInfoFieldStyle,
                    GUILayout.Height(TotalLabelHeight));

                EditorGUILayout.EndVertical();
            }

            DrawEditorHelper.EndVerticalCard();
        }

        private static void DrawPinnedRow(ScriptableObject obj)
        {
            Rect rowRect = EditorGUILayout.GetControlRect(false, RowHeight);

            DrawEditorHelper.SetGuiButtonColor(ScriptablesStylesColors.ButtonColorStyle.Neutral);

            if (GUI.Button(rowRect, GUIContent.none, ScriptablesStyles.ButtonStyle))
            {
                Selection.activeObject = obj;
                EditorGUIUtility.PingObject(obj);
            }

            DrawEditorHelper.RestoreGuiButtonColor();

            Rect iconRect = new Rect(
                rowRect.x + RowLeftPadding,
                rowRect.y + (rowRect.height - IconSize) / 2f,
                IconSize,
                IconSize);

            Texture icon = EditorGUIUtility.ObjectContent(obj, obj.GetType()).image;

            if (icon != null)
            {
                GUI.DrawTexture(iconRect, icon);
            }

            Rect nameRect = new Rect(
                iconRect.xMax + IconLabelSpacing,
                rowRect.y,
                rowRect.width - iconRect.width - IconLabelSpacing - RowRightPadding,
                rowRect.height);

            EditorGUI.LabelField(nameRect, obj.name, ScriptablesStyles.LabelFieldStyle);
        }
    }
}
#endif
