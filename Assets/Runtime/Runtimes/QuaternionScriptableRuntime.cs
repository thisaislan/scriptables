using Thisaislan.Scriptables.Abstracts;
using UnityEngine;
using System;

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR    
    /// <summary>
    /// Creates a menu entry for creating QuaternionScriptableRuntime assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(QuaternionScriptableRuntime), menuName = Meta.ScriptableRuntimeMenuPath + nameof(QuaternionScriptableRuntime), order = 9)]
    /// <summary>
    /// Runtime scriptable object for Quaternion values
    /// </summary>
#endif
    public class QuaternionScriptableRuntime : ScriptableRuntime<QuaternionScriptableRuntime.QuaternionData>
    {
        [Serializable]
        public class QuaternionData : Data
        {
            public Quaternion QuaternionValue;
        }
    }
}