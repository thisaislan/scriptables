using Thisaislan.Scriptables.Abstracts;
using UnityEngine;
using System;

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR    
    /// <summary>
    /// Creates a menu entry for creating IntScriptableRuntime assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(IntScriptableRuntime), menuName = RuntimeConsts.ScriptableRuntimeMenuPath + nameof(IntScriptableRuntime), order = 3)]
    /// <summary>
    /// Runtime scriptable object for integer values
    /// </summary>
#endif
    public class IntScriptableRuntime : ScriptableRuntime<IntScriptableRuntime.IntData>
    {
        [Serializable]
        public class IntData : Data
        {
            public int IntValue;
        }
    }
}