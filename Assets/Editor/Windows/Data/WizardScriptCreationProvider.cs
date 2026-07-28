#if UNITY_EDITOR
using Thisaislan.Scriptables.Editor.Windows.Data.Enums;

namespace Thisaislan.Scriptables.Editor.Windows.Data
{
    /// <summary>
    /// Provides script template generation and manipulation for the Scriptables wizard.
    /// Composes C# source content from templates, wraps in namespaces, and manages
    /// <see cref="CreateAssetMenuAttribute"/> annotations.
    /// </summary>
    internal static class WizardScriptCreationProvider
    {
        internal enum DataSubType { Type, Class, Struct, Enum }

        private const string ReactiveTemplate = @"using UnityEngine;
using Thisaislan.Scriptables.Abstracts;

public class {0} : ScriptableReactive<{1}>
{{
    // Add properties, methods, or event handlers for reactive value changes.
}}
";

        private const string ReactiveWithInnerClassTemplate = @"using UnityEngine;
using Thisaislan.Scriptables.Abstracts;
using System;

public class {0} : ScriptableReactive<{0}.{1}>
{{
    [Serializable]
    public {2} {1}
    {{
        // Add properties, methods, or event handlers for reactive value changes.
    }}
}}
";

        private const string SettingsTemplate = @"using UnityEngine;
using Thisaislan.Scriptables.Abstracts;
using System;

public class {0} : ScriptableSettings<{0}.{1}>
{{
    [Serializable]
    public {2} {1}
    {{
        // Define serializable configuration fields for persistent game settings.
    }}
}}
";

        private const string RuntimeTemplate = @"using UnityEngine;
using Thisaislan.Scriptables.Abstracts;
using System;

public class {0} : ScriptableRuntime<{0}.{1}>
{{
    [Serializable]
    public {2} {1}
    {{
        // Declare runtime data fields that change during gameplay.
    }}
}}
";

        private const string ScriptableObjectTemplate = @"using UnityEngine;
using System;

public class {0} : ScriptableObject
{{
    // Add serialized fields, methods, and custom logic for your data container.
}}
";

        private const string SettingsTypeTemplate = @"using UnityEngine;
using Thisaislan.Scriptables.Abstracts;

public class {0} : ScriptableSettings<{1}>
{{
    // Define serializable configuration fields for persistent game settings.
}}
";

        private const string RuntimeTypeTemplate = @"using UnityEngine;
using Thisaislan.Scriptables.Abstracts;

public class {0} : ScriptableRuntime<{1}>
{{
    // Declare runtime data fields that change during gameplay.
}}
";

        private const string ReactiveWithEnumTemplate = @"using UnityEngine;
using Thisaislan.Scriptables.Abstracts;

public class {0} : ScriptableReactive<{0}.{1}>
{{
    public enum {1}
    {{
        // Define enum values.
    }}
}}
";

        private const string SettingsWithEnumTemplate = @"using UnityEngine;
using Thisaislan.Scriptables.Abstracts;

public class {0} : ScriptableSettings<{0}.{1}>
{{
    public enum {1}
    {{
        // Define enum values for persistent game settings.
    }}
}}
";

        private const string RuntimeWithEnumTemplate = @"using UnityEngine;
using Thisaislan.Scriptables.Abstracts;

public class {0} : ScriptableRuntime<{0}.{1}>
{{
    public enum {1}
    {{
        // Define enum values that change during gameplay.
    }}
}}
";

        /// <summary>
        /// Generates the full source code template for the given wizard category and data sub-type,
        /// substituting <paramref name="assetName"/> and <paramref name="dataFieldValue"/> into the
        /// appropriate template. For <see cref="DataSubType.Class"/> and <see cref="DataSubType.Struct"/>,
        /// the keyword is resolved via <see cref="GetKeyword"/>.
        /// </summary>
        /// <param name="category">The wizard category determining which template to use</param>
        /// <param name="assetName">The name of the new script class</param>
        /// <param name="dataFieldValue">The value for the generic type or nested type name</param>
        /// <param name="dataSubType">Determines whether a nested type is generated and its kind</param>
        /// <returns>The generated source code as a string</returns>
        internal static string GenerateTemplate(
            WizardScriptablesCategory category,
            string assetName,
            string dataFieldValue,
            DataSubType dataSubType)
        {
            switch (category)
            {
                case WizardScriptablesCategory.Reactive:
                    switch (dataSubType)
                    {
                        case DataSubType.Type:
                            return string.Format(ReactiveTemplate, assetName, dataFieldValue);
                        case DataSubType.Class:
                        case DataSubType.Struct:
                            return string.Format(ReactiveWithInnerClassTemplate,
                                assetName, dataFieldValue, GetKeyword(dataSubType));
                        default:
                            return string.Format(ReactiveWithEnumTemplate, assetName, dataFieldValue);
                    }
                case WizardScriptablesCategory.Settings:
                    switch (dataSubType)
                    {
                        case DataSubType.Type:
                            return string.Format(SettingsTypeTemplate, assetName, dataFieldValue);
                        case DataSubType.Class:
                        case DataSubType.Struct:
                            return string.Format(SettingsTemplate,
                                assetName, dataFieldValue, GetKeyword(dataSubType));
                        default:
                            return string.Format(SettingsWithEnumTemplate, assetName, dataFieldValue);
                    }
                case WizardScriptablesCategory.Runtime:
                    switch (dataSubType)
                    {
                        case DataSubType.Type:
                            return string.Format(RuntimeTypeTemplate, assetName, dataFieldValue);
                        case DataSubType.Class:
                        case DataSubType.Struct:
                            return string.Format(RuntimeTemplate,
                                assetName, dataFieldValue, GetKeyword(dataSubType));
                        default:
                            return string.Format(RuntimeWithEnumTemplate, assetName, dataFieldValue);
                    }
                default:
                    return string.Format(ScriptableObjectTemplate, assetName);
            }
        }

        private static string GetKeyword(DataSubType dataSubType)
        {
            switch (dataSubType)
            {
                case DataSubType.Class: return "class";
                case DataSubType.Struct: return "struct";
                default: return string.Empty;
            }
        }

        /// <summary>
        /// Wraps the given source content inside a namespace block.
        /// Using directives and blank lines at the start of the file remain outside the namespace.
        /// </summary>
        /// <param name="content">The source code content to wrap</param>
        /// <param name="namespaceName">The namespace name, or null/empty to return content unchanged</param>
        /// <returns>The source code wrapped in the namespace, or the original content if no namespace is specified</returns>
        internal static string WrapInNamespace(string content, string namespaceName)
        {
            if (string.IsNullOrEmpty(namespaceName))
            {
                return content;
            }

            string[] lines = content.Split('\n');
            var beforeNamespace = new System.Collections.Generic.List<string>();
            var insideNamespace = new System.Collections.Generic.List<string>();
            bool isUsingBlock = true;

            foreach (string line in lines)
            {
                string trimmed = line.Trim();
                if (isUsingBlock && (trimmed.StartsWith("using ") || trimmed.Length == 0))
                {
                    beforeNamespace.Add(line);
                }
                else
                {
                    isUsingBlock = false;
                    insideNamespace.Add(line);
                }
            }

            for (int i = 0; i < insideNamespace.Count; i++)
            {
                if (!string.IsNullOrEmpty(insideNamespace[i]))
                {
                    insideNamespace[i] = "    " + insideNamespace[i];
                }
            }

            return string.Join("\n", beforeNamespace) + "\n" +
                   $"namespace {namespaceName}\n{{\n" +
                   string.Join("\n", insideNamespace) + "\n" +
                   "}";
        }

        /// <summary>
        /// Inserts or replaces a <see cref="CreateAssetMenuAttribute"/> in the source content.
        /// If an existing <c>[CreateAssetMenu(...)]</c> attribute is found, it is replaced.
        /// Otherwise, a new attribute line is inserted after the using directives.
        /// When <paramref name="menuItem"/> is null or empty, <see cref="RemoveMenuAttribute"/> is called instead.
        /// </summary>
        /// <param name="content">The source code to modify</param>
        /// <param name="assetName">The class name of the asset, used for <c>nameof</c> references</param>
        /// <param name="menuItem">The menu item path (e.g., "MyGame/Config"), or null/empty to remove the attribute</param>
        /// <param name="fileName">Optional override for the <c>fileName</c> parameter in the attribute</param>
        /// <returns>The modified source code with the <c>[CreateAssetMenu]</c> attribute applied</returns>
        internal static string ApplyMenuAttribute(
            string content, string assetName, string menuItem, string fileName)
        {
            if (string.IsNullOrEmpty(menuItem))
            {
                return RemoveMenuAttribute(content);
            }

            string fileNameValue = !string.IsNullOrEmpty(fileName) ? fileName : assetName;

            string fileNameParam = fileNameValue == assetName
                ? $"nameof({assetName})"
                : $"\"{fileNameValue}\"";

            // Ensure menuItem ends with "/" so the final path is <menuPath>/<className>
            if (!menuItem.EndsWith("/"))
            {
                menuItem += "/";
            }

            string menuNameParam = $"\"{menuItem}\" + nameof({assetName})";

            string menuAttrLine =
                $"[CreateAssetMenu(fileName = {fileNameParam}, menuName = {menuNameParam})]";

            string[] contentLines = content.Split('\n');
            bool replaced = false;

            for (int i = 0; i < contentLines.Length; i++)
            {
                if (contentLines[i].Contains("[CreateAssetMenu("))
                {
                    contentLines[i] = menuAttrLine;
                    replaced = true;
                    break;
                }
            }

            if (!replaced)
            {
                int insertAt = 0;
                
                for (int i = 0; i < contentLines.Length; i++)
                {
                    string trimmed = contentLines[i].Trim();
                    if (trimmed.StartsWith("using ") || trimmed.Length == 0)
                    {
                        insertAt = i + 1;
                    }
                    else
                    {
                        break;
                    }
                }

                var list = new System.Collections.Generic.List<string>(contentLines);
                list.Insert(insertAt, menuAttrLine);
                return string.Join("\n", list);
            }

            return string.Join("\n", contentLines);
        }

        /// <summary>
        /// Removes any <c>[CreateAssetMenu(...)]</c> attribute from the source content.
        /// </summary>
        /// <param name="content">The source code to process</param>
        /// <returns>The source code with all <c>[CreateAssetMenu]</c> lines removed</returns>
        internal static string RemoveMenuAttribute(string content)
        {
            string[] contentLines = content.Split('\n');
            var filtered = new System.Collections.Generic.List<string>();

            foreach (string line in contentLines)
            {
                if (!line.TrimStart().StartsWith("[CreateAssetMenu("))
                {
                    filtered.Add(line);
                }
            }

            return string.Join("\n", filtered);
        }
    }
}
#endif
