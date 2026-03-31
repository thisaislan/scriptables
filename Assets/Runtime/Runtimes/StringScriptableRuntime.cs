using Thisaislan.Scriptables.Abstracts;
using UnityEngine;
using System;

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR    
    /// <summary>
    /// Creates a menu entry for creating StringScriptableRuntime assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(StringScriptableRuntime), menuName = RuntimeConsts.ScriptableRuntimeMenuPath + nameof(StringScriptableRuntime), order = 6)]
    /// <summary>
    /// Runtime scriptable object for string values
    /// </summary>
#endif
    public class StringScriptableRuntime : ScriptableRuntime<StringScriptableRuntime.StringData>
    {
        [Serializable]
        public class StringData : Data
        {
            public string StringValue;
        }
    }
}