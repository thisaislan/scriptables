#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Thisaislan.Scriptables.Editor.Utilities.Styles;
using UnityEditor;
using UnityEngine;

namespace Thisaislan.Scriptables.Editor.Utilities.Widgets
{
    /// <summary>
    /// Manages the display and state of a loading indicator for asset scanning operations.
    /// Provides progress tracking, status updates, and a visual progress bar with optional cancel functionality.
    /// </summary>
    internal class LoadingIndicator
    {
        private const float ButtonSpacing = 10f;
        private const float EndingSpacing = 5f;
        private const float EmptySpacing = 20f;
        private const float ProgressBarHeight = 20f;
        private const float CancelButtonWidth = 100f;
        private const float ProgressBarMinWidth = 100f;

        private const string ScanningAssetsTitle = "Scanning Assets...";
        private const string ButtonCancelLabel = "Cancel";

        private AssetScanner scanner;
        private Action<List<UnityEngine.Object>> onCompleteCallback;

        /// <summary>
        /// Gets a value indicating whether an asset scan is currently in progress.
        /// </summary>
        /// <value>True if a scanner exists and is actively scanning; otherwise, false.</value>
        internal bool IsLoading
        {
            get
            {
                return scanner != null && scanner.IsScanning;
            }
        }

        /// <summary>
        /// Gets the current progress of the asset scan as a value between 0 and 1.
        /// </summary>
        /// <value>The scan progress percentage (0-1), or 0 if no scan is active.</value>
        internal float Progress
        {
            get
            {
                return scanner?.Progress ?? 0f;
            }
        }

        /// <summary>
        /// Gets the current status message describing the scan's ongoing operation.
        /// </summary>
        /// <value>A string containing the current scan status, or an empty string if no scan is active.</value>
        internal string Status
        {
            get
            {
                return scanner?.Status ?? string.Empty;
            }
        }

        /// <summary>
        /// Initiates an asynchronous asset scanning operation using the specified filters.
        /// </summary>
        /// <param name="filters">Array of filter strings to apply to the asset scan (e.g., "t:Prefab", "l:MyLabel")</param>
        /// <param name="onComplete">Optional callback invoked when the scan completes successfully, providing the list of found assets</param>
        internal void StartScan(string[] filters, Action<List<UnityEngine.Object>> onComplete = null)
        {
            StartScan(filters, null, onComplete);
        }

        /// <summary>
        /// Initiates an asynchronous asset scanning operation with include and exclude filters.
        /// </summary>
        /// <param name="filters">Array of filter strings for assets to include</param>
        /// <param name="excludeFilters">Optional array of filter strings for assets to exclude</param>
        /// <param name="onComplete">Optional callback invoked when the scan completes successfully, providing the list of found assets</param>
        internal void StartScan(string[] filters, string[] excludeFilters, Action<List<UnityEngine.Object>> onComplete = null)
        {
            if (IsLoading)
            {
                return;
            }

            onCompleteCallback = onComplete;
            scanner = new AssetScanner();
            scanner.Start(filters, excludeFilters, null, OnScanCompleted);
        }

        /// <summary>
        /// Cancels the ongoing scan, if any. The completion callback is invoked with null.
        /// </summary>
        internal void Cancel()
        {
            if (scanner != null && scanner.IsScanning)
            {
                scanner.Cancel();
                scanner = null;
                onCompleteCallback?.Invoke(null);
                onCompleteCallback = null;
            }
            else
            {
                onCompleteCallback = null;
            }
        }

        /// <summary>
        /// Draws the loading progress bar and status message.
        /// Returns true if the scan is still in progress (i.e., you should keep calling this each frame).
        /// </summary>
        /// <param name="showCancelButton">If true, draws a Cancel button next to the progress bar.</param>
        internal bool DrawProgress(bool showCancelButton = false)
        {
            if (!IsLoading)
            {
                return false;
            }

            bool result = true;

            DrawProgressCard(ScanningAssetsTitle, Status, Progress);

            if (showCancelButton)
            {
                result = DrawButtonSection(result);
            }

            return result;
        }

        /// <summary>
        /// Draws a progress card with title, status text, and a progress bar.
        /// </summary>
        /// <param name="title">The title displayed at the top of the card</param>
        /// <param name="status">The current status message shown below the title</param>
        /// <param name="progress">Progress value between 0 and 1</param>
        internal void DrawProgressCard(string title, string status, float progress)
        {
            GUILayout.Space(EmptySpacing);

            EditorGUILayout.BeginHorizontal();

            GUILayout.Space(EmptySpacing);

            EditorGUILayout.BeginVertical(ScriptablesStyles.CardStyle);

            EditorGUILayout.LabelField(title, ScriptablesStyles.LabelTitleFieldStyle);

            EditorGUILayout.LabelField(status, ScriptablesStyles.LabelInfoFieldStyle);

            Rect rect = GUILayoutUtility.GetRect(ProgressBarMinWidth, ProgressBarHeight);

            EditorGUI.ProgressBar(rect, progress, $"{progress * ProgressBarMinWidth:F0}%");

            GUILayout.Space(EndingSpacing);

            EditorGUILayout.EndVertical();

            GUILayout.Space(EmptySpacing);

            EditorGUILayout.EndHorizontal();
        }

        private bool DrawButtonSection(bool result)
        {
            GUILayout.Space(ButtonSpacing);

            EditorGUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            ButtonPalette.DrawButton(
                label: ButtonCancelLabel,
                buttonIcon: ScriptablesStylesIcons.ButtonIcon.ClearIcon,
                fixedWidth: CancelButtonWidth,
                style: ScriptablesStylesColors.ButtonColorStyle.Urgent,
                tooltip: ButtonCancelLabel,
                action: () =>
                {
                    Cancel();
                    result = false;
                }
            );

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();
            return result;
        }

        private void OnScanCompleted(List<UnityEngine.Object> results)
        {
            scanner = null;
            onCompleteCallback?.Invoke(results);
            onCompleteCallback = null;
        }
    }
}
#endif
