#if UNITY_EDITOR
using System.Collections.Generic;
using Thisaislan.Scriptables.Editor.Utilities.Styles;
using UnityEditor;
using UnityEngine;

namespace Thisaislan.Scriptables.Editor.Utilities.Widgets
{
    /// <summary>
    /// Generic pagination widget for Editor UI that manages page navigation and item display.
    /// </summary>
    internal class Pagination<T>
    {
        private const int DefaultItemsPerPage = 50;
        private const int ButtonWidth = 60;
        private const int ButtonHeight = 21;
        private const int MinPagesNumberIndicatorWidth = 60;

        private const string LeftArrow = "◀";
        private const string RightArrow = "▶";
        private const string DoubleLeftArrow = "◀◀";
        private const string DoubleRightArrow = "▶▶";
        private const string LeftArrowTooltip = "Previous page";
        private const string RightArrowTooltip = "Next page";
        private const string DoubleLeftArrowTooltip = "First page";
        private const string DoubleRightArrowTooltip = "Last page";

        private readonly int itemsPerPage;
        
        private int currentPage = 0;
        private List<T> allItems = new List<T>();
        private List<T> currentPageItems = new List<T>();

        /// <summary>
        /// Gets the items for the current page.
        /// </summary>
        internal List<T> CurrentPageItems
        {
            get
            {
                return currentPageItems;
            }
        }

        /// <summary>
        /// Gets the current page index (zero-based).
        /// </summary>
        internal int CurrentPage
        {
            get
            {
                return currentPage;
            }
        }

        /// <summary>
        /// Gets the total number of pages.
        /// </summary>
        internal int TotalPages
        {
            get
            {
                return Mathf.Max(1, Mathf.CeilToInt((float)allItems.Count / itemsPerPage));
            }
        }
        
        /// <summary>
        /// Gets the total number of items.
        /// </summary>
        internal int TotalItems
        {
            get
            {
                return allItems.Count;
            }
        }

        /// <summary>
        /// Initializes pagination with optional items per page.
        /// </summary>
        internal Pagination(int itemsPerPage = DefaultItemsPerPage)
        {
            this.itemsPerPage = Mathf.Max(1, itemsPerPage);
        }

        /// <summary>
        /// Sets the item list and optionally resets to first page.
        /// </summary>
        internal void SetItems(List<T> items, bool resetToFirstPage = false)
        {
            allItems = items ?? new List<T>();

            if (resetToFirstPage)
            {
                currentPage = 0;
            }

            ApplyPagination();
        }

        /// <summary>
        /// Resets pagination to the first page.
        /// </summary>
        internal void Reset()
        {
            currentPage = 0;
            ApplyPagination();
        }

        /// <summary>
        /// Draws pagination controls. Returns true if page changed.
        /// </summary>
        internal bool DrawControls()
        {
            if (allItems.Count <= itemsPerPage)
            {
                return false;
            }

            bool pageChanged = false;

            EditorGUILayout.BeginHorizontal(ScriptablesStyles.WindowHeaderStyle);
            GUILayout.FlexibleSpace();

            DrawPaginationControllers(ref pageChanged);

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            if (pageChanged)
            {
                ApplyPagination();
            }

            return pageChanged;
        }

        private void DrawPaginationControllers(ref bool pageChanged)
        {
            pageChanged = DrawLeftArrows(pageChanged);

            DrawPagesNumberIndicator();

            pageChanged = DrawRightArrows(pageChanged);
        }

        private bool DrawRightArrows(bool pageChanged)
        {
            ButtonPalette.DrawButton(
                label: RightArrow,
                style: ScriptablesStylesColors.ButtonColorStyle.Neutral,
                tooltip: RightArrowTooltip,
                fixedWidth: ButtonWidth,
                customHeight: ButtonHeight,
                action: () =>
                {
                    currentPage = Mathf.Min(TotalPages - 1, currentPage + 1);
                    pageChanged = true;
                }
            );

            ButtonPalette.DrawButton(
                label: DoubleRightArrow,
                style: ScriptablesStylesColors.ButtonColorStyle.Neutral,
                tooltip: DoubleRightArrowTooltip,
                fixedWidth: ButtonWidth,
                customHeight: ButtonHeight,
                action: () =>
                {
                    currentPage = TotalPages - 1;
                    pageChanged = true;
                }
            );
            return pageChanged;
        }

        private bool DrawLeftArrows(bool pageChanged)
        {
            ButtonPalette.DrawButton(
                label: DoubleLeftArrow,
                style: ScriptablesStylesColors.ButtonColorStyle.Neutral,
                tooltip: DoubleLeftArrowTooltip,
                fixedWidth: ButtonWidth,
                customHeight: ButtonHeight,
                action: () =>
                {
                    currentPage = 0;
                    pageChanged = true;
                }
            );

            ButtonPalette.DrawButton(
                label: LeftArrow,
                style: ScriptablesStylesColors.ButtonColorStyle.Neutral,
                tooltip: LeftArrowTooltip,
                fixedWidth: ButtonWidth,
                customHeight: ButtonHeight,
                action: () =>
                {
                    currentPage = Mathf.Max(0, currentPage - 1);
                    pageChanged = true;
                }
            );
            return pageChanged;
        }

        private void DrawPagesNumberIndicator()
        {
            EditorGUILayout.BeginVertical(ScriptablesStyles.DarkHelpBox, GUILayout.MinWidth(MinPagesNumberIndicatorWidth));

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.Label(
                $"{(currentPage + 1)}/{TotalPages}",
                ScriptablesStyles.LabelHighlightedInfoFieldStyle
            );

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        private void ApplyPagination()
        {
            currentPage = Mathf.Clamp(currentPage, 0, TotalPages - 1);

            int startIndex = currentPage * itemsPerPage;
            int count = Mathf.Min(itemsPerPage, allItems.Count - startIndex);

            if (count <= 0)
            {
                currentPageItems = new List<T>();
            }
            else
            {
                currentPageItems = allItems.GetRange(startIndex, count);
            }
        }
    }
}
#endif