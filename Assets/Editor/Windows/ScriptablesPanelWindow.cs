#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Search;
using System;
using Thisaislan.Scriptables.Abstracts;
using Thisaislan.Scriptables.Editor.Abstracts;
using Thisaislan.Scriptables.Editor.Abstracts.Bases;
using Thisaislan.Scriptables.Editor.Utilities;
using Thisaislan.Scriptables.Editor.Utilities.Styles;
using System.Reflection;
using Thisaislan.Scriptables.Editor.Utilities.Widgets;
using Object = UnityEngine.Object;

namespace Thisaislan.Scriptables.Editor.Windows
{
    /// <summary>
    /// Main editor window for managing and viewing ScriptableObjects with search, filtering, and asset actions.
    /// </summary>
    internal class ScriptablesPanelWindow : EditorWindow
    {
        private enum Tab 
        {
            Scriptables,
            Settings,
            Runtime,
            Reactive,
            ScriptableObjects
        }

        private const float MinDescriptionWidth = 150f;
        private const float MinWindowWidth = 900f;
        private const float MinWindowHeight = 500f;
        private const float AssetColumnWidth = 250f;
        private const float ActionsColumnWidth = 195f;
        private const float ButtonWidth = 60f;
        private const float HeaderHeight = 20f;
        private const float IconSize = 20f;
        private const float MinPathWidth = 160f;
        private const float RowHeight = 20f;
        private const float PathWidthMultiplier = 0.4f;
        private const float DescWidthMultiplier = 0.6f;
        private const float RefreshButtonWidth = 90f;
        private const float SpaceBeforeIcon = 10f;
        private const float SpaceBeforeDescription = 5f;
        private const float SpaceAfterDescription = 5f;
        private const float MaxWidthMultiplier = 0.5f;
        private const float ScrollViewOffset = 150f;
        private const int IndexStart = 0;
        private const int IndexIncrement = 1;
        private const float TabHeight = 30f;
        private const float HeaderPadding = 30f;
        private const float DescriptionPadding = 10f;
        private const float EmptyStateSpacing = 20f;
        private const float ButtonHeight = 20f;
        private const float RowHorizontalPadding = 30f;
        private const int PaginationItemsPerPage = 50;
        private const float NameLabelRightPadding = 20f;

        // Labels
        private const string RefQuerySuffix = "ref:";
        private const string WindowTitle = "Scriptables Panel";
        private const string NoItemsLabel = "No items found.";
        private const string ConfirmDeleteTitle = "Confirm Delete";
        private const string ConfirmDeleteMessage = "Delete '{0}'?";
        private const string RenameFieldName = "RenameField";
        private const string TabScriptablesLabel = " Scriptables";
        private const string TabSettingsLabel = " Settings";
        private const string TabRuntimeLabel = " Runtime";
        private const string TabReactiveLabel = " Reactive";
        private const string TabScriptableObjectLabel = " Scriptable Object";
        private const string AssetColumnHeader = "  Asset";
        private const string PathColumnHeader = "Path";
        private const string DescriptionColumnHeader = "       Description";
        private const string ActionsColumnHeader = "     Actions";
        private const string RenameButtonLabel = "Rename ";
        private const string SearchButtonLabel = "Search ";
        private const string DeleteButtonLabel = "Delete ";
        private const string CancelButtonLabel = "Cancel";
        private const string RefreshButtonLabel = "Refresh";
        private const string AssetListScrollViewName = "AssetListScrollView";
        private const string NotScriptableTooltip = "Not a Scriptable";
        private const string NoDescriptionLabel = "No description";
        private const string DefaultDisplayText = "-";
        private const string DescriptionFieldName = "description";
        private const string DescriptionPropertyName = "Description";

        private static readonly GUIStyle WrappedLabelStyle = new GUIStyle(ScriptablesStyles.WindowLabelStyle);
        private static readonly GUIContent LabelContent = new GUIContent();

        // Filters
        private readonly string FilterDebuggable = $"t:{nameof(BaseEditorDebuggableScriptable)}";
        private readonly string FilterSettings = $"t:{typeof(ScriptableSettings<>).Name}";
        private readonly string FilterRuntime = $"t:{typeof(ScriptableRuntime<>).Name}";
        private readonly string FilterReactive = $"t:{typeof(ScriptableReactive<>).Name}";
        private readonly string FilterReactiveNoParams = $"t:{nameof(ReactiveNoParamsEditorDebuggableScriptable)}";
        private readonly string FilterScriptableObject = $"t:{nameof(ScriptableObject)}";

        // Variables
        private Tab currentTab = Tab.Scriptables;
        private Vector2 scrollPos;
        private GUIContent[] tabContents;
        private Object assetBeingRenamed = null;
        private string newName = string.Empty;
        private bool isRenaming = false;

        private List<Object> currentDisplayList = new List<Object>();
        private int selectedIndex = -1;
        private bool isKeyboardNavigation = false;
        private bool pendingRefresh;
        private string previousSearchFilter = string.Empty;

        private List<Object> validAssetsBuffer = new List<Object>();
        private float cachedPathWidth;
        private float cachedDescWidth;

        // Pagination
        private Pagination<Object> pagination = new Pagination<Object>(PaginationItemsPerPage);

        // Per‑tab caches and loading UI
        private Dictionary<Tab, List<Object>> cachedAssets = new Dictionary<Tab, List<Object>>();
        private Dictionary<Tab, LoadingIndicator> loadingUIs = new Dictionary<Tab, LoadingIndicator>();

        // Search
        private SearchBar searchBar = new SearchBar();

        /// <summary>
        /// Opens or focuses the Scriptables Panel window.
        /// </summary>
        internal static void ShowWindow()
        {
            ScriptablesPanelWindow window = GetWindow<ScriptablesPanelWindow>(true, WindowTitle);
            window.minSize = new Vector2(MinWindowWidth, MinWindowHeight);
            window.Show();
        }

        private void OnEnable()
        {
            tabContents = new GUIContent[] {
                new GUIContent(TabScriptablesLabel, ScriptablesStylesIcons.GetIconTexture(ScriptablesStylesIcons.IconType.SmallMainScriptables)),
                new GUIContent(TabSettingsLabel, ScriptablesStylesIcons.GetIconTexture(ScriptablesStylesIcons.IconType.SmallSettings)),
                new GUIContent(TabRuntimeLabel, ScriptablesStylesIcons.GetIconTexture(ScriptablesStylesIcons.IconType.SmallRuntime)),
                new GUIContent(TabReactiveLabel, ScriptablesStylesIcons.GetIconTexture(ScriptablesStylesIcons.IconType.SmallReactive)),
                new GUIContent(TabScriptableObjectLabel, ScriptablesStylesIcons.GetIconTexture(ScriptablesStylesIcons.IconType.SmallScriptableObject))
            };

            foreach (Tab tab in Enum.GetValues(typeof(Tab)))
            {
                cachedAssets[tab] = new List<Object>();
                loadingUIs[tab] = new LoadingIndicator();
            }

            // Start scan for the default tab
            if (cachedAssets[Tab.Scriptables].Count == 0 && !loadingUIs[Tab.Scriptables].IsLoading)
            {
                StartScanForCurrentTab();
            }
        }

        private void OnDisable()
        {
            // Cancel all ongoing scans
            foreach (LoadingIndicator ui in loadingUIs.Values)
            {
                ui.Cancel();
            }
        }

        private void OnGUI()
        {
            if (pendingRefresh)
            {
                pendingRefresh = false;
                RefreshCurrentTab();
                return;
            }

            float availableWidth = position.width - AssetColumnWidth - ActionsColumnWidth - RowHorizontalPadding;
            cachedPathWidth = availableWidth * PathWidthMultiplier;
            cachedDescWidth = availableWidth * DescWidthMultiplier;

            if (cachedPathWidth < MinPathWidth)
            {
                cachedPathWidth = MinPathWidth;
            }

            if (cachedDescWidth < MinDescriptionWidth)
            {
                cachedDescWidth = MinDescriptionWidth;
            }

            RenderTabs();
            RenderSearchAndRefresh();
            RenderHeader();
            RenderAssetList();
        }

        private void RenderTabs()
        {
            EditorGUILayout.BeginHorizontal();
            float tabWidth = position.width / tabContents.Length;

            for (int i = 0; i < tabContents.Length; i++)
            {
                bool isSelected = (int)currentTab == i;
                GUIStyle tabStyle = isSelected ? ScriptablesStyles.SelectedTabStyle : ScriptablesStyles.UnselectedTabStyle;

                if (GUILayout.Toggle(isSelected, tabContents[i], tabStyle, GUILayout.Width(tabWidth), GUILayout.Height(TabHeight)))
                {
                    if (!isSelected)
                    {
                        searchBar.Unfocus();
                        currentTab = (Tab)i;
                        scrollPos = Vector2.zero;
                        selectedIndex = -1;
                        isKeyboardNavigation = false;
                        pagination.Reset();

                        // If this tab's cache is empty and not already loading, start its scan
                        if (cachedAssets[currentTab].Count == 0 && !loadingUIs[currentTab].IsLoading)
                        {
                            StartScanForCurrentTab();
                        }
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void RenderSearchAndRefresh()
        {
            EditorGUILayout.BeginHorizontal(ScriptablesStyles.WindowSearchBarBgStyle);

            searchBar.Draw();

            using (new EditorGUI.DisabledScope(loadingUIs[currentTab].IsLoading))
            {
                ButtonPalette.DrawButton(
                    label: RefreshButtonLabel,
                    buttonIcon: ScriptablesStylesIcons.ButtonIcon.RefreshIcon,
                    fixedWidth: RefreshButtonWidth,
                    customHeight: ButtonHeight,
                    style: ScriptablesStylesColors.ButtonColorStyle.Plain,
                    tooltip: RefreshButtonLabel,
                    action: () =>
                    {
                        ClearCacheForTab(currentTab);
                        StartScanForCurrentTab();
                    }
                );
            }

            EditorGUILayout.EndHorizontal();
        }

        private void RenderHeader()
        {
            EditorGUILayout.BeginHorizontal(ScriptablesStyles.WindowHeaderStyle, GUILayout.Height(HeaderHeight));

            GUILayout.Label(AssetColumnHeader, ScriptablesStyles.WindowHeaderLabelStyle, GUILayout.Width(AssetColumnWidth));
            GUILayout.Label(PathColumnHeader, ScriptablesStyles.WindowHeaderLabelStyle, GUILayout.Width(cachedPathWidth));
            GUILayout.Label(DescriptionColumnHeader, ScriptablesStyles.WindowHeaderLabelStyle, GUILayout.Width(cachedDescWidth));
            GUILayout.Label(ActionsColumnHeader, ScriptablesStyles.WindowHeaderLabelStyle, GUILayout.Width(ActionsColumnWidth));

            EditorGUILayout.EndHorizontal();
        }

        private void RenderAssetList()
        {
            LoadingIndicator currentUI = loadingUIs[currentTab];

            if (currentUI.DrawProgress())
            {
                Repaint();
                return;
            }

            ApplyFilterAndUpdateDisplay();

            if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.UpArrow || Event.current.keyCode == KeyCode.DownArrow)
                {
                    HandleKeyboardNavigation(Event.current.keyCode);
                    Event.current.Use();
                    isKeyboardNavigation = true;
                    Repaint();
                }
            }

            if (isKeyboardNavigation && selectedIndex >= 0 && selectedIndex < currentDisplayList.Count)
            {
                Selection.activeObject = currentDisplayList[selectedIndex];
            }

            GUI.SetNextControlName(AssetListScrollViewName);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);

            if (currentDisplayList.Count == 0)
            {
                GUILayout.Space(EmptyStateSpacing);
                GUILayout.Label(NoItemsLabel, ScriptablesStyles.WindowEmptyLabelStyle);
            }
            else
            {
                for (int i = 0; i < currentDisplayList.Count; i++)
                {
                    Object asset = currentDisplayList[i];
                    if (asset == null)
                    {
                        pendingRefresh = true;
                        break;
                    }
                    RenderAssetRow(asset, i);
                }
            }

            EditorGUILayout.EndScrollView();

            if (pagination.DrawControls())
            {
                scrollPos = Vector2.zero;
            }

            if (isKeyboardNavigation && selectedIndex >= 0 && selectedIndex < currentDisplayList.Count)
            {
                float targetY = selectedIndex * RowHeight;
                float viewTop = scrollPos.y;
                float viewBottom = viewTop + position.height - ScrollViewOffset;

                if (targetY < viewTop)
                {
                    scrollPos.y = targetY;
                }
                else if (targetY + RowHeight > viewBottom)
                {
                    scrollPos.y = targetY - (viewBottom - viewTop) + RowHeight;
                }
            }
        }

        private void ApplyFilterAndUpdateDisplay()
        {
            if (!cachedAssets.ContainsKey(currentTab))
            {
                return;
            }

            validAssetsBuffer.Clear();

            foreach (Object asset in cachedAssets[currentTab])
            {
                if (asset != null)
                {
                    validAssetsBuffer.Add(asset);
                }
            }

            if (validAssetsBuffer.Count != cachedAssets[currentTab].Count)
            {
                pendingRefresh = true;
                return;
            }

            string currentSearch = searchBar.CurrentFilter;
            bool searchChanged = currentSearch != previousSearchFilter;
            if (searchChanged)
            {
                previousSearchFilter = currentSearch;
            }

            List<Object> filteredAssets = searchBar.Apply(validAssetsBuffer, obj => obj.name);
            pagination.SetItems(filteredAssets, resetToFirstPage: searchChanged);
            currentDisplayList = pagination.CurrentPageItems;

            if (selectedIndex >= currentDisplayList.Count)
            {
                selectedIndex = currentDisplayList.Count - 1;
            }
        }

        private void RefreshCurrentTab()
        {
            ClearCacheForTab(currentTab);
            StartScanForCurrentTab();
        }

        private void HandleKeyboardNavigation(KeyCode keyCode)
        {
            if (currentDisplayList.Count == 0)
            {
                return;
            }

            if (selectedIndex == -1)
            {
                selectedIndex = IndexStart;
            }
            else if (keyCode == KeyCode.UpArrow)
            {
                selectedIndex = Mathf.Max(0, selectedIndex - IndexIncrement);
            }
            else if (keyCode == KeyCode.DownArrow)
            {
                selectedIndex = Mathf.Min(currentDisplayList.Count - IndexIncrement, selectedIndex + IndexIncrement);
            }
            else if (keyCode == KeyCode.Home)
            {
                selectedIndex = IndexStart;
            }
            else if (keyCode == KeyCode.End)
            {
                selectedIndex = currentDisplayList.Count - IndexIncrement;
            }
        }

        private void StartScanForCurrentTab()
        {
            LoadingIndicator ui = loadingUIs[currentTab];
            if (ui.IsLoading)
            {
                return;
            }

            string[] filters = GetFiltersForTab(currentTab);
            string[] excludeFilters = currentTab == Tab.ScriptableObjects
                ? new[] { FilterDebuggable } : null;
            ui.StartScan(filters, excludeFilters, OnScanCompleted);
            cachedAssets[currentTab].Clear();
            ApplyFilterAndUpdateDisplay();
        }

        private void OnScanCompleted(List<Object> results)
        {
            if (results != null)
            {
                cachedAssets[currentTab] = results;
                ApplyFilterAndUpdateDisplay();
            }
            Repaint();
        }

        private string[] GetFiltersForTab(Tab tab)
        {
            return tab switch
            {
                Tab.Scriptables => new[] { FilterDebuggable },
                Tab.Settings => new[] { FilterSettings },
                Tab.Runtime => new[] { FilterRuntime },
                Tab.Reactive => new[] { FilterReactive, FilterReactiveNoParams },
                Tab.ScriptableObjects => new[] { FilterScriptableObject },
                _ => new string[0]
            };
        }

        private void ClearCacheForTab(Tab tab)
        {
            cachedAssets[tab].Clear();
            ApplyFilterAndUpdateDisplay();
        }

        private void RenderAssetRow(Object asset, int index)
        {
            string path = AssetDatabase.GetAssetPath(asset);
            Rect rowRect = EditorGUILayout.BeginHorizontal();

            bool isSelected = (Selection.activeObject == asset) ||
                (isKeyboardNavigation && selectedIndex == index);

            bool isHover = rowRect.Contains(Event.current.mousePosition) && !isRenaming;

            if (isSelected)
            {
                EditorGUI.DrawRect(rowRect, ScriptablesStylesColors.GetSelectionColor());

                if (Selection.activeObject == asset && (!isKeyboardNavigation || selectedIndex != index))
                {
                    selectedIndex = index;
                    isKeyboardNavigation = false;
                }
            }
            else if (isHover && Event.current.type == EventType.Repaint)
            {
                EditorGUI.DrawRect(rowRect, ScriptablesStylesColors.GetHoverColor());
            }
            else if (Event.current.type == EventType.Repaint)
            {
                EditorGUI.DrawRect(rowRect, ScriptablesStylesColors.GetRowBackgroundColor());
            }

            RenderAssetNameAndIcon(asset, path, Mathf.Min(AssetColumnWidth, position.width * MaxWidthMultiplier));
            RenderAssetPath(path, cachedPathWidth);
            RenderAssetDescription(asset, cachedDescWidth);
            RenderActionButtons(asset, path);

            EditorGUILayout.EndHorizontal();
            HandleRowClick(asset, rowRect, index);
        }

        private void RenderAssetNameAndIcon(Object asset, string path, float maxWidth)
        {
            EditorGUILayout.BeginHorizontal(GUILayout.Width(maxWidth));

            GUILayout.Space(SpaceBeforeIcon);
            GUILayout.Label(AssetPreview.GetMiniThumbnail(asset), GUILayout.Width(IconSize), GUILayout.Height(IconSize));

            if (isRenaming && assetBeingRenamed == asset)
            {
                GUI.SetNextControlName(RenameFieldName);
                newName = EditorGUILayout.TextField(newName);
                EditorGUI.FocusTextInControl(RenameFieldName);

                if (Event.current.isKey && Event.current.keyCode == KeyCode.Return)
                {
                    ConfirmRename(asset, path);
                }
                else if (Event.current.isKey && Event.current.keyCode == KeyCode.Escape)
                {
                    CancelRename();
                }
            }
            else
            {
                string displayName = StringShortener.Shorten(asset.name, maxWidth - IconSize - NameLabelRightPadding, ScriptablesStyles.WindowLabelStyle);
                GUILayout.Label(displayName, GUILayout.ExpandWidth(true));
            }

            EditorGUILayout.EndHorizontal();
        }

        private void RenderAssetPath(string path, float availableWidth)
        {
            string displayPath = StringShortener.Shorten(path, availableWidth, ScriptablesStyles.WindowLabelStyle);
            LabelContent.text = displayPath;
            LabelContent.tooltip = path;
            GUILayout.Label(LabelContent, GUILayout.ExpandWidth(true), GUILayout.MinWidth(MinPathWidth));
        }

        private void RenderAssetDescription(Object asset, float width)
        {
            string desc = GetDescription(asset);
            string displayText;
            string tooltip;

            if (desc == null)
            {
                displayText = DefaultDisplayText;
                tooltip = NotScriptableTooltip;
            }
            else
            {
                // Replace newlines with spaces to keep it single line
                string singleLineDesc = desc.Replace('\n', ' ').Replace('\r', ' ');
                displayText = StringShortener.Shorten(singleLineDesc, width - DescriptionPadding, ScriptablesStyles.WindowLabelStyle);
                tooltip = desc;
            }

            WrappedLabelStyle.wordWrap = false;

            GUILayout.Space(SpaceBeforeDescription);
            LabelContent.text = displayText;
            LabelContent.tooltip = tooltip;
            GUILayout.Label(LabelContent, WrappedLabelStyle, GUILayout.Width(width - SpaceAfterDescription));
        }

        private void RenderActionButtons(Object asset, string path)
        {
            EditorGUILayout.BeginHorizontal(GUILayout.Width(ActionsColumnWidth));

            ButtonPalette.DrawButton(
                    label: SearchButtonLabel,
                    fixedWidth: ButtonWidth,
                    customHeight: ButtonHeight,
                    style: ScriptablesStylesColors.ButtonColorStyle.Neutral,
                    tooltip: SearchButtonLabel,
                    action: () =>
                    {
                        searchBar.Unfocus();
                        FindReferencesInScene(asset, path);
                    }
                );

            ButtonPalette.DrawButton(
                    label: RenameButtonLabel,
                    fixedWidth: ButtonWidth,
                    customHeight: ButtonHeight,
                    style: ScriptablesStylesColors.ButtonColorStyle.Neutral,
                    tooltip: RenameButtonLabel,
                    action: () =>
                    {
                        searchBar.Unfocus();
                        StartRenaming(asset);
                    }
                );
            
            ButtonPalette.DrawButton(
                    label: DeleteButtonLabel,
                    fixedWidth: ButtonWidth,
                    customHeight: ButtonHeight,
                    style: ScriptablesStylesColors.ButtonColorStyle.Urgent,
                    tooltip: DeleteButtonLabel,
                    action: () =>
                    {
                        searchBar.Unfocus();
                        HandleDeleteAction(asset, path);
                    }
                );

            EditorGUILayout.EndHorizontal();
        }

        private string GetDescription(Object asset)
        {
            // Check if the asset is derived from BaseEditorDebuggableScriptable
            Type type = asset.GetType();
            Type baseType = typeof(BaseEditorDebuggableScriptable);

            if (!baseType.IsAssignableFrom(type))
            {
                return null; // not a descendant
            }

            // Try to get description field
            FieldInfo field = type.GetField(DescriptionFieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (field != null)
            {
                object value = field.GetValue(asset);

                if (value is string str && !string.IsNullOrEmpty(str))
                {
                    return str;
                }
            }
            
            // Try property
            PropertyInfo prop = type.GetProperty(DescriptionPropertyName, BindingFlags.Public | BindingFlags.Instance);
            
            if (prop != null && prop.CanRead)
            {
                object value = prop.GetValue(asset);

                if (value is string str && !string.IsNullOrEmpty(str))
                {
                    return str;
                }
            }

            return NoDescriptionLabel;
        }

        private void HandleRowClick(Object asset, Rect rowRect, int index)
        {
            if (Event.current.type == EventType.MouseDown && rowRect.Contains(Event.current.mousePosition))
            {
                searchBar.Unfocus();

                if (isRenaming)
                {
                    ConfirmRename(assetBeingRenamed, AssetDatabase.GetAssetPath(assetBeingRenamed));
                }

                Selection.activeObject = asset;
                selectedIndex = index;
                isKeyboardNavigation = false;
                Event.current.Use();
            }
        }

        private void StartRenaming(Object asset)
        {
            Selection.activeObject = asset;
            assetBeingRenamed = asset;
            newName = asset.name;
            isRenaming = true;
        }

        private void FindReferencesInScene(Object asset, string path)
        {
            string searchQuery = $"{RefQuerySuffix}\"{path}\"";
            SearchContext context = SearchService.CreateContext(searchQuery);
            SearchService.ShowWindow(context);
        }

        private void ConfirmRename(Object asset, string path)
        {
            if (newName != asset.name && !string.IsNullOrEmpty(newName))
            {
                ScriptFileManager.Rename(path, newName);
            }
            CancelRename();
        }

        private void CancelRename()
        {
            isRenaming = false;
            assetBeingRenamed = null;
            newName = string.Empty;
            Repaint();
        }

        private void HandleDeleteAction(Object asset, string path)
        {
            Selection.activeObject = asset;
            Repaint();

            EditorApplication.delayCall += () =>
            {
                DisplayDialog.DialogButtonSettings negativeButtonSettings = new DisplayDialog.DialogButtonSettings(
                    buttonColorStyle: ScriptablesStylesColors.ButtonColorStyle.Neutral,
                    buttonIcon: ScriptablesStylesIcons.ButtonIcon.None,
                    label: CancelButtonLabel,
                    tooltip: CancelButtonLabel
                );

                DisplayDialog.DialogButtonSettings positiveButtonSettings = new DisplayDialog.DialogButtonSettings(
                    buttonColorStyle: ScriptablesStylesColors.ButtonColorStyle.Urgent,
                    buttonIcon: ScriptablesStylesIcons.ButtonIcon.ClearIcon,
                    label: DeleteButtonLabel,
                    tooltip: DeleteButtonLabel,
                    onClickAction: () => {
                            ScriptFileManager.Delete(path);
                            ClearCacheForTab(currentTab);
                            StartScanForCurrentTab();
                        }
                );

                DisplayDialog.Show(
                    title: ConfirmDeleteTitle,
                    message: string.Format(ConfirmDeleteMessage, asset.name),
                    negativeButtonSettings: negativeButtonSettings,
                    positiveButtonSettings: positiveButtonSettings
                );
            };
        }
    }
}
#endif