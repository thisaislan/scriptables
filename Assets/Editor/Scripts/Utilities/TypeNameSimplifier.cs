#if UNITY_EDITOR
using System.Collections.Generic;

namespace Thisaislan.Scriptables.Editor
{
    /// <summary>
    /// Utility class for simplifying and converting type names to more readable formats.
    /// Primarily used in editor UI to display type names in a more user-friendly manner.
    /// </summary>
    /// <remarks>
    /// This class provides functionality to:
    /// - Convert system type names (e.g., "Int32") to language keywords (e.g., "int")
    /// - Handle generic type names by extracting and simplifying the base type
    /// - Provide consistent type name formatting throughout the editor UI
    /// </remarks>
    internal static class TypeNameSimplifier
    {
        /// <summary>
        /// Dictionary mapping system type names to their simplified language keyword equivalents
        /// </summary>
        /// <remarks>
        /// This mapping converts .NET framework type names to their C# language keyword equivalents
        /// for better readability in the editor interface.
        /// </remarks>
        private static readonly Dictionary<string, string> typeNameMap = new Dictionary<string, string>
        {
            { "Int32", "int" },      // System.Int32 → int
            { "Single", "float" },    // System.Single → float
            { "Double", "double" },   // System.Double → double
            { "Boolean", "bool" },    // System.Boolean → bool
            { "String", "string" },   // System.String → string
            { "Char", "char" },       // System.Char → char
            { "Byte", "byte" },       // System.Byte → byte
            { "SByte", "sbyte" },     // System.SByte → sbyte
            { "Int16", "short" },     // System.Int16 → short
            { "Int64", "long" },      // System.Int64 → long
            { "UInt16", "ushort" },   // System.UInt16 → ushort
            { "UInt32", "uint" },     // System.UInt32 → uint
            { "UInt64", "ulong" },    // System.UInt64 → ulong
            { "Decimal", "decimal" }  // System.Decimal → decimal
        };

        /// <summary>
        /// Simplifies a type name by converting system type names to language keywords
        /// and handling generic type names appropriately.
        /// </summary>
        /// <param name="originalName">The original type name to simplify</param>
        /// <returns>
        /// A simplified version of the type name:
        /// - System type names are converted to language keywords
        /// - Generic types have their base name simplified (without generic parameters)
        /// - Other type names are returned unchanged
        /// </returns>
        /// <remarks>
        /// Examples:
        /// - "Int32" → "int"
        /// - "List`1" → "List" (base name only, generic parameters are removed)
        /// - "CustomType" → "CustomType" (unchanged)
        /// </remarks>
        internal static string SimplifyTypeName(string originalName)
        {
            // Check if we have a simplified name for this type in our mapping
            if (typeNameMap.TryGetValue(originalName, out string simplifiedName))
            {
                return simplifiedName;
            }

            // Handle generic types (indicated by backtick in the name, e.g., List`1, Dictionary`2)
            if (originalName.Contains("`"))
            {
                // Find the position of the backtick which indicates generic type parameters
                int backtickIndex = originalName.IndexOf('`');

                // Extract the base name (everything before the backtick)
                string baseName = originalName.Substring(0, backtickIndex);

                // Try to simplify the base name if it's a system type
                if (typeNameMap.TryGetValue(baseName, out string simplifiedBaseName))
                {
                    return simplifiedBaseName;
                }

                // Return the base name without generic parameters for non-system types
                return baseName;
            }

            // Return the original name if no simplification is available or needed
            return originalName;
        }

        /// <summary>
        /// Capitalizes the first letter of a string
        /// </summary>
        internal static string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return char.ToUpper(input[0]) + input.Substring(1);
        }
        
        /// <summary>
        /// Clean the type name if needed
        /// </summary>
        internal static string CleanTypeName(string input)
        {
            return input.Replace("PPtr<$", string.Empty).Replace(">", string.Empty);
        }

    }
}
#endif