using Thisaislan.Scriptables.Abstracts;
using UnityEngine;

#if UNITY_EDITOR
using Thisaislan.Scriptables.Editor;
#endif

namespace Thisaislan.Scriptables
{
#if UNITY_EDITOR
    /// <summary>
    /// Creates a menu entry for creating GameObjectScriptableReactive assets in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = nameof(GameObjectScriptableReactive), menuName = Consts.ScriptablesMenuPath + nameof(GameObjectScriptableReactive), order = 7)]
#endif
    /// <summary>
    /// Reactive scriptable object for GameObject references with change notifications
    /// </summary>
    /// <remarks>
    /// This class extends ScriptableReactive<GameObject> to provide a specialized implementation
    /// for GameObject references with editor debugging support and change notifications.
    /// </remarks>
    public class GameObjectScriptableReactive : ScriptableReactive<GameObject>
    {
        // Inherits all functionality from ScriptableReactive<GameObject>
    }
}