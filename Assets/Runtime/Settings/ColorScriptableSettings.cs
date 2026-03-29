using Thisaislan.Scriptables.Abstracts;
using UnityEngine;
using System;

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR    
    /// <summary>
    /// Creates a menu entry for creating ColorScriptableSettings assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(ColorScriptableSettings), menuName = Meta.ScriptableSettingsMenuPath + nameof(ColorScriptableSettings), order = 12)]
    /// <summary>
    /// Settings scriptable object for Color values
    /// </summary>
#endif
    public class ColorScriptableSettings : ScriptableSettings<ColorScriptableSettings.ColorData>
    {
        [Serializable]
        public class ColorData : Data
        {
            public Color ColorValue;
        }
    }
}