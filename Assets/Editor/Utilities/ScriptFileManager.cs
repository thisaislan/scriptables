#if UNITY_EDITOR
using System.IO;
using UnityEditor;

namespace Thisaislan.Scriptables.Editor.Utilities
{
    /// <summary>
    /// Provides utility methods for creating, deleting, and renaming script asset files,
    /// with automatic <see cref="AssetDatabase.Refresh"/> after each operation.
    /// </summary>
    internal static class ScriptFileManager
    {
        /// <summary>
        /// Creates a new script file at the given path, creating parent directories if needed.
        /// </summary>
        /// <param name="content">The full text content to write to the file</param>
        /// <param name="filePath">The project-relative file path for the new script</param>
        /// <returns>A tuple indicating success and an optional error message</returns>
        internal static (bool success, string errorMessage) Create(string content, string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(filePath, content);
            AssetDatabase.Refresh();

            return (true, null);
        }

        /// <summary>
        /// Deletes the asset at the specified project-relative path.
        /// </summary>
        /// <param name="assetPath">The project-relative path of the asset to delete</param>
        /// <returns>A tuple indicating success and an optional error message</returns>
        internal static (bool success, string errorMessage) Delete(string assetPath)
        {
            AssetDatabase.DeleteAsset(assetPath);
            AssetDatabase.Refresh();
            return (true, null);
        }

        /// <summary>
        /// Renames the asset at the specified project-relative path to a new name.
        /// </summary>
        /// <param name="assetPath">The project-relative path of the asset to rename</param>
        /// <param name="newName">The new name for the asset (without extension)</param>
        /// <returns>A tuple indicating success and an optional error message</returns>
        internal static (bool success, string errorMessage) Rename(string assetPath, string newName)
        {
            AssetDatabase.RenameAsset(assetPath, newName);
            AssetDatabase.Refresh();
            return (true, null);
        }
    }
}
#endif
