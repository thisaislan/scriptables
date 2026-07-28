#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using Thisaislan.Scriptables.Editor.Utilities;
using Thisaislan.Scriptables.Editor.Utilities.Styles;
using Thisaislan.Scriptables.Editor.Utilities.Widgets;
using Thisaislan.Scriptables.Editor.Windows.Data;
using Thisaislan.Scriptables.Editor.Windows.Data.Enums;
using UnityEditor;
using UnityEngine;

namespace Thisaislan.Scriptables.Editor.Windows
{
    internal class ScriptablesWizardWindow : EditorWindow
    {
        private enum ClassScriptSubType
        {
            Type,
            Class,
            Struct,
            Enum
        };

        private const float WindowWidth = 800f;
        private const float WindowHeight = 600f;
        private const float LeftPanelRatio = 0.28f;
        private const float BottomBarHeight = 60f;
        private const float InfoPanelHeight = 140f;
        private const float SearchHeight = 22f;
        private const int CustomItemsPerPage = 50;
        private const int ScriptableObjectItemsPerPage = 50;
        private const float ButtonHeight = 22f;
        private const float CategoryIconSize = 18f;
        private const float ItemIconSize = 16f;
        private const float ButtonWidth = 80f;
        private const int LabelWidth = 200;
        private const int ConfigHeaderSpacing = 20;
        private const int ConfigLateralSpacing = 20;
        private const int ConfigAfterTitleSpacing = 8;
        private const int ConfigBetweenFieldsSpacing = 2;
        private const int ConfigBetweenSectionsSpacing = 4;
        private const int ConfigBetweenBigSectionsSpacing = 6;
        private const int DropdownWidhtSize = 110;

        private const float ItemRowHeight = 28f;
        private const float InfoPanelIconSize = 28f;
        private const float PinnedLabelWidth = 32f;
        private const float RefreshButtonWidth = 80f;
        private const float RefreshButtonHeight = 20f;
        private const float BrowseButtonWidth = 70f;
        private const float BrowseButtonHeight = 20f;
        private const float LeftPanelItemPadding = 8f;
        private const float InfoPanelPadding = 10f;
        private const float InfoPanelSmallPadding = 6f;
        private const float InfoPanelTinyPadding = 2f;
        private const float BottomBarPadding = 12f;
        private const int DropdownSpacing = 90;
        private const string WindowTitleValue = "Scriptables Wizard";
        private const string PreviousButtonText = "Previous";
        private const string NextButtonText = "Next";
        private const string CancelButtonText = "Cancel";
        private const string FinishButtonText = "Finish";
        private const string RefreshButtonText = "Refresh";
        private const string BrowseButtonTooltip = "Browse";
        private const string PinnedNewLabel = "NEW";
        private const string NewItemName = "New";
        private const string NoItemsFoundText = "No items found.";
        private const string ScanningTitle = "Scanning Assets...";
        private const string ScanningStatus = "Searching for project scripts...";
        private const string CreateNewPrefix = "Create New ";
        private const string NameFieldLabel = "Name";
        private const string FolderFieldLabel = "Folder";
        private const string HasNamespaceLabel = "Set Namespace";
        private const string NamespaceFieldLabel = "Namespace";
        private const string CreateMenuItemLabel = "Create Menu Item";
        private const string MenuPathFieldLabel = "Menu Path";
        private const string FileNameFieldLabel = "File Name";
        private const string NameRequiredMessage = "Name is required.";
        private const string AssetsFolderPrefix = "Assets/";
        private const string AssetsFolderPath = "Assets";
        private const string SelectFolderDialogTitle = "Select Folder";
        private const string InvalidFolderDialogTitle = "Invalid Folder";
        private const string InvalidFolderDialogMessage = "Please select a folder inside the project's Assets folder.";
        private const string OkButtonText = "OK";
        private const string ErrorDialogTitle = "Error";
        private const string ErrorDialogMessage = "Failed to create: ";
        private const string ScriptNotFoundDialogTitle = "Script Not Found";
        private const string ScriptNotFoundDialogMessage = "The script '{0}' was deleted from disk. Please refresh the list and try again.";
        private const string ScriptableObjectAssetExtension = ".asset";
        private const string ScriptExtension = ".cs";
        private const char SlashChar = '/';
        private const string SlashString = "/";
        private const string ScriptFilter = "t:Script";
        private const string UnknownName = "Unknown";
        private const string FileExistsTitle = "File Exists";
        private const string FileExistsMessageFormat = "A file named '{0}{1}' already exists in this folder. Please rename the file or choose a different folder.";
        private const string DefaultDataFieldTypeValue = "Object";
        private const string DefaultDataFieldInnerValue = "NewData";
        private const string DefaultDataFieldEnumValue = "NewState";
        private const string DataConfigurationLabel = "Data Configuration";
        private const string DropdownTooltip = "Select the data type";

        private static readonly GUIContent DropdownTooltipContent = new GUIContent();

        private readonly WizardCategoryDataProvider categoryDataProvider = new WizardCategoryDataProvider();
        private readonly LoadingIndicator loadingIndicator = new LoadingIndicator();
        private readonly List<WizardScriptablesCategory> categories = new List<WizardScriptablesCategory>();
        private int selectedCategoryIndex;
        private int selectedItemIndex = -1;
        private string searchFilter = string.Empty;
        private string previousSearchFilter = string.Empty;
        private Vector2 rightScroll;
        private Vector2 configScroll;
        private Pagination<WizardItemData> customPagination;
        private Pagination<WizardItemData> scriptablePagination;
        private List<WizardItemData> currentItems = new List<WizardItemData>();
        private List<WizardItemData> displayItems = new List<WizardItemData>();
        private Dictionary<WizardScriptablesCategory, List<WizardItemData>> itemsCache = new Dictionary<WizardScriptablesCategory, List<WizardItemData>>();
        private HashSet<WizardScriptablesCategory> loadingCategories = new HashSet<WizardScriptablesCategory>();
        private readonly string[] subTypeLabels = { "Type", "Class", "Struct", "Enum" };
        private bool isConfiguring;
        private string targetFolder = AssetsFolderPrefix;
        private bool isCreating;
        private WizardItemData configuringItem;
        private bool isNewItemConfiguration;
        private bool namespaceManuallyEdited;
        private bool menuPathManuallyEdited;
        private bool fileNameManuallyEdited;

        private string assetName = string.Empty;
        private string assetNamespace = string.Empty;
        private string menuItem = string.Empty;
        private string fileName = string.Empty;
        private int dataSubType;
        private string dataFieldValue = string.Empty;
        private bool dataFieldManuallyEdited;
        private bool hasNamespace;
        private bool hasMenuItem;

        internal static void ShowWizard()
        {
            ScriptablesWizardWindow window = GetWindow<ScriptablesWizardWindow>(true, WindowTitleValue);
            window.minSize = new Vector2(WindowWidth, WindowHeight);
            window.maxSize = new Vector2(WindowWidth, WindowHeight);
            window.ShowUtility();
        }

        private void OnEnable()
        {
            customPagination = new Pagination<WizardItemData>(CustomItemsPerPage);
            scriptablePagination = new Pagination<WizardItemData>(ScriptableObjectItemsPerPage);
            targetFolder = NormalizeFolderPath(GetSelectedFolder());
            InitializeCategories();
        }

        private void InitializeCategories()
        {
            categories.Clear();
            categories.AddRange(categoryDataProvider.GetCategories());
            selectedCategoryIndex = 0;
            RefreshItemsForCurrentCategory();
        }

        private static Texture LoadDefaultIcon()
        {
            return ScriptablesStylesIcons.GetIconTexture(ScriptablesStylesIcons.IconType.Default);
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));

            if (isConfiguring)
            {
                DrawConfigurationContent();
            }
            else
            {
                DrawMainContent();
            }

            DrawBottomInfo();

            if (isConfiguring)
            {
                DrawConfigurationBottomBar();
            }
            else
            {
                DrawBottomBar();
            }

            EditorGUILayout.EndVertical();

            UpdateSelectedItem();
        }

        private void UpdateSelectedItem()
        {
            if (Event.current.type == EventType.Repaint)
            {
                WizardScriptablesCategory? category = CurrentCategory;

                if (category.HasValue)
                {
                    if (selectedItemIndex >= 0 && selectedItemIndex < displayItems.Count)
                    {
                        WizardItemData item = displayItems[selectedItemIndex];

                        if (item != null && !string.IsNullOrEmpty(item.AssetPath))
                        {
                            GUI.tooltip = item.AssetPath;
                        }
                    }
                }
            }
        }

        private void DrawMainContent()
        {
            EditorGUILayout.BeginHorizontal(GUILayout.ExpandHeight(true));
            DrawLeftPanel();
            DrawRightPanel();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawLeftPanel()
        {
            EditorGUILayout.BeginVertical(ScriptablesStyles.WindowCategoryAreaStyle, GUILayout.Width(position.width * LeftPanelRatio), GUILayout.ExpandHeight(true));

            for (int i = 0; i < categories.Count; i++)
            {
                WizardScriptablesCategory category = categories[i];
                bool isSelected = i == selectedCategoryIndex;
                GUIStyle style = isSelected ? ScriptablesStyles.WindowCategoryBtnSelectedStyle : ScriptablesStyles.WindowCategoryBtnStyle;
                float availableHeight = position.height - BottomBarHeight - InfoPanelHeight;
                float buttonHeight = availableHeight/categories.Count - 1;

                EditorGUILayout.BeginHorizontal(style, GUILayout.Height(buttonHeight));

                GUILayout.Space(LeftPanelItemPadding);

                Texture icon = categoryDataProvider.GetCategoryIcon(category) ?? LoadDefaultIcon();
                GUILayout.Label(icon, GUILayout.Width(CategoryIconSize), GUILayout.Height(CategoryIconSize));

                GUILayout.Space(LeftPanelItemPadding);
                EditorGUILayout.LabelField(categoryDataProvider.GetCategoryTitle(category), isSelected ? ScriptablesStyles.WindowBoldLabelStyle : ScriptablesStyles.WindowLabelStyle, GUILayout.ExpandWidth(true));

                EditorGUILayout.EndHorizontal();

                Rect lastRect = GUILayoutUtility.GetLastRect();

                if (Event.current.type == EventType.MouseDown && lastRect.Contains(Event.current.mousePosition))
                {
                    GUI.FocusControl(null);
                    SelectCategory(i);
                    Event.current.Use();
                    Repaint();
                }
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawRightPanel()
        {
            EditorGUILayout.BeginVertical(ScriptablesStyles.WindowItemAreaStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            DrawSearchBar();

            rightScroll = EditorGUILayout.BeginScrollView(rightScroll, GUILayout.ExpandHeight(true));

            DrawLoadingIndicator();
            DrawItemList();
            DrawEmptyState();

            EditorGUILayout.EndScrollView();

            DrawPaginationControls();

            EditorGUILayout.EndVertical();
        }

        private void DrawLoadingIndicator()
        {
            WizardScriptablesCategory? loadingCategory = CurrentCategory;

            if (loadingCategory.HasValue && loadingCategories.Contains(loadingCategory.Value))
            {
                loadingIndicator.DrawProgressCard(ScanningTitle, ScanningStatus, 0f);
            }
        }

        private void DrawItemList()
        {
            for (int i = 0; i < displayItems.Count; i++)
            {
                WizardItemData item = displayItems[i];

                if (item == null)
                {
                    continue;
                }

                bool isSelected = i == selectedItemIndex;
                DrawItemRow(i, item, isSelected);
            }
        }

        private void DrawItemRow(int index, WizardItemData item, bool isSelected)
        {
            GUIStyle style = isSelected ? ScriptablesStyles.WindowItemBtnSelectedStyle : ScriptablesStyles.WindowItemBtnStyle;

            try
            {
                EditorGUILayout.BeginHorizontal(style, GUILayout.Height(ItemRowHeight));
                GUILayout.Space(LeftPanelItemPadding);
                GUILayout.Label(item.Icon ?? LoadDefaultIcon(), GUILayout.Width(ItemIconSize), GUILayout.Height(ItemIconSize));
                GUILayout.Space(LeftPanelItemPadding);
                EditorGUILayout.LabelField(item.Name ?? string.Empty, ScriptablesStyles.WindowLabelStyle, GUILayout.ExpandWidth(true));

                if (item.IsPinned)
                {
                    GUILayout.Label(PinnedNewLabel, ScriptablesStyles.WindowPinnedLabelStyle, GUILayout.Width(PinnedLabelWidth));
                }

                EditorGUILayout.EndHorizontal();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"ScriptablesWizardWindow.DrawItemRow encountered an error: {e.Message}");
                EditorGUILayout.EndHorizontal();
                return;
            }

            Rect lastRect = GUILayoutUtility.GetLastRect();

            if (Event.current.type == EventType.MouseDown && lastRect.Contains(Event.current.mousePosition))
            {
                GUI.FocusControl(null);
                selectedItemIndex = index;
                Event.current.Use();
                Repaint();
            }
        }

        private void DrawEmptyState()
        {
            WizardScriptablesCategory? displayCategory = CurrentCategory;

            if (displayItems.Count == 0 && string.IsNullOrEmpty(searchFilter) &&
                (!displayCategory.HasValue || !loadingCategories.Contains(displayCategory.Value)))
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField(NoItemsFoundText, ScriptablesStyles.WindowEmptyLabelStyle, GUILayout.ExpandWidth(true));
                GUILayout.FlexibleSpace();
            }
        }

        private void DrawPaginationControls()
        {
            WizardScriptablesCategory? category = CurrentCategory;

            bool handled = TryDrawPagination(category, WizardScriptablesCategory.Custom, customPagination);
            if (!handled)
            {
                TryDrawPagination(category, WizardScriptablesCategory.ScriptableObject, scriptablePagination);
            }
        }

        private bool TryDrawPagination(WizardScriptablesCategory? category, WizardScriptablesCategory targetCategory, Pagination<WizardItemData> pagination)
        {
            if (!category.HasValue || category.Value != targetCategory || !itemsCache.ContainsKey(category.Value))
            {
                return false;
            }

            if (pagination.DrawControls())
            {
                rightScroll = Vector2.zero;
                RefreshDisplayItems();
            }

            return true;
        }

        private void DrawSearchBar()
        {
            EditorGUILayout.BeginHorizontal(GUILayout.Height(SearchHeight));
            GUILayout.Space(LeftPanelItemPadding);
            string newSearch = EditorGUILayout.TextField(searchFilter, ScriptablesStyles.ToolbarSearchFieldStyle, GUILayout.ExpandWidth(true));

            if (newSearch != searchFilter)
            {
                searchFilter = newSearch;
                RefreshDisplayItems();
                Repaint();
            }

            WizardScriptablesCategory? category = CurrentCategory;

            bool showRefresh = category.HasValue &&
                (category.Value == WizardScriptablesCategory.Custom ||
                 category.Value == WizardScriptablesCategory.ScriptableObject);

            if (showRefresh)
            {
                using (new EditorGUI.DisabledScope(
                    category.HasValue && loadingCategories.Contains(category.Value)))
                {
                    ButtonPalette.DrawButton(
                        label: RefreshButtonText,
                        buttonIcon: ScriptablesStylesIcons.ButtonIcon.RefreshIcon,
                        fixedWidth: RefreshButtonWidth,
                        customHeight: RefreshButtonHeight,
                        style: ScriptablesStylesColors.ButtonColorStyle.Plain,
                        tooltip: RefreshButtonText,
                        action: PerformRefresh
                    );
                }
            }
            GUILayout.Space(LeftPanelItemPadding);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawBottomInfo()
        {
            WizardScriptablesCategory? category = CurrentCategory;

            if (!category.HasValue)
            {
                return;
            }

            EditorGUILayout.BeginHorizontal(ScriptablesStyles.WindowInfoBoxStyle, GUILayout.Height(InfoPanelHeight), GUILayout.ExpandWidth(true));
            
            GUILayout.Space(InfoPanelPadding);
            Texture icon = categoryDataProvider.GetCategoryIcon(category.Value) ?? LoadDefaultIcon();
            GUILayout.Label(icon, GUILayout.Width(InfoPanelIconSize), GUILayout.Height(InfoPanelIconSize));
            GUILayout.Space(InfoPanelPadding);
            
            EditorGUILayout.BeginVertical();
            GUILayout.Space(InfoPanelSmallPadding);
            EditorGUILayout.LabelField(categoryDataProvider.GetCategoryTitle(category.Value), ScriptablesStyles.WindowTitleLabelStyle);
            GUILayout.Space(InfoPanelTinyPadding);
            EditorGUILayout.LabelField(categoryDataProvider.GetCategoryDescription(category.Value), ScriptablesStyles.WindowDescLabelStyle, GUILayout.ExpandWidth(true));
            
            GUILayout.Space(InfoPanelSmallPadding);
            EditorGUILayout.EndVertical();
            
            GUILayout.Space(InfoPanelPadding);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawBottomBar()
        {
            DrawBottomActionButtons(
                new ButtonParams
                {
                    text = PreviousButtonText,
                    enabled = false,
                    action = null
                },
                new ButtonParams
                {
                    text = NextButtonText,
                    enabled = selectedItemIndex >= 0,
                    action = HandleNextAction
                },
                new ButtonParams
                {
                    text = CancelButtonText,
                    enabled = true,
                    action = CloseAction
                }
            );
        }

        private void PerformRefresh()
        {
            GUI.FocusControl(null);
            WizardScriptablesCategory? category = CurrentCategory;

            if (!category.HasValue)
            {
                return;
            }

            if (category.Value == WizardScriptablesCategory.Custom)
            {
                categoryDataProvider.ClearCustomScriptCache();
            }

            itemsCache.Remove(category.Value);
            loadingCategories.Remove(category.Value);
            RefreshItemsForCurrentCategory();
            Repaint();
        }

        private static void ShowDialog(string title, string message, Action onClickAction = null)
        {
            EditorApplication.delayCall += () =>
            {
                DisplayDialog.DialogButtonSettings buttonSettings =
                    new DisplayDialog.DialogButtonSettings(
                        buttonColorStyle: ScriptablesStylesColors.ButtonColorStyle.Neutral,
                        buttonIcon: ScriptablesStylesIcons.ButtonIcon.None,
                        label: OkButtonText,
                        tooltip: OkButtonText,
                        onClickAction: onClickAction
                    );

                DisplayDialog.Show(
                    title: title,
                    message: message,
                    negativeButtonSettings: buttonSettings
                );
            };
        }

        private void HandleNextAction()
        {
            GUI.FocusControl(null);
            HandleNext();
        }

        private void CloseAction()
        {
            GUI.FocusControl(null);
            Close();
        }

        private static bool IsItemPinned(WizardItemData item)
        {
            return item.IsPinned;
        }

        private static bool IsItemNotPinned(WizardItemData item)
        {
            return !item.IsPinned;
        }

        private bool MatchesFilter(WizardItemData item)
        {
            return item.IsPinned || string.IsNullOrEmpty(searchFilter) ||
                   item.Name.IndexOf(searchFilter, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void HandleNext()
        {
            WizardScriptablesCategory? category = CurrentCategory;

            if (!category.HasValue || selectedItemIndex < 0 || selectedItemIndex >= displayItems.Count)
            {
                return;
            }

            WizardItemData item = displayItems[selectedItemIndex];

            if (item == null)
            {
                return;
            }

            if (item.IsPinned)
            {
                SetNextAction(category.Value);
                StartNewItemConfig(category.Value);
            }
            else
            {
                if (item.ScriptType != null)
                {
                    if (!string.IsNullOrEmpty(item.AssetPath))
                    {
                        string fullPath = Path.Combine(
                            Path.GetDirectoryName(Application.dataPath),
                            item.AssetPath);

                        if (!File.Exists(fullPath))
                        {
                            ShowDialog(
                                ScriptNotFoundDialogTitle,
                                string.Format(ScriptNotFoundDialogMessage, item.Name),
                                PerformRefresh);
                            return;
                        }
                    }

                    EnterSimpleConfig(item);
                }
                else if (!string.IsNullOrEmpty(item.AssetPath))
                {
                    AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(item.AssetPath));
                    Close();
                }
            }
        }

        private void SetNextAction(WizardScriptablesCategory category)
        {
            string newName = categoryDataProvider.GetCategoryNewItemName(category);
            if (!string.IsNullOrEmpty(newName))
            {
                assetName = newName;
            }

            dataSubType = category == WizardScriptablesCategory.Reactive
                ? (int)WizardScriptCreationProvider.DataSubType.Type
                : (int)WizardScriptCreationProvider.DataSubType.Class;

            dataFieldValue = GetDefaultDataFieldValue();
            dataFieldManuallyEdited = false;
        }

        private void EnterSimpleConfig(WizardItemData item)
        {
            isConfiguring = true;
            isNewItemConfiguration = false;
            configuringItem = item;
            assetName = item.Name;
            hasNamespace = false;
            hasMenuItem = false;
            menuItem = string.Empty;
        }

        private void StartNewItemConfig(WizardScriptablesCategory category)
        {
            isConfiguring = true;
            isNewItemConfiguration = true;
            configuringItem = displayItems[selectedItemIndex];
            hasNamespace = false;
            hasMenuItem = false;
            menuItem = string.Empty;
            fileName = string.Empty;
            namespaceManuallyEdited = false;
            menuPathManuallyEdited = false;
            fileNameManuallyEdited = false;
            dataSubType = category == WizardScriptablesCategory.Reactive
                ? (int)WizardScriptCreationProvider.DataSubType.Type
                : (int)WizardScriptCreationProvider.DataSubType.Class;
            dataFieldValue = GetDefaultDataFieldValue();
            dataFieldManuallyEdited = false;
            PrefillFromFolder();
        }

        private void PrefillFromFolder()
        {
            if (!isNewItemConfiguration)
            {
                return;
            }

            TryDetectFromExistingScripts(out string detectedNamespace, out string detectedMenuPath);

            ApplyDetectedNamespace(detectedNamespace);
            ApplyDetectedMenuPath(detectedMenuPath);

            if (!fileNameManuallyEdited)
            {
                fileName = assetName;
            }
        }

        private void TryDetectFromExistingScripts(out string detectedNamespace, out string detectedMenuPath)
        {
            detectedNamespace = null;
            detectedMenuPath = null;

            try
            {
                string searchFolder = targetFolder.TrimEnd(SlashChar);
                string[] guids = AssetDatabase.FindAssets(ScriptFilter, new[] { searchFolder });

                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);

                    int lastSlash = path.LastIndexOf(SlashChar);
                    string scriptDir = lastSlash >= 0 ? path.Substring(0, lastSlash) : path;
                    if (!string.Equals(scriptDir, searchFolder, StringComparison.Ordinal))
                    {
                        continue;
                    }

                    MonoScript monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(path);

                    if (monoScript == null)
                    {
                        continue;
                    }

                    Type type = monoScript.GetClass();

                    if (type == null || !type.IsSubclassOf(typeof(ScriptableObject)))
                    {
                        continue;
                    }

                    if (detectedNamespace == null && !string.IsNullOrEmpty(type.Namespace))
                    {
                        detectedNamespace = type.Namespace;
                    }

                    if (detectedMenuPath == null)
                    {
                        detectedMenuPath = ExtractMenuPathFromType(type);
                    }

                    if (detectedNamespace != null && detectedMenuPath != null)
                    {
                        break;
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"ScriptablesWizardWindow.TryDetectFromExistingScripts encountered an error: {e.Message}");
            }
        }

        private static string ExtractMenuPathFromType(Type type)
        {
            object[] attrs = type.GetCustomAttributes(typeof(CreateAssetMenuAttribute), false);
            if (attrs.Length > 0)
            {
                CreateAssetMenuAttribute menuAttr = attrs[0] as CreateAssetMenuAttribute;
                if (menuAttr != null && !string.IsNullOrEmpty(menuAttr.menuName))
                {
                    string menuBase = menuAttr.menuName;
                    int menuLastSlash = menuBase.LastIndexOf(SlashChar);
                    return menuLastSlash >= 0
                        ? menuBase.Substring(0, menuLastSlash + 1)
                        : string.Empty;
                }
            }
            return null;
        }

        private void ApplyDetectedNamespace(string detectedNamespace)
        {
            if (!namespaceManuallyEdited)
            {
                if (!string.IsNullOrEmpty(detectedNamespace))
                {
                    hasNamespace = true;
                    assetNamespace = detectedNamespace;
                }
                else
                {
                    hasNamespace = false;
                    assetNamespace = string.Empty;
                }
            }
        }

        private void ApplyDetectedMenuPath(string detectedMenuPath)
        {
            if (!menuPathManuallyEdited)
            {
                if (!string.IsNullOrEmpty(detectedMenuPath))
                {
                    hasMenuItem = true;
                    menuItem = detectedMenuPath;
                }
                else
                {
                    hasMenuItem = false;
                    menuItem = string.Empty;
                }
            }
        }

        private void UpdateAutoFields()
        {
            if (!isNewItemConfiguration)
            {
                return;
            }

            if (!menuPathManuallyEdited && hasMenuItem &&
                !string.IsNullOrEmpty(menuItem))
            {
                string menuPath = menuItem;
                int lastSlash = menuPath.LastIndexOf(SlashChar);
                if (lastSlash >= 0)
                {
                    menuItem = menuPath.Substring(0, lastSlash + 1) + assetName;
                }
                else
                {
                    menuItem = assetName;
                }
            }

            if (!fileNameManuallyEdited)
            {
                fileName = assetName;
            }
        }

        private void DrawConfigurationContent()
        {
            WizardScriptablesCategory? category = CurrentCategory;

            if (!category.HasValue)
            {
                return;
            }

            GUILayout.Space(ConfigHeaderSpacing);

            EditorGUILayout.BeginHorizontal(GUILayout.ExpandHeight(true));

            GUILayout.Space(ConfigLateralSpacing);

            EditorGUILayout.BeginVertical(ScriptablesStyles.WindowItemAreaStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            configScroll = EditorGUILayout.BeginScrollView(configScroll, GUILayout.ExpandHeight(true));

            EditorGUILayout.BeginVertical();


            if (isNewItemConfiguration)
            {
                DrawNewItemConfig(category.Value);
            }
            else
            {
                DrawExistingItemConfig(category.Value);
            }

            EditorGUILayout.EndVertical();

            if (string.IsNullOrEmpty(assetName))
            {
                GUILayout.Box(NameRequiredMessage, ScriptablesStyles.DarkHelpBox);
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();

            GUILayout.Space(ConfigLateralSpacing);

            EditorGUILayout.EndHorizontal();
        }

        private void DrawNewItemConfig(WizardScriptablesCategory category)
        {
            EditorGUILayout.LabelField(CreateNewPrefix + categoryDataProvider.GetCategoryTitle(category), ScriptablesStyles.WindowBoldLabelStyle);

            GUILayout.Space(ConfigAfterTitleSpacing);

            DrawNameAndFolderFields();

            if (category == WizardScriptablesCategory.Reactive ||
                category == WizardScriptablesCategory.Settings ||
                category == WizardScriptablesCategory.Runtime)
            {
                GUILayout.Space(ConfigBetweenBigSectionsSpacing);
                EditorGUILayout.LabelField(DataConfigurationLabel, ScriptablesStyles.WindowBoldLabelStyle, GUILayout.Width(LabelWidth));
                GUILayout.Space(ConfigBetweenFieldsSpacing);
                DrawDataConfigField(category);
                GUILayout.Space(ConfigBetweenFieldsSpacing); // Add extra space
                GUILayout.Space(ConfigBetweenBigSectionsSpacing);
            }
            else
            {
                GUILayout.Space(ConfigBetweenBigSectionsSpacing);
            }

            DrawNamespaceField();
            GUILayout.Space(ConfigBetweenSectionsSpacing);
            DrawMenuItemFields();
        }

        private void DrawNameAndFolderFields()
        {
            string previousName = assetName;
            assetName = DrawLabelledTextField(NameFieldLabel, assetName);

            if (assetName != previousName)
            {
                UpdateAutoFields();
                UpdateDataFieldFromAssetName();
            }

            string previousFolder = targetFolder;
            DrawConfigFolder();

            if (targetFolder != previousFolder)
            {
                PrefillFromFolder();
            }
        }

        private void DrawNamespaceField()
        {
            hasNamespace = GUILayout.Toggle(hasNamespace, HasNamespaceLabel, ScriptablesStyles.WindowToggleStyle);

            if (hasNamespace)
            {
                string namespaceValue = DrawLabelledTextField(NamespaceFieldLabel, assetNamespace);
                if (namespaceValue != assetNamespace)
                {
                    namespaceManuallyEdited = true;
                }
                assetNamespace = namespaceValue;
            }
        }
        
        private void DrawMenuItemFields()
        {
            hasMenuItem = GUILayout.Toggle(hasMenuItem, CreateMenuItemLabel, ScriptablesStyles.WindowToggleStyle);

            if (hasMenuItem)
            {
                string fileNameValue = DrawLabelledTextField(FileNameFieldLabel, fileName);

                if (fileNameValue != fileName)
                {
                    fileNameManuallyEdited = true;
                }
                fileName = fileNameValue;

                string menuPathValue = DrawLabelledTextField(MenuPathFieldLabel, menuItem);
                
                if (menuPathValue != menuItem)
                {
                    menuPathManuallyEdited = true;
                }
                menuItem = menuPathValue;
            }
        }

        private void DrawDataConfigField(WizardScriptablesCategory category)
        {
            EditorGUILayout.BeginHorizontal();

            int previousSubType = dataSubType;

            // Get the rect for the popup
            Rect popupRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight, GUILayout.Width(DropdownWidhtSize));

            // Draw the popup
            dataSubType = EditorGUI.Popup(
                popupRect,
                dataSubType,
                Enum.GetNames(typeof(ClassScriptSubType)),
                EditorStyles.popup
            );

            // Now overlay a label with the tooltip – only if mouse is over the rect
            if (popupRect.Contains(Event.current.mousePosition))
            {
                // Set the tooltip for the current GUI element
                GUI.Label(popupRect, GUIContent.none, GUIStyle.none);
                // The tooltip will be picked up by the editor's tooltip system
                // We need to use a dummy GUIContent with the tooltip
                DropdownTooltipContent.tooltip = DropdownTooltip;
                EditorGUI.LabelField(popupRect, DropdownTooltipContent);
            }

            if (dataSubType != previousSubType)
            {
                dataFieldManuallyEdited = false;
                UpdateDataFieldFromAssetName();
            }

            GUILayout.Space(DropdownSpacing);

            string previousValue = dataFieldValue;

            dataFieldValue = EditorGUILayout.TextField(dataFieldValue);

            if (dataFieldValue != previousValue)
            {
                dataFieldManuallyEdited = true;
            }

            EditorGUILayout.EndHorizontal();
        }

        private string GetDefaultDataFieldValue()
        {
            return dataSubType switch
            {
                0 => DefaultDataFieldTypeValue,
                3 => DefaultDataFieldEnumValue,
                _ => DefaultDataFieldInnerValue
            };
        }

        private void UpdateDataFieldFromAssetName()
        {
            if (dataFieldManuallyEdited)
            {
                return;
            }

            WizardScriptablesCategory? category = CurrentCategory;
            if (!category.HasValue)
            {
                return;
            }

            string suffix = category.Value.ToString();
            string prefix = assetName.EndsWith(suffix)
                ? assetName.Substring(0, assetName.Length - suffix.Length)
                : assetName;

            if (string.IsNullOrEmpty(prefix))
            {
                prefix = "New";
            }

            dataFieldValue = dataSubType switch
            {
                0 => DefaultDataFieldTypeValue,
                3 => prefix + "State",
                _ => prefix + "Data"
            };
        }

        private void DrawExistingItemConfig(WizardScriptablesCategory category)
        {
            EditorGUILayout.LabelField(CreateNewPrefix + (configuringItem?.Name ?? categoryDataProvider.GetCategoryTitle(category)), ScriptablesStyles.WindowBoldLabelStyle);
            
            GUILayout.Space(ConfigAfterTitleSpacing);

            assetName = DrawLabelledTextField(NameFieldLabel, assetName);
            
            GUILayout.Space(ConfigBetweenFieldsSpacing);
            
            DrawConfigFolder();
        }

        private string DrawLabelledTextField(string label, string currentValue)
        {
            EditorGUILayout.BeginHorizontal();
            DrawLabel(label);
            currentValue = EditorGUILayout.TextField(currentValue);
            EditorGUILayout.EndHorizontal();
            return currentValue;
        }

        private void DrawLabel(string label)
        {
            EditorGUILayout.LabelField(label, ScriptablesStyles.WindowConfigLabelStyle, GUILayout.Width(LabelWidth));
        }

        private void DrawConfigFolder()
        {
            EditorGUILayout.BeginHorizontal();
            DrawLabel(FolderFieldLabel);

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.TextField(targetFolder);
            }

            ButtonPalette.DrawButton(
                label: string.Empty,
                buttonIcon: ScriptablesStylesIcons.ButtonIcon.EditIcon,
                fixedWidth: BrowseButtonWidth,
                customHeight: BrowseButtonHeight,
                style: ScriptablesStylesColors.ButtonColorStyle.Alert,
                tooltip: BrowseButtonTooltip,
                action: () =>
                {
                    string absolute = EditorUtility.OpenFolderPanel(SelectFolderDialogTitle, Application.dataPath, string.Empty);

                    if (!string.IsNullOrEmpty(absolute))
                    {
                        if (absolute.StartsWith(Application.dataPath))
                        {
                            string relativePath = absolute.Substring(Application.dataPath.Length);
                            
                            targetFolder = AssetsFolderPath + relativePath;

                            if (!targetFolder.EndsWith(SlashString))
                            {
                                targetFolder += SlashString;
                            }
                        }
                        else
                        {
                            ShowDialog(InvalidFolderDialogTitle, InvalidFolderDialogMessage);
                        }
                    }
                }
            );
            
            EditorGUILayout.EndHorizontal();
        }

        private bool CanFinish()
        {
            WizardScriptablesCategory? currentCategory = CurrentCategory;

            return !string.IsNullOrEmpty(assetName) &&
                targetFolder.StartsWith(AssetsFolderPrefix) &&
                (!hasNamespace || !string.IsNullOrEmpty(assetNamespace)) &&
                (!hasMenuItem || !string.IsNullOrEmpty(menuItem)) &&
                (!hasMenuItem || !string.IsNullOrEmpty(fileName)) &&
                (!isNewItemConfiguration || IsNewItemDataComplete(currentCategory));
        }

        private bool IsNewItemDataComplete(WizardScriptablesCategory? currentCategory)
        {
            if (!currentCategory.HasValue)
            {
                return false;
            }

            WizardScriptablesCategory cat = currentCategory.Value;

            if (cat == WizardScriptablesCategory.Reactive ||
                cat == WizardScriptablesCategory.Settings ||
                cat == WizardScriptablesCategory.Runtime)
            {
                return !string.IsNullOrEmpty(dataFieldValue);
            }

            return true;
        }

        private void ExecuteCreation()
        {
            isCreating = true;
            try
            {
                if (isNewItemConfiguration)
                {
                    ExecuteNewItemCreation();
                }
                else
                {
                    ExecuteExistingItemCreation();
                }
            }
            catch (Exception ex)
            {
                ShowDialog(ErrorDialogTitle, $"{ErrorDialogMessage}{ex.Message}");
            }
            finally
            {
                isCreating = false;
                EditorUtility.ClearProgressBar();
            }
        }

        private void ExecuteNewItemCreation()
        {
            WizardScriptablesCategory? category = CurrentCategory;

            string filePath = Path.Combine(targetFolder, assetName + ScriptExtension);

            if (File.Exists(filePath))
            {
                ShowDialog(FileExistsTitle,
                    string.Format(FileExistsMessageFormat, assetName, ScriptExtension));
                return;
            }

            string content = WizardScriptCreationProvider.GenerateTemplate(
                category.Value, assetName, dataFieldValue, (WizardScriptCreationProvider.DataSubType)dataSubType);

            if (hasMenuItem && !string.IsNullOrEmpty(menuItem))
            {
                content = WizardScriptCreationProvider.ApplyMenuAttribute(
                    content, assetName, menuItem, fileName);
            }
            else
            {
                content = WizardScriptCreationProvider.RemoveMenuAttribute(content);
            }

            if (hasNamespace && !string.IsNullOrEmpty(assetNamespace))
            {
                content = WizardScriptCreationProvider.WrapInNamespace(content, assetNamespace);
            }

            ScriptFileManager.Create(content, filePath);

            EditorApplication.delayCall += delegate { OnDelayCall(configuringItem); };
        }

        private void ExecuteExistingItemCreation()
        {
            WizardItemData item = configuringItem;

            if (item == null || item.ScriptType == null || ScriptFileNotFound(item))
            {
                ShowDialog(
                    ScriptNotFoundDialogTitle,
                    string.Format(ScriptNotFoundDialogMessage, item?.Name ?? UnknownName),
                    GoBackAndRefresh);
                return;
            }

            string assetPath = Path.Combine(targetFolder, assetName + ScriptableObjectAssetExtension);

            if (File.Exists(assetPath))
            {
                ShowDialog(FileExistsTitle,
                    string.Format(FileExistsMessageFormat, assetName, ScriptableObjectAssetExtension));
                return;
            }

            CreateAssetFromType(item.ScriptType);
            EditorApplication.delayCall += delegate { OnDelayCall(item); };
        }

        private void CreateAssetFromType(Type type)
        {
            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
            }

            ScriptableObject instance = ScriptableObject.CreateInstance(type);
            string path = Path.Combine(targetFolder, assetName + ScriptableObjectAssetExtension);
            AssetDatabase.CreateAsset(instance, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void OnDelayCall(WizardItemData item)
        {
            string extension = (item != null && !item.IsPinned && item.ScriptType != null) ? ScriptableObjectAssetExtension : ScriptExtension;
            string fullAssetPath = Path.Combine(targetFolder, assetName + extension);

            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(fullAssetPath);

            if (obj != null)
            {
                Selection.activeObject = obj;
                EditorGUIUtility.PingObject(obj);

                if (item != null && item.IsPinned)
                {
                    AssetDatabase.OpenAsset(obj);
                }
            }
            Close();
        }

        private void BackToSelectionAction()
        {
            GUI.FocusControl(null);
            isConfiguring = false;
        }

        private void GoBackAndRefresh()
        {
            isConfiguring = false;
            PerformRefresh();
        }

        private bool ScriptFileNotFound(WizardItemData item)
        {
            if (string.IsNullOrEmpty(item.AssetPath))
            {
                return false;
            }

            string fullPath = Path.Combine(
                Path.GetDirectoryName(Application.dataPath),
                item.AssetPath);

            return !File.Exists(fullPath);
        }

        private void ExecuteCreationAction()
        {
            GUI.FocusControl(null);
            ExecuteCreation();
        }

        private void CancelCreationAction()
        {
            GUI.FocusControl(null);
            isConfiguring = false;
            Close();
        }

        private struct ButtonParams
        {
            public string text;
            public bool enabled;
            public Action action;
        }

        private void DrawBottomActionButtons(ButtonParams left, ButtonParams middle, ButtonParams right)
        {
            EditorGUILayout.BeginVertical(ScriptablesStyles.WindowBottomAreaStyle, GUILayout.Height(BottomBarHeight), GUILayout.ExpandHeight(false));
            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            using (new EditorGUI.DisabledScope(!left.enabled))
            {
                ButtonPalette.DrawButton(
                    label: left.text,
                    fixedWidth: ButtonWidth,
                    customHeight: ButtonHeight,
                    style: ScriptablesStylesColors.ButtonColorStyle.Neutral,
                    action: left.action
                );
            }

            using (new EditorGUI.DisabledScope(!middle.enabled))
            {
                ButtonPalette.DrawButton(
                    label: middle.text,
                    fixedWidth: ButtonWidth,
                    customHeight: ButtonHeight,
                    style: ScriptablesStylesColors.ButtonColorStyle.Growth,
                    action: middle.action
                );
            }

            using (new EditorGUI.DisabledScope(!right.enabled))
            {
                ButtonPalette.DrawButton(
                    label: right.text,
                    fixedWidth: ButtonWidth,
                    customHeight: ButtonHeight,
                    buttonIcon: ScriptablesStylesIcons.ButtonIcon.ClearIcon,
                    style: ScriptablesStylesColors.ButtonColorStyle.Urgent,
                    action: right.action
                );
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(BottomBarPadding);
            EditorGUILayout.EndVertical();
        }

        private void DrawConfigurationBottomBar()
        {
            DrawBottomActionButtons(
                new ButtonParams
                {
                    text = PreviousButtonText,
                    enabled = true,
                    action = BackToSelectionAction
                },
                new ButtonParams
                {
                    text = FinishButtonText,
                    enabled = CanFinish() && !isCreating,
                    action = ExecuteCreationAction
                },
                new ButtonParams
                {
                    text = CancelButtonText,
                    enabled = true,
                    action = CancelCreationAction
                }
            );
        }


        private static string GetSelectedFolder()
        {
            foreach (UnityEngine.Object obj in Selection.GetFiltered<UnityEngine.Object>(SelectionMode.Assets))
            {
                string path = AssetDatabase.GetAssetPath(obj);

                if (Directory.Exists(path))
                {
                    return NormalizeFolderPath(path);
                }
                return NormalizeFolderPath(Path.GetDirectoryName(path));
            }
            return NormalizeFolderPath(AssetsFolderPrefix);
        }

        private static string NormalizeFolderPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return AssetsFolderPrefix;
            }
            if (!path.EndsWith(SlashString))
            {
                return path + SlashString;
            }
            return path;
        }

        private WizardScriptablesCategory? CurrentCategory
        {
            get
            {
                if (selectedCategoryIndex >= 0 && selectedCategoryIndex < categories.Count)
                {
                    return categories[selectedCategoryIndex];
                }
                return null;
            }
        }

        private void SelectCategory(int index)
        {
            selectedCategoryIndex = index;
            selectedItemIndex = -1;
            searchFilter = string.Empty;
            previousSearchFilter = string.Empty;
            RefreshItemsForCurrentCategory();
        }

        private void RefreshItemsForCurrentCategory()
        {
            if (!TryGetCurrentCategory(out WizardScriptablesCategory cat))
            {
                return;
            }

            currentItems.Clear();

            if (itemsCache.TryGetValue(cat, out List<WizardItemData> cached))
            {
                BuildCurrentItems(cat, cached);
            }
            else if (!loadingCategories.Contains(cat))
            {
                StartAsyncLoad(cat);
            }

            RefreshDisplayItems();
        }

        private bool TryGetCurrentCategory(out WizardScriptablesCategory category)
        {
            WizardScriptablesCategory? current = CurrentCategory;

            if (current.HasValue)
            {
                category = current.Value;
                return true;
            }

            category = default;
            return false;
        }

        private void BuildCurrentItems(WizardScriptablesCategory cat, List<WizardItemData> items)
        {
            TryAddNewItemPinned(cat);
            currentItems.AddRange(items);
        }

        private void TryAddNewItemPinned(WizardScriptablesCategory cat)
        {
            if (categoryDataProvider.HasNewItem(cat) && !string.IsNullOrEmpty(categoryDataProvider.GetCategoryNewItemName(cat)))
            {
                currentItems.Add(CreateNewPinnedItem());
            }
        }

        private static WizardItemData CreateNewPinnedItem()
        {
            return new WizardItemData
            {
                Name = NewItemName,
                Icon = ScriptablesStylesIcons.GetIconTexture(ScriptablesStylesIcons.IconType.CreateAddNew),
                IsPinned = true,
                ScriptType = null,
                AssetPath = null
            };
        }

        private void StartAsyncLoad(WizardScriptablesCategory cat)
        {
            loadingCategories.Add(cat);

            if (categoryDataProvider.GetItemProvider(cat) != null)
            {
                EditorApplication.delayCall += () => CompleteAsyncLoad(cat);
            }
            else
            {
                loadingCategories.Remove(cat);
            }
        }

        private void CompleteAsyncLoad(WizardScriptablesCategory cat)
        {
            if (CurrentCategory != cat)
            {
                loadingCategories.Remove(cat);
                return;
            }

            try
            {
                Func<List<WizardItemData>> provider = categoryDataProvider.GetItemProvider(cat);
                List<WizardItemData> provided = provider();
                itemsCache[cat] = provided ?? new List<WizardItemData>();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"ScriptablesWizardWindow.LoadItemsForCategory encountered an error: {e.Message}");
                itemsCache[cat] = new List<WizardItemData>();
            }

            loadingCategories.Remove(cat);

            if (CurrentCategory == cat)
            {
                currentItems.Clear();
                BuildCurrentItems(cat, itemsCache[cat]);
                RefreshDisplayItems();
                Repaint();
            }
        }

        private void RefreshDisplayItems()
        {
            WizardScriptablesCategory? category = CurrentCategory;

            if (!category.HasValue)
            {
                return;
            }

            WizardScriptablesCategory cat = category.Value;

            List<WizardItemData> filtered = currentItems.FindAll(MatchesFilter);

            List<WizardItemData> pinned = filtered.FindAll(IsItemPinned);
            List<WizardItemData> nonPinned = filtered.FindAll(IsItemNotPinned);

            if (cat == WizardScriptablesCategory.Custom)
            {
                customPagination.SetItems(nonPinned, resetToFirstPage: searchFilter != previousSearchFilter);
                displayItems = customPagination.CurrentPageItems;
            }
            else if (cat == WizardScriptablesCategory.ScriptableObject)
            {
                scriptablePagination.SetItems(nonPinned, resetToFirstPage: searchFilter != previousSearchFilter);
                displayItems = new List<WizardItemData>(pinned);
                displayItems.AddRange(scriptablePagination.CurrentPageItems);
            }
            else
            {
                displayItems = new List<WizardItemData>(pinned);
                displayItems.AddRange(nonPinned);
            }

            if (selectedItemIndex >= displayItems.Count)
            {
                selectedItemIndex = displayItems.Count - 1;
            }

            previousSearchFilter = searchFilter;
        }

    }
}
#endif
