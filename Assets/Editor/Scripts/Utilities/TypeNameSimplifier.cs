#if UNITY_EDITOR
using System.Collections.Generic;

namespace Thisaislan.Scriptables.Editor
{
    /// <summary>
    /// Utility class for simplifying and converting type names to more readable formats.
    /// Primarily used in editor UI to display type names in a more user-friendly manner.
    /// </summary>
    internal static class TypeNameSimplifier
    {
        // String constants for type name patterns
        private const string Backtick = "`";
        private const string PPtrPattern = "PPtr<$";
        private const string AngleBracketClose = ">";
        private const string EmptyString = "";
        
        // System type name constants
        private const string TypeInt32 = "Int32";
        private const string TypeSingle = "Single";
        private const string TypeDouble = "Double";
        private const string TypeBoolean = "Boolean";
        private const string TypeString = "String";
        private const string TypeChar = "Char";
        private const string TypeByte = "Byte";
        private const string TypeSByte = "SByte";
        private const string TypeInt16 = "Int16";
        private const string TypeInt64 = "Int64";
        private const string TypeUInt16 = "UInt16";
        private const string TypeUInt32 = "UInt32";
        private const string TypeUInt64 = "UInt64";
        private const string TypeDecimal = "Decimal";
        
        // Simplified type name constants
        private const string SimplifiedInt = "int";
        private const string SimplifiedFloat = "float";
        private const string SimplifiedDouble = "double";
        private const string SimplifiedBool = "bool";
        private const string SimplifiedString = "string";
        private const string SimplifiedChar = "char";
        private const string SimplifiedByte = "byte";
        private const string SimplifiedSByte = "sbyte";
        private const string SimplifiedShort = "short";
        private const string SimplifiedLong = "long";
        private const string SimplifiedUShort = "ushort";
        private const string SimplifiedUInt = "uint";
        private const string SimplifiedULong = "ulong";
        private const string SimplifiedDecimal = "decimal";

        /// <summary>
        /// Dictionary mapping system type names to their simplified language keyword equivalents
        /// </summary>
        private static readonly Dictionary<string, string> typeNameMap = new Dictionary<string, string>
        {
            { TypeInt32, SimplifiedInt },
            { TypeSingle, SimplifiedFloat },
            { TypeDouble, SimplifiedDouble },
            { TypeBoolean, SimplifiedBool },
            { TypeString, SimplifiedString },
            { TypeChar, SimplifiedChar },
            { TypeByte, SimplifiedByte },
            { TypeSByte, SimplifiedSByte },
            { TypeInt16, SimplifiedShort },
            { TypeInt64, SimplifiedLong },
            { TypeUInt16, SimplifiedUShort },
            { TypeUInt32, SimplifiedUInt },
            { TypeUInt64, SimplifiedULong },
            { TypeDecimal, SimplifiedDecimal }
        };

        /// <summary>
        /// Simplifies a type name by converting system type names to language keywords
        /// and handling generic type names appropriately.
        /// </summary>
        internal static string SimplifyTypeName(string originalName)
        {
            // Check if we have a simplified name for this type in our mapping
            if (typeNameMap.TryGetValue(originalName, out string simplifiedName))
            {
                return simplifiedName;
            }

            // Handle generic types (indicated by backtick in the name, e.g., List`1, Dictionary`2)
            if (originalName.Contains(Backtick))
            {
                // Find the position of the backtick which indicates generic type parameters
                int backtickIndex = originalName.IndexOf(Backtick);

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
            return input.Replace(PPtrPattern, EmptyString).Replace(AngleBracketClose, EmptyString);
        }
    }
}
#endif