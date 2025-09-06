using Thisaislan.Scriptables.Abstracts;
using UnityEngine;

#if UNITY_EDITOR
using Thisaislan.Scriptables.Editor;
#endif

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR
    /// <summary>
    /// Creates a menu entry for creating Vector2ScriptableReactive assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(Vector2ScriptableReactive), menuName = Consts.ScriptablesMenuPath + nameof(Vector2ScriptableReactive), order = 10)]
#endif
    /// <summary>
    /// Reactive scriptable object for Vector2 values with change notifications
    /// </summary>
    /// <remarks>
    /// This class extends ScriptableReactive<Vector2> to provide a specialized implementation
    /// for Vector2 values with editor debugging support and change notifications.
    /// </remarks>
    public class Vector2ScriptableReactive : ScriptableReactive<Vector2>
    {
        // Inherits all functionality from ScriptableReactive<Vector2>
    }
}