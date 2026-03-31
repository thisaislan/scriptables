using Thisaislan.Scriptables.Abstracts;
using UnityEngine;
using System;

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR    
    /// <summary>
    /// Creates a menu entry for creating DoubleScriptableSettings assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(DoubleScriptableSettings), menuName = RuntimeConsts.ScriptableSettingsMenuPath + nameof(DoubleScriptableSettings), order = 5)]
    /// <summary>
    /// Settings scriptable object for double values
    /// </summary>
#endif
    public class DoubleScriptableSettings : ScriptableSettings<DoubleScriptableSettings.DoubleData>
    {
        [Serializable]
        public class DoubleData : Data
        {
            public double DoubleValue;
        }
    }
}