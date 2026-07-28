#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Thisaislan.Scriptables.Editor.Utilities.Styles;
using UnityEditor;
using UnityEngine;

namespace Thisaislan.Scriptables.Editor.Utilities.Widgets
{
    /// <summary>
    /// Reusable search bar for Editor windows. Handles rendering, filtering, and focus state.
    /// </summary>
    internal class SearchBar
    {
        private const string SearchFieldName = "SearchField";
        private const string SearchLabel = "Search...";

        private string searchFilter = "";

        /// <summary>
        /// Current search filter text.
        /// </summary>
        internal string CurrentFilter
        {
            get
            {
                return searchFilter;
            }
        }

        /// <summary>
        /// True if search has active filter text.
        /// </summary>
        internal bool IsActive
        {
            get
            {
                return  !string.IsNullOrWhiteSpace(searchFilter);
            }
        }

        /// <summary>
        /// Draws the search bar in the Editor. Returns true if filter changed.
        /// </summary>
        internal bool Draw()
        {
            bool result = false;

            EditorGUILayout.BeginHorizontal();

            GUI.SetNextControlName(SearchFieldName);

            Rect rect = EditorGUILayout.GetControlRect(false, ScriptablesStyles.ToolbarSearchFieldStyle.fixedHeight, GUILayout.ExpandWidth(true));

            string newSearch = EditorGUI.TextField(rect, searchFilter, ScriptablesStyles.ToolbarSearchFieldStyle);

            if (string.IsNullOrEmpty(searchFilter) && GUI.GetNameOfFocusedControl() != SearchFieldName)
            {
                EditorGUI.LabelField(rect, SearchLabel, ScriptablesStyles.SearchPlaceholderStyle);
            }

            if (newSearch != searchFilter)
            {
                searchFilter = newSearch;

                result = true;
            }

            EditorGUILayout.EndHorizontal();

            return result;
        }

        /// <summary>
        /// Removes focus from the search field.
        /// </summary>
        internal void Unfocus()
        {
            GUI.FocusControl(null);
        }

        /// <summary>
        /// Filters a list by case-insensitive substring match on item names.
        /// </summary>
        internal List<T> Apply<T>(List<T> source, Func<T, string> getName)
        {
            if (!IsActive)
            {
                return new List<T>(source);
            }

            return source.Where(item => getName(item).IndexOf(searchFilter, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
        }

        /// <summary>
        /// Clears the current search filter.
        /// </summary>
        internal void Clear()
        {
            searchFilter = string.Empty;
        }
    }
}
#endif
