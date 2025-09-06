using Thisaislan.Scriptables.Abstracts;
using UnityEngine;

#if UNITY_EDITOR
using Thisaislan.Scriptables.Editor;
#endif

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR
    /// <summary>
    /// Creates a menu entry for creating TransformScriptableReactive assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(TransformScriptableReactive), menuName = Consts.ScriptablesMenuPath + nameof(TransformScriptableReactive), order = 8)]
#endif
    /// <summary>
    /// Reactive scriptable object for Transform references with change notifications
    /// </summary>
    /// <remarks>
    /// This class extends ScriptableReactive<Transform> to provide a specialized implementation
    /// for Transform references with editor debugging support and change notifications.
    /// </remarks>
    public class TransformScriptableReactive : ScriptableReactive<Transform>
    {
        // Inherits all functionality from ScriptableReactive<Transform>
    }
}