#if UNITY_EDITOR
using System;
using Thisaislan.Scriptables.Editor.Utilities.DrawHelpers;
using UnityEngine;

namespace Thisaislan.Scriptables.Editor.Inspectors.Abstracts
{
    /// <summary>
    /// Custom base editor for dual data debbugable scriptables editors.
    /// Provides base methods shared.
    /// </summary>
    internal abstract class BaseDualDataScriptableDebuggableEditor<T> :  BaseTransientDataScriptableDebuggableEditor<T> where T : UnityEngine.Object
    {
        /// <summary>
        /// Draws two small buttons horizontally centered.
        /// </summary>
        /// <param name="onLeftClick">Action for left button</param>
        /// <param name="onRightClick">Action for right button</param>
        /// <param name="isDataPropertyNotNull">Determines whether the data is null or not.</param>
        protected void DrawBridge(Action onLeftClick, Action onRightClick, bool isDataPropertyNotNull)
        {
            DrawDataEditorHelper.DrawBridge(
                onLeftClick: onLeftClick,
                onRightClick: onRightClick,
                enabled: Application.isPlaying && isDataPropertyNotNull);
        }
    }
}
#endif