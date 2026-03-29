using Thisaislan.Scriptables.Abstracts;
using UnityEngine;
using System;

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR    
    /// <summary>
    /// Creates a menu entry for creating Vector2ScriptableRuntime assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(Vector2ScriptableRuntime), menuName = Meta.ScriptableRuntimeMenuPath + nameof(Vector2ScriptableRuntime), order = 10)]
    /// <summary>
    /// Runtime scriptable object for Vector2 values
    /// </summary>
#endif
    public class Vector2ScriptableRuntime : ScriptableRuntime<Vector2ScriptableRuntime.Vector2Data>
    {
        [Serializable]
        public class Vector2Data : Data
        {
            public Vector2 Vector2Value;
        }
    }
}