#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Thisaislan.Scriptables.Editor.MonoBehaviours
{
    /// <summary>
    /// Editor-only component that displays pinned ScriptableObjects in the Inspector
    /// </summary>
    internal class ScriptableCache : MonoBehaviour
    {
        [NonSerialized]
        private List<ScriptableObject> scriptableObjects;

        /// <summary>
        /// Updates the displayed list of pinned ScriptableObjects
        /// </summary>
        /// <param name="scriptableObjects">The current list of pinned ScriptableObjects to show in the Inspector</param>
        internal void UpdateScriptableList(List<ScriptableObject> scriptableObjects)
        {
            this.scriptableObjects = scriptableObjects;
        }

        internal List<ScriptableObject> GetScriptableObjects()
        {
            return scriptableObjects ?? new List<ScriptableObject>();
        }
    }
}
#endif