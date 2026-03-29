using Thisaislan.Scriptables.Abstracts;
using UnityEngine;
using System;

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR    
    /// <summary>
    /// Creates a menu entry for creating IntScriptableSettings assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(IntScriptableSettings), menuName = Meta.ScriptableSettingsMenuPath + nameof(IntScriptableSettings), order = 3)]
    /// <summary>
    /// Settings scriptable object for integer values
    /// </summary>
#endif
    public class IntScriptableSettings : ScriptableSettings<IntScriptableSettings.IntData>
    {
        [Serializable]
        public class IntData : Data
        {
            public int IntValue;
        }
    }
}