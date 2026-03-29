using Thisaislan.Scriptables.Abstracts;
using UnityEngine;
using System;

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR    
    /// <summary>
    /// Creates a menu entry for creating FloatScriptableSettings assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(FloatScriptableSettings), menuName = Meta.ScriptableSettingsMenuPath + nameof(FloatScriptableSettings), order = 4)]
    /// <summary>
    /// Settings scriptable object for float values
    /// </summary>
#endif
    public class FloatScriptableSettings : ScriptableSettings<FloatScriptableSettings.FloatData>
    {
        [Serializable]
        public class FloatData : Data
        {
            public float FloatValue;
        }
    }
}