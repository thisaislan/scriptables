#if UNITY_EDITOR
using System;
using Thisaislan.Scriptables.Editor.Utilities.Styles;
using Thisaislan.Scriptables.Editor.Utilities.Widgets;
using UnityEditor;
using UnityEngine;

namespace Thisaislan.Scriptables.Editor.Utilities.DrawHelpers
{
    /// <summary>
    /// Helper class for drawing observable/delegate observer lists in the Unity Editor.
    /// Provides a table-style UI for displaying registered subscribers with target and method information.
    /// </summary>
    internal static class DrawSubscribersListEditorHelper
    {
        private const float RowHeight = 26f;
        private const float HeaderHeight = 25f;

        private static readonly GUIContent RowButtonContent = new GUIContent(string.Empty, SelectToolTipMessage);

        //  Make scroll height adapt to row size
        private const float MaxHeight = RowHeight * 8f + Spacing * 11f;
        private const float MinTargetWidth = 80;
        private const float MethodPreferredWidth = 160;
        private const float MinMethodWidth = 80f;
        private const float ButtonWidth = 80f;
        private const float Spacing = 4f;

        //  Strings
        private const string ObjectLabel = "Object";
        private const string MethodLabel = "Method";
        private const string SelectToolTipMessage = "Select";
        private const string EmitButtonLabel = "Emit";
        private const string DefaultReactiveMethodName = "Static";
        private const string TotalLabel =  "Total: ";
        private const string ApplicationIsPlayingObserversRegisteredMessage = "Active During Play Mode";
        private const string NoSubscribersRegisteredMessage = "No subscribers registered";
        private const string RuntimeSubscribersSectionTitle = "Subscribers";
        private const string EmitAllLabel = "Emit to All";

        /// <summary>
        /// Draws a card with a single emit all button.
        /// </summary>
        /// <param name="actionOnEmitAll">Optional function to get data for DynamicInvoke</param>
        /// <param name="enabled">set the enable state of the card</param>
        internal static void DrawRuntimeObserversCard(
            Action actionOnEmitAll,
            bool enabled)
        {
            DrawEditorHelper.SetGuiEnableState(enabled);
            
            DrawEditorHelper.BeginVerticalCard();

            EditorGUILayout.LabelField(RuntimeSubscribersSectionTitle, ScriptablesStyles.LabelTitleFieldStyle);

            EditorGUILayout.Space();

            ButtonPalette.DrawEmitButton(EmitAllLabel, actionOnEmitAll);

            DrawEditorHelper.EndVerticalCard();

            DrawEditorHelper.RestoreGuiEnableState();
        }

        /// <summary>
        /// Draws a complete subscribers list inside a styled card container.
        /// </summary>
        /// <param name="actionDelegate">Delegate containing the registered subscribers</param>
        /// <param name="scrollPos">Reference to the scroll position (pass by reference)</param>
        /// <param name="actionOnEmitAll">Action to execute when the emit All button is clicked</param>
        /// <param name="messageOnEmitError">message to show in case of emit error</param>
        /// <param name="enabled">set the enable state of the card</param>
        /// <param name="getEmitData">Optional function to get data for DynamicInvoke</param>
        internal static void DrawRuntimeObserversListCard(
            Delegate actionDelegate,
            ref Vector2 scrollPos,
            Action actionOnEmitAll,
            string messageOnEmitError,
            bool enabled,
            Func<object> getEmitData = null)
        {
            DrawEditorHelper.SetGuiEnableState(enabled);
            
            DrawEditorHelper.BeginVerticalCard();

            EditorGUILayout.LabelField(RuntimeSubscribersSectionTitle, ScriptablesStyles.LabelTitleFieldStyle);
            EditorGUILayout.Space(2);

            string message = GetObserverValidationError(actionDelegate);

            if (message != null)
            {
                DrawEditorHelper.DrawMessage(message);
            }
            else
            {
                scrollPos = DrawObserversTable(actionDelegate, scrollPos, getEmitData, messageOnEmitError);
            }

            EditorGUILayout.Space();

            ButtonPalette.DrawEmitButton(EmitAllLabel, actionOnEmitAll);

            DrawEditorHelper.EndVerticalCard();

            DrawEditorHelper.RestoreGuiEnableState();
        }

        /// <summary>
        /// Draws the subscribers table with headers and scrollable rows.
        /// </summary>
        /// <param name="actionDelegate">Delegate containing the registered subscribers</param>
        /// <param name="scrollPos">Current scroll position</param>
        /// <param name="getEmitData">Optional function to get data for DynamicInvoke</param>
        /// <param name="messageOnEmitError">message to show in case of emit error</param>
        /// <returns>Updated scroll position</returns>
        private static Vector2 DrawObserversTable(Delegate actionDelegate, Vector2 scrollPos, Func<object> getEmitData, string messageOnEmitError)
        {
            Delegate[] invocationList = actionDelegate.GetInvocationList();

            int numberOfObservers = invocationList.Length;
            float contentHeight = RowHeight * invocationList.Length;
            bool needsScroll = contentHeight > MaxHeight;

            EditorGUILayout.BeginVertical(ScriptablesStyles.DarkHelpBox);

            DrawObserversListHeader();

            BeginObserversListBody(needsScroll, MaxHeight, contentHeight, ref scrollPos);

            DrawObserversListRows(invocationList, getEmitData, messageOnEmitError);

            EndObserversListBody(needsScroll);

            EditorGUILayout.LabelField($"{TotalLabel}{numberOfObservers}", ScriptablesStyles.LabelInfoFieldStyle, GUILayout.Height(20));

            EditorGUILayout.EndVertical();
            
            return scrollPos;
        }

        /// <summary>
        /// Draws the header row of the subscribers table.
        /// </summary>
        internal static void DrawObserversListHeader()
        {
            EditorGUILayout.BeginHorizontal();

            Rect headerRect = EditorGUILayout.GetControlRect(false, HeaderHeight);
            float availableWidth = headerRect.width - ButtonWidth - Spacing;

            CalculateWidths(availableWidth, out float targetWidth, out float methodWidth);

            Rect targetRect = new Rect(headerRect.x, headerRect.y, targetWidth, headerRect.height);
            Rect methodRect = new Rect(targetRect.xMax, headerRect.y, methodWidth, headerRect.height);

            EditorGUI.LabelField(targetRect, ObjectLabel, ScriptablesStyles.LabelTitleFieldStyle);
            EditorGUI.LabelField(methodRect, $"{MethodLabel}", ScriptablesStyles.LabelTitleFieldStyle);
            EditorGUILayout.EndHorizontal();

            ScriptablesStyles.DrawLine();
        }

        /// <summary>
        /// Begins the scrollable or fixed-height container for the observer rows.
        /// </summary>
        /// <param name="needsScroll">Whether scrolling is required</param>
        /// <param name="maxHeight">Maximum height when scrolling is enabled</param>
        /// <param name="contentHeight">Total content height when scrolling is disabled</param>
        /// <param name="scrollPos">Reference to the scroll position</param>
        internal static void BeginObserversListBody(bool needsScroll, float maxHeight, float contentHeight, ref Vector2 scrollPos)
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
        /// <param name="needsScroll">Whether scrolling was enabled</param>
        internal static void EndObserversListBody(bool needsScroll)
        {
            if (needsScroll)
            {
                EditorGUILayout.EndScrollView();
            }
            else
            {
                EditorGUILayout.EndVertical();
            }
        }

        /// <summary>
        /// Draws all observer rows from the provided delegate array.
        /// </summary>
        /// <param name="invocationList">Array of delegates representing each observer</param>
        /// <param name="getEmitData">Optional function to get data for DynamicInvoke</param>
        /// <param name="messageOnEmitError">message to show in case of emit error</param>
        internal static void DrawObserversListRows(Delegate[] invocationList, Func<object> getEmitData, string messageOnEmitError)
        {
            foreach (Delegate del in invocationList)
            {
                if (del == null) continue;

                DrawObserverRow(del, getEmitData, messageOnEmitError);
            }
        }

        /// <summary>
        /// Draws a single observer row with target name, method name, and emit button.
        /// </summary>
        /// <param name="del">The delegate representing this observer</param>
        /// <param name="getEmitData">Optional function to get data for DynamicInvoke</param>
        /// <param name="messageOnEmitError">message to show in case of emit error</param>
        internal static void DrawObserverRow(Delegate del, Func<object> getEmitData, string messageOnEmitError)
        {
            string targetName = del.Target != null ? del.Target.ToString() : DefaultReactiveMethodName;
            string methodName = del.Method.Name;

            Rect rowRect = EditorGUILayout.GetControlRect(false, RowHeight);

            float availableWidth = rowRect.width - ButtonWidth - Spacing;
            CalculateWidths(availableWidth, out float targetWidth, out float methodWidth);

            Rect contentRect = new Rect(rowRect.x, rowRect.y, availableWidth, rowRect.height);

            Rect targetRect = new Rect(contentRect.x, contentRect.y, targetWidth, contentRect.height);

            // First, compute the button rect at the end
            Rect buttonRect = new Rect(rowRect.xMax - ButtonWidth, rowRect.y, ButtonWidth, rowRect.height);

            // Method width should fill available space but not overlap button
            float methodMaxWidth = buttonRect.x - Spacing - targetRect.xMax - Spacing;
            float methodActualWidth = Mathf.Min(methodWidth, methodMaxWidth);

            // Now compute method rect using the actual width
            Rect methodRect = new Rect(targetRect.xMax, contentRect.y, methodActualWidth, contentRect.height);

            DrawObserversListRowButton(contentRect, del);
            DrawObserversListRowLabels(targetRect, methodRect, targetName, methodName);
            DrawEmitButton(buttonRect, del, getEmitData, messageOnEmitError);
        }

        /// <summary>
        /// Draws the selection button that selects and pings the target object.
        /// </summary>
        /// <param name="rect">Rectangle to draw the button in</param>
        /// <param name="del">The delegate whose target will be selected</param>
        internal static void DrawObserversListRowButton(Rect rect, Delegate del)
        {
            DrawEditorHelper.SetGuiButtonColor(ScriptablesStylesColors.ButtonColorStyle.Neutral);

            //  Better style for taller rows
            if (GUI.Button(rect, RowButtonContent, ScriptablesStyles.ButtonStyle))
            {
                if (del.Target is UnityEngine.Object obj)
                {
                    Selection.activeObject = obj;
                    EditorGUIUtility.PingObject(obj);
                }
            }

            DrawEditorHelper.RestoreGuiButtonColor();
        }

        /// <summary>
        /// Draws the target name and method name labels for an observer row.
        /// </summary>
        /// <param name="targetRect">Rectangle for the target name label</param>
        /// <param name="methodRect">Rectangle for the method name label</param>
        /// <param name="targetName">The name of the target object</param>
        /// <param name="methodName">The name of the method</param>
        internal static void DrawObserversListRowLabels(
            Rect targetRect,
            Rect methodRect,
            string targetName,
            string methodName)
        {
            EditorGUI.LabelField(targetRect, $" {targetName}", ScriptablesStyles.LabelFieldStyle);
            EditorGUI.LabelField(methodRect, $"  {methodName}", ScriptablesStyles.LabelFieldStyle);
        }

        /// <summary>
        /// Draws the emit button that invokes the observer delegate.
        /// </summary>
        /// <param name="rect">Rectangle to draw the button in</param>
        /// <param name="del">The delegate to invoke</param>
        /// <param name="getEmitData">Optional function to get data for DynamicInvoke</param>
        /// <param name="messageOnEmitError">message to show in case of emit error</param>
        internal static void DrawEmitButton(Rect rect, Delegate del, Func<object> getEmitData, string messageOnEmitError)
        {
           ButtonPalette.DrawEmitButton(rect, EmitButtonLabel, () => {
                try
                {
                    if (getEmitData != null)
                    {
                        del.DynamicInvoke(getEmitData());
                    }
                    else
                    {
                        del.DynamicInvoke();
                    }
                }
                catch
                {
                    Printer.PrintError(messageOnEmitError);
                }
            });
        }

        /// <summary>
        /// Calculates the target and method column widths based on available space.
        /// </summary>
        /// <param name="availableWidth">Total width available for both columns</param>
        /// <param name="targetWidth">Calculated width for the target column (output)</param>
        /// <param name="methodWidth">Calculated width for the method column (output)</param>
        internal static void CalculateWidths(float availableWidth, out float targetWidth, out float methodWidth)
        {
            if (availableWidth <= MinTargetWidth + MinMethodWidth)
            {
                targetWidth = MinTargetWidth;
                methodWidth = MinMethodWidth;
                return;
            }

            // Give method its preferred size first
            methodWidth = MethodPreferredWidth;

            // Remaining goes to target
            targetWidth = availableWidth - methodWidth;

            // If there's extra space, let method grow too
            float extraSpace = availableWidth - (MinTargetWidth + MethodPreferredWidth);

            if (extraSpace > 0)
            {
                // Split extra space (you can tweak this ratio)
                float methodExtra = extraSpace * 0.4f; // method grows a bit
                float targetExtra = extraSpace * 0.6f;

                methodWidth += methodExtra;
                targetWidth = MinTargetWidth + targetExtra;
            }

            // Final safety clamp
            methodWidth = Mathf.Max(MinMethodWidth, methodWidth);
            targetWidth = Mathf.Max(MinTargetWidth, targetWidth);
        }

        /// <summary>
        /// Validates the observer delegate and returns an error message if invalid.
        /// </summary>
        /// <param name="actionDelegate">The delegate to validate</param>
        /// <returns>Error message string, or null if the delegate is valid</returns>
        private static string GetObserverValidationError(Delegate actionDelegate)
        {
             if (!Application.isPlaying)
            {
                return ApplicationIsPlayingObserversRegisteredMessage;
            }

            if (actionDelegate == null || actionDelegate.GetInvocationList().Length == 0)
            {
                return NoSubscribersRegisteredMessage;
            }

            Delegate[] invocationList = actionDelegate.GetInvocationList();

            if (invocationList == null || invocationList.Length == 0)
            {
                return NoSubscribersRegisteredMessage;
            }

            return null;
        }
    }
}
#endif