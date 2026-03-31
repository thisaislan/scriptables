using Thisaislan.Scriptables.Abstracts;
using UnityEngine;
using System;

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR    
    /// <summary>
    /// Creates a menu entry for creating GameObjectScriptableRuntime assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(GameObjectScriptableRuntime), menuName = RuntimeConsts.ScriptableRuntimeMenuPath + nameof(GameObjectScriptableRuntime), order = 7)]
    /// <summary>
    /// Runtime scriptable object for GameObject references
    /// </summary>
#endif
    public class GameObjectScriptableRuntime : ScriptableRuntime<GameObjectScriptableRuntime.GameObjectData>
    {
        [Serializable]
        public class GameObjectData : Data
        {
            public GameObject GameObjectValue;
        }
    }
}