using Thisaislan.Scriptables.Abstracts;
using UnityEngine;
using System;

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR    
    /// <summary>
    /// Creates a menu entry for creating BooleanScriptableSettings assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(BooleanScriptableSettings), menuName = RuntimeConsts.ScriptableSettingsMenuPath + nameof(BooleanScriptableSettings), order = 2)]
    /// <summary>
    /// Runtime scriptable object for boolean values
    /// </summary>
#endif
    public class BooleanScriptableSettings : ScriptableSettings<BooleanScriptableSettings.BooleanData>
    {
        // Inherits all functionality from ScriptableSettings

        [Serializable]
        public class BooleanData : Data
        {
            public bool BoolValue;
        }
    }
}