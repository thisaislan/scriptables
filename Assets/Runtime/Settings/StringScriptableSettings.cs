using Thisaislan.Scriptables.Abstracts;
using UnityEngine;
using System;

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR    
    /// <summary>
    /// Creates a menu entry for creating StringScriptableSettings assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(StringScriptableSettings), menuName = RuntimeConsts.ScriptableSettingsMenuPath + nameof(StringScriptableSettings), order = 6)]
    /// <summary>
    /// Settings scriptable object for string values
    /// </summary>
#endif
    public class StringScriptableSettings : ScriptableSettings<StringScriptableSettings.StringData>
    {
        [Serializable]
        public class StringData : Data
        {
            public string StringValue;
        }
    }
}