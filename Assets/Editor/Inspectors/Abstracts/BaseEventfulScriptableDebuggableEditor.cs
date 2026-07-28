#if UNITY_EDITOR
using System;
using System.Reflection;
using Thisaislan.Scriptables.Editor.Utilities.DrawHelpers;
using UnityEngine;

namespace Thisaislan.Scriptables.Editor.Inspectors.Abstracts
{
    /// <summary>
    /// Custom base editor for eventful debbugable scriptables editors.
    /// Provides base methods shared.
    /// </summary>
    internal abstract class BaseEventfulScriptableDebuggableEditor<T> :  BaseTransientDataScriptableDebuggableEditor<T>  where T : UnityEngine.Object
    {
        protected const string callbacksFieldName = "callbacks";
        protected const string EmitDataErrorMessage = " Unable to emit {0}. Possible cause invalid state";

        /// <summary>
        /// Draws the list of registered runtime subscribers using reflection to find the action delegate.
        /// </summary>
        protected virtual void DrawRuntimeObserversSingleModeCard(
            ScriptableObject scriptable,
            ref Vector2 scrollPos,
            Action actionOnNotifyAll,
            bool enabled,
            Func<object> getNotifyData = null)
        {
            Type type = scriptable.GetType();
            string messageOnNotifyError = string.Format(EmitDataErrorMessage, serializedObject.targetObject.name);

            // Traverse the inheritance chain to find the private "callbacks" field
            FieldInfo actionField = null;

            while (type != null)
            {
                actionField = type.GetField(callbacksFieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                
                if (actionField != null)
                {
                    break;
                }
                
                type = type.BaseType;
            }

            Delegate actionDelegate = actionField?.GetValue(scriptable) as Delegate;

            DrawSubscribersListEditorHelper.DrawRuntimeObserversListCard(
                actionDelegate: actionDelegate,
                scrollPos: ref scrollPos,
                actionOnEmitAll: actionOnNotifyAll,
                messageOnEmitError: messageOnNotifyError,
                enabled: enabled,
                getEmitData: getNotifyData);
        }

        /// <summary>
        /// Draws the UI card for multiple mode, providing an option to emit
        /// all runtime subscribers simultaneously.
        /// </summary>
        protected virtual void DrawRuntimeObserversMultipleModeCard(
            bool enabled,
            Action actionOnNotifyAll)
        {
            DrawSubscribersListEditorHelper.DrawRuntimeObserversCard(
                actionOnEmitAll: actionOnNotifyAll,
                enabled: enabled);
        }
    }
}
#endif