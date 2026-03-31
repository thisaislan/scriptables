using Thisaislan.Scriptables.Abstracts;
using UnityEngine;
using System;

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR    
    /// <summary>
    /// Creates a menu entry for creating GameObjectScriptableSettings assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(GameObjectScriptableSettings), menuName = RuntimeConsts.ScriptableSettingsMenuPath + nameof(GameObjectScriptableSettings), order = 7)]
    /// <summary>
    /// Settings scriptable object for GameObject references
    /// </summary>
#endif
    public class GameObjectScriptableSettings : ScriptableSettings<GameObjectScriptableSettings.GameObjectData>
    {
        [Serializable]
        public class GameObjectData : Data
        {
            public GameObject GameObjectValue;
        }
    }
}