#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Thisaislan.Scriptables.Editor.Utilities
{
    /// <summary>
    /// Stores information about a scanned script, including its name, type, and asset path.
    /// </summary>
    internal struct ScriptScanInfo
    {
        /// <summary>
        /// The name of the script class.
        /// </summary>
        internal string Name;

        /// <summary>
        /// The <see cref="Type"/> of the script class.
        /// </summary>
        internal Type ScriptType;

        /// <summary>
        /// The project-relative asset path of the script file.
        /// </summary>
        internal string AssetPath;

        /// <summary>
        /// Initializes a new instance of <see cref="ScriptScanInfo"/>.
        /// </summary>
        /// <param name="name">The name of the script class</param>
        /// <param name="scriptType">The <see cref="Type"/> of the script class</param>
        /// <param name="assetPath">The project-relative path to the script file</param>
        internal ScriptScanInfo(string name, Type scriptType, string assetPath)
        {
            Name = name;
            ScriptType = scriptType;
            AssetPath = assetPath;
        }
    }

    /// <summary>
    /// Provides asynchronous scanning and loading of <see cref="ScriptableObject"/> assets
    /// using editor filter strings, with progress reporting and cancellation support.
    /// </summary>
    internal class AssetScanner
    {
        private const string ScanPathStart = "Assets/";
        private const string ScanStartMessage = "Found {0} assets. Loading...";
        private const string ScanLoadingMessage = "Loading {0}/{1}...";
        private const string ScanFileExtension = ".asset";
        private const string ScriptFilter = "t:Script";

        private Queue<string> pendingPaths;
        private int totalPaths;
        private List<Object> results;
        private string[] filters;
        private bool isScanning;
        private event Action<float, string> onProgress;
        private event Action<List<Object>> onComplete;

        /// <summary>
        /// Gets the current scan progress as a value between 0 and 1.
        /// </summary>
        internal float Progress { get; private set; }

        /// <summary>
        /// Gets the current status message describing the scan's ongoing operation.
        /// </summary>
        internal string Status { get; private set; }

        /// <summary>
        /// Gets a value indicating whether a scan is currently in progress.
        /// </summary>
        internal bool IsScanning => isScanning;

        /// <summary>
        /// Starts an asynchronous scan with the specified filters and callbacks.
        /// </summary>
        /// <param name="filters">Array of filter strings for assets to include</param>
        /// <param name="onProgress">Callback invoked with progress (0-1) and a status message</param>
        /// <param name="onComplete">Callback invoked with the list of found assets when scanning finishes</param>
        internal void Start(string[] filters, Action<float, string> onProgress, Action<List<Object>> onComplete)
        {
            Start(filters, null, onProgress, onComplete);
        }

        /// <summary>
        /// Starts an asynchronous scan with include and exclude filters and callbacks.
        /// </summary>
        /// <param name="filters">Array of filter strings for assets to include</param>
        /// <param name="excludeFilters">Optional array of filter strings for assets to exclude</param>
        /// <param name="onProgress">Callback invoked with progress (0-1) and a status message</param>
        /// <param name="onComplete">Callback invoked with the list of found assets when scanning finishes</param>
        internal void Start(string[] filters, string[] excludeFilters, Action<float, string> onProgress, Action<List<Object>> onComplete)
        {
            if (isScanning) return;
            isScanning = true;

            this.onProgress = onProgress;
            this.onComplete = onComplete;
            this.filters = filters;

            StartScan(excludeFilters);
        }

        /// <summary>
        /// Cancels the ongoing scan and invokes the completion callback with null.
        /// </summary>
        internal void Cancel()
        {
            Stop();
            onComplete?.Invoke(null);
        }

        /// <summary>
        /// Scans a project folder for non-abstract, non-generic MonoScript classes.
        /// </summary>
        /// <param name="folderPath">The project-relative folder path to scan</param>
        /// <returns>A sorted list of <see cref="ScriptScanInfo"/> for each valid script found</returns>
        internal static List<ScriptScanInfo> ScanScriptsInFolder(string folderPath)
        {
            List<ScriptScanInfo> results = new List<ScriptScanInfo>();

            try
            {
                string[] guids = AssetDatabase.FindAssets(ScriptFilter, new[] { folderPath });

                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    MonoScript monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(path);

                    if (monoScript != null)
                    {
                        Type type = monoScript.GetClass();

                        if (type != null && !type.IsAbstract &&
                            !type.IsGenericTypeDefinition)
                        {
                            results.Add(new ScriptScanInfo(type.Name, type, path));
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"AssetScanner.ScanScriptsInFolder encountered an error: {e.Message}");
            }

            results.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));
            return results;
        }

        /// <summary>
        /// Scans all non-abstract types derived from <paramref name="baseType"/>,
        /// excluding library types whose namespace starts with any of the given prefixes.
        /// </summary>
        /// <param name="baseType">The base type to search for derived types</param>
        /// <param name="libraryNamespacePrefixes">Namespace prefixes that identify library (non-project) types to exclude</param>
        /// <returns>A sorted list of <see cref="ScriptScanInfo"/> for each matching derived type</returns>
        internal static List<ScriptScanInfo> ScanDerivedTypes(
            Type baseType, string[] libraryNamespacePrefixes)
        {
            List<ScriptScanInfo> results = new List<ScriptScanInfo>();

            try
            {
                TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom(baseType);

                foreach (Type type in types)
                {
                    if (!type.IsAbstract && !type.IsGenericTypeDefinition &&
                        !IsLibraryType(type, libraryNamespacePrefixes))
                    {
                        string path = FindScriptPath(type);
                        results.Add(new ScriptScanInfo(type.Name, type, path));
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"AssetScanner.ScanDerivedTypes encountered an error: {e.Message}");
            }

            results.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));
            return results;
        }

        /// <summary>
        /// Scans all <see cref="ScriptableObject"/> derived types that are not debuggable
        /// (i.e., not derived from <paramref name="excludeBaseType"/>) and are not in a library namespace
        /// or the excluded package root.
        /// </summary>
        /// <param name="excludeBaseType">Types derived from this base are excluded from results</param>
        /// <param name="libraryNamespacePrefixes">Namespace prefixes identifying library types to exclude</param>
        /// <param name="excludedPackageRoot">Package root path; assets under this path are excluded</param>
        /// <returns>A sorted list of <see cref="ScriptScanInfo"/> for each matching scriptable object type</returns>
        internal static List<ScriptScanInfo> ScanNonDebuggableScriptableObjects(
            Type excludeBaseType, string[] libraryNamespacePrefixes, string excludedPackageRoot)
        {
            List<ScriptScanInfo> results = new List<ScriptScanInfo>();

            try
            {
                TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom(typeof(ScriptableObject));

                foreach (Type type in types)
                {
                    if (type == null || type.IsAbstract ||
                        type.IsGenericTypeDefinition ||
                        type == typeof(ScriptableObject) ||
                        excludeBaseType.IsAssignableFrom(type))
                    {
                        continue;
                    }

                    string path = FindScriptPath(type);

                    if (string.IsNullOrEmpty(path) ||
                        !path.StartsWith("Assets") ||
                        path.StartsWith(excludedPackageRoot))
                    {
                        continue;
                    }

                    results.Add(new ScriptScanInfo(type.Name, type, path));
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"AssetScanner.ScanNonDebuggableScriptableObjects encountered an error: {e.Message}");
            }

            results.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));
            return results;
        }

        /// <summary>
        /// Finds the project-relative asset path of the script file that defines the given type.
        /// </summary>
        /// <param name="type">The type whose script path to locate</param>
        /// <returns>The asset path of the script, or null if not found</returns>
        internal static string FindScriptPath(Type type)
        {
            string[] guids = AssetDatabase.FindAssets($"{ScriptFilter} {type.Name}");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);

                if (script != null && script.GetClass() == type)
                {
                    return path;
                }
            }

            return null;
        }

        /// <summary>
        /// Determines whether the given type belongs to a library namespace.
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <param name="libraryNamespacePrefixes">Namespace prefixes considered as library roots</param>
        /// <returns>True if the type's namespace starts with any of the given prefixes</returns>
        internal static bool IsLibraryType(Type type, string[] libraryNamespacePrefixes)
        {
            if (type.Namespace == null || libraryNamespacePrefixes == null)
            {
                return false;
            }

            foreach (string prefix in libraryNamespacePrefixes)
            {
                if (type.Namespace.StartsWith(prefix))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Resolves the project root folder of a script by locating its file and
        /// traversing up five directory levels.
        /// </summary>
        /// <param name="scriptName">The name of the script class to locate</param>
        /// <returns>The root folder path, or an empty string if the script could not be found</returns>
        internal static string ResolveScriptRoot(string scriptName)
        {
            string[] guids = AssetDatabase.FindAssets($"{ScriptFilter} {scriptName}");

            if (guids.Length > 0)
            {
                string scriptPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                return Path.GetDirectoryName(
                    Path.GetDirectoryName(
                        Path.GetDirectoryName(
                            Path.GetDirectoryName(
                                Path.GetDirectoryName(scriptPath)))));
            }
            return string.Empty;
        }



        private void StartScan(string[] excludeFilters = null)
        {
            pendingPaths = new Queue<string>();
            results = new List<Object>();

            List<string> paths = new List<string>();
            foreach (string filter in filters)
            {
                foreach (string guid in AssetDatabase.FindAssets(filter))
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    if (!string.IsNullOrEmpty(path) && path.StartsWith(ScanPathStart) && ShouldIncludeAsset(path))
                    {
                        paths.Add(path);
                    }
                }
            }

            paths = paths.Distinct().ToList();

            if (excludeFilters != null)
            {
                HashSet<string> excludePaths = new HashSet<string>();
                foreach (string filter in excludeFilters)
                {
                    foreach (string guid in AssetDatabase.FindAssets(filter))
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guid);
                        if (!string.IsNullOrEmpty(path))
                        {
                            excludePaths.Add(path);
                        }
                    }
                }
                if (excludePaths.Count > 0)
                {
                    paths.RemoveAll(p => excludePaths.Contains(p));
                }
            }

            totalPaths = paths.Count;
            foreach (string path in paths) pendingPaths.Enqueue(path);

            Progress = 0f;
            Status = string.Format(ScanStartMessage, totalPaths);
            onProgress?.Invoke(Progress, Status);

            EditorApplication.update += ProcessBatch;
        }

        private void ProcessBatch()
        {
            if (!isScanning) return;

            const int batchSize = 10;
            int processed = 0;
            while (pendingPaths.Count > 0 && processed < batchSize)
            {
                string path = pendingPaths.Dequeue();
                Object asset = AssetDatabase.LoadAssetAtPath<Object>(path);
                if (asset != null && asset is ScriptableObject && !results.Contains(asset))
                    results.Add(asset);
                processed++;
            }

            int loadedCount = totalPaths - pendingPaths.Count;
            Progress = totalPaths == 0 ? 1f : (float)loadedCount / totalPaths;
            Status = string.Format(ScanLoadingMessage, loadedCount, totalPaths);
            onProgress?.Invoke(Progress, Status);

            if (pendingPaths.Count == 0)
            {
                Stop();
                onComplete?.Invoke(results);
            }
        }

        private void Stop()
        {
            if (!isScanning) return;
            isScanning = false;
            EditorApplication.update -= ProcessBatch;
        }

        private bool ShouldIncludeAsset(string assetPath)
        {
            return Path.GetExtension(assetPath).ToLower().Equals(ScanFileExtension);
        }
    }
}
#endif
