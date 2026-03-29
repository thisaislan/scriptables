using Thisaislan.Scriptables.Abstracts;
using UnityEngine;
using System;

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR    
    /// <summary>
    /// Creates a menu entry for creating FloatScriptableRuntime assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(FloatScriptableRuntime), menuName = Meta.ScriptableRuntimeMenuPath + nameof(FloatScriptableRuntime), order = 4)]
    /// <summary>
    /// Runtime scriptable object for float values
    /// </summary>
#endif
    public class FloatScriptableRuntime : ScriptableRuntime<FloatScriptableRuntime.FloatData>
    {
        [Serializable]
        public class FloatData : Data
        {
            public float FloatValue;
        }
    }
}