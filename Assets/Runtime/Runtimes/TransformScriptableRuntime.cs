using Thisaislan.Scriptables.Abstracts;
using UnityEngine;
using System;

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR    
    /// <summary>
    /// Creates a menu entry for creating TransformScriptableRuntime assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(TransformScriptableRuntime), menuName = Meta.ScriptableRuntimeMenuPath + nameof(TransformScriptableRuntime), order = 8)]
    /// <summary>
    /// Runtime scriptable object for Transform references
    /// </summary>
#endif
    public class TransformScriptableRuntime : ScriptableRuntime<TransformScriptableRuntime.TransformData>
    {
        [Serializable]
        public class TransformData : Data
        {
            public Transform TransformValue;
        }
    }
}