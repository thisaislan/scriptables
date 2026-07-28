#if UNITY_EDITOR
using System;
using Thisaislan.Scriptables.Editor.Utilities;
using UnityEngine;

namespace Thisaislan.Scriptables.Editor.Inspectors.Abstracts
{
    /// <summary>
    /// Custom base editor for transient debbugable scriptables editors.
    /// Provides base methods shared.
    /// </summary>
    internal abstract class BaseTransientDataScriptableDebuggableEditor<T> :  BaseScriptableDebuggableEditor<T>  where T : UnityEngine.Object
    {
        protected const string HandleDataErrorMessage = " Unable to handle Scriptable. Possible causes: null data object, corrupted serialization, or invalid state";

        /// <summary>
        /// Executes the specified function after clearing GUI focus, then repaints the inspector.
        /// Handles exceptions gracefully by logging a warning without breaking the editor.
        /// </summary>
        /// <param name="func">The action to execute (e.g., resetting runtime data)</param>
        protected virtual T2 SafeExecuteFuncWithFocusReset<T2>(Func<T2> func)
        {
            T2 value;

            GUI.FocusControl(null);
            value = SafeExecuteFunc<T2>(func, GetFormattedErrorHandler());
            Repaint();

            return value;
        }

        /// <summary>
        /// Executes the specified function after clearing GUI focus, then repaints the inspector.
        /// Handles exceptions gracefully by logging a warning without breaking the editor.
        /// <param name="message">error message</param>
        protected virtual T2 SafeExecuteFunc<T2>(Func<T2> func)
        {
            return SafeExecuteFunc(func);
        }

        /// <summary>
        /// Executes the specified function after clearing GUI focus, then repaints the inspector.
        /// Handles exceptions gracefully by logging a warning without breaking the editor.
        /// <param name="func">The action to execute (e.g., resetting runtime data)</param>
        /// <param name="message">error message</param>
        protected virtual T2 SafeExecuteFunc<T2>(Func<T2> func, string message)
        {
            try
            {
                return func.Invoke();
            }
            catch
            {
                Printer.PrintWarning(message);
            }

            return default;
        }

        /// <summary>
        /// Executes the specified action after clearing GUI focus, then repaints the inspector.
        /// Handles exceptions gracefully by logging a warning without breaking the editor.
        /// </summary>
        /// <param name="action">The action to execute (e.g., resetting runtime data)</param>
        protected virtual void SafeExecuteWithFocusReset(Action action)
        {
            GUI.FocusControl(null);
            SafeExecute(action, GetFormattedErrorHandler());
            Repaint();
        }

        /// <summary>
        /// Executes the specified action after inside a try catch with a error print in the catch.
        /// Handles exceptions gracefully by logging a warning without breaking the editor.
        /// </summary>
        /// <param name="action">The action to execute (e.g., resetting runtime data)</param>
        protected virtual void SafeExecute(Action action)
        {
            SafeExecute(action, GetFormattedErrorHandler());
        }

        /// <summary>
        /// Executes the specified action after inside a try catch with a error print in the catch.
        /// Handles exceptions gracefully by logging a warning without breaking the editor.
        /// </summary>
        /// <param name="action">The action to execute (e.g., resetting runtime data)</param>
        /// <param name="message">error message</param>
        protected virtual void SafeExecute(Action action, string message)
        {
            try
            {
                action?.Invoke();
            }
            catch
            {
                Printer.PrintWarning(message);
            }
        }

        /// <summary>
        /// Formats an error message for a data handling operation failure.
        /// </summary>
        /// <returns>A formatted error string containing the serialized object name and the specific error message.</returns>
        protected virtual string GetFormattedErrorHandler()
        {
            return $"{serializedObject.targetObject.name}: {HandleDataErrorMessage}";
        }
    }
}
#endif