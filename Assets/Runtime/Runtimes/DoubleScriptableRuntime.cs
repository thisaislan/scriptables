using Thisaislan.Scriptables.Abstracts;
using UnityEngine;
using System;

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR    
    /// <summary>
    /// Creates a menu entry for creating DoubleScriptableRuntime assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(DoubleScriptableRuntime), menuName = RuntimeConsts.ScriptableRuntimeMenuPath + nameof(DoubleScriptableRuntime), order = 5)]
    /// <summary>
    /// Runtime scriptable object for double values
    /// </summary>
#endif
    public class DoubleScriptableRuntime : ScriptableRuntime<DoubleScriptableRuntime.DoubleData>
    {
        [Serializable]
        public class DoubleData : Data
        {
            public double DoubleValue;
        }
    }
}