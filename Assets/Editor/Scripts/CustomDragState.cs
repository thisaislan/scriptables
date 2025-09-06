// This preprocessor directive ensures the code is only compiled in the Unity Editor.
// It prevents errors when building the game, as the `UnityEditor` namespace is not available in builds.
#if UNITY_EDITOR
using UnityEngine;

namespace Thisaislan.Scriptables.Editor
{
    //// <summary>
    /// Custom drag state replacement for the inaccessible DragAndDropDelay class
    /// </summary>
    internal class CustomDragState
    {
        public Vector2 mouseDownPosition;
        public object originalValue;
        public bool isDragging;
        public int controlId;
    }
}
#endif