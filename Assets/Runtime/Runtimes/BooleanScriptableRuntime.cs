using Thisaislan.Scriptables.Abstracts;
using UnityEngine;
using System;


#if UNITY_EDITOR
using Thisaislan.Scriptables.Editor;
#endif

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR    
    /// <summary>
    /// Creates a menu entry for creating BooleanScriptableRuntime assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(BooleanScriptableRuntime), menuName = Meta.ScriptableRuntimeMenuPath + nameof(BooleanScriptableRuntime), order = 2)]
    /// <summary>
    /// Runtime scriptable object for boolean values
    /// </summary>
#endif
    public class BooleanScriptableRuntime : ScriptableRuntime<BooleanScriptableRuntime.BooleanData>
    {
        // Inherits all functionality from ScriptableReactive

        [Serializable]
        public class BooleanData : Data
        {   
            public bool BoolValue;
        }
    }
}