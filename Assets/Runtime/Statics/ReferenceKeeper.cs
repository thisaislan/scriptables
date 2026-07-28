using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using Thisaislan.Scriptables.Editor.MonoBehaviours;
#endif

namespace Thisaislan.Scriptables.Statics
{
    /// <summary>
    /// Prevents ScriptableObjects from being unloaded by memory management
    /// </summary>
    internal static class ReferenceKeeper
    {
        private static readonly HashSet<ScriptableObject> scriptableObjects = new HashSet<ScriptableObject>();

#if UNITY_EDITOR
        private static ScriptableCache scriptableCache;
#endif

        /// <summary>
        /// Adds a ScriptableObject to the reference list to keep it alive
        /// </summary>
        /// <param name="scriptableObject">The ScriptableObject to keep alive</param>
        internal static void Keep(ScriptableObject scriptableObject)
        {
            if (scriptableObject == null || !scriptableObjects.Add(scriptableObject))
            {
                return;
            }

#if UNITY_EDITOR
            if (scriptableCache == null)
            {
                InstantiateScriptableCache();
            }

            scriptableCache.UpdateScriptableList(new List<ScriptableObject>(scriptableObjects));
#endif
        }

        /// <summary>
        /// Removes a ScriptableObject from the reference list allowing it to be unloaded
        /// </summary>
        /// <param name="scriptableObject">The ScriptableObject to release</param>
        internal static void Unkeep(ScriptableObject scriptableObject)
        {
            if (scriptableObject == null || scriptableObjects.Count == 0)
            {
                return;
            }

            scriptableObjects.Remove(scriptableObject);

#if UNITY_EDITOR
            scriptableCache.UpdateScriptableList(new List<ScriptableObject>(scriptableObjects));
#endif
        }

#if UNITY_EDITOR

        /// <summary>
        /// Checks whether the specified ScriptableObject is currently being kept alive in memory
        /// </summary>
        /// <param name="scriptableObject">The ScriptableObject to check</param>
        /// <returns>True if the ScriptableObject is pinned; otherwise, false</returns>
        internal static bool IsKept(ScriptableObject scriptableObject)
        {
            return scriptableObjects.Contains(scriptableObject);
        }

        private static void InstantiateScriptableCache()
        {
            GameObject cacheObject = new GameObject(nameof(ScriptableCache));
            Object.DontDestroyOnLoad(cacheObject);
            
            cacheObject.hideFlags = HideFlags.HideInHierarchy;
            cacheObject.transform.hideFlags = HideFlags.HideInInspector;
            
            scriptableCache = cacheObject.AddComponent<ScriptableCache>();
        }
#endif
    }
}
