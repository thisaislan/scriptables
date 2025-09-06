#if UNITY_EDITOR
using UnityEngine;

namespace Thisaislan.Scriptables.Editor.Utilities
{
    /// <summary>
    /// Utility class for printing formatted messages to the Unity console with controlled stack traces.
    /// Provides methods for logging messages, warnings, and errors with customizable stack trace behavior.
    /// </summary>
    /// <remarks>
    /// This class enhances Unity's default logging by:
    /// - Temporarily suppressing stack traces for cleaner console output
    /// - Providing consistent logging format throughout the editor system
    /// - Allowing control over stack trace visibility on a per-message basis
    /// </remarks>
    internal static class Printer
    {
        /// <summary>
        /// Prints a standard log message to the Unity console without stack traces
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <remarks>
        /// Useful for informational messages where stack traces would add unnecessary noise.
        /// Temporarily disables stack traces for LogType.Log during the message output.
        /// </remarks>
        internal static void PrintMessage(string message)
        {
            Print(LogType.Log, message);
        }

        /// <summary>
        /// Prints a warning message to the Unity console without stack traces
        /// </summary>
        /// <param name="message">The warning message to log</param>
        /// <remarks>
        /// Useful for non-critical warnings where stack traces would add unnecessary noise.
        /// Temporarily disables stack traces for LogType.Warning during the message output.
        /// </remarks>
        internal static void PrintWarning(string message)
        {
            Print(LogType.Warning, message);
        }

        /// <summary>
        /// Prints an error message to the Unity console without stack traces
        /// </summary>
        /// <param name="message">The error message to log</param>
        /// <remarks>
        /// Useful for error messages where the stack trace isn't immediately needed.
        /// Temporarily disables stack traces for LogType.Error during the message output.
        /// </remarks>
        internal static void PrintError(string message)
        {
            Print(LogType.Error, message);
        }
        
        /// <summary>
        /// Internal method that handles the actual printing with stack trace control
        /// </summary>
        /// <param name="logType">The type of log message (Log, Warning, Error)</param>
        /// <param name="message">The message to print</param>
        /// <remarks>
        /// This method:
        /// 1. Saves the current stack trace setting for the given log type
        /// 2. Temporarily disables stack traces for cleaner output
        /// 3. Prints the message using the appropriate Debug method
        /// 4. Restores the original stack trace setting
        /// 
        /// This ensures that stack trace settings are not permanently modified.
        /// </remarks>
        private static void Print(LogType logType, string message)
        {
            // Save the current stack trace setting for this log type
            var stackTraceLogType = Application.GetStackTraceLogType(logType);

            // Temporarily disable stack traces for cleaner output
            Application.SetStackTraceLogType(logType, StackTraceLogType.None);

            // Print the message using the appropriate log method
            if (logType == LogType.Error) 
            { 
                Debug.LogError(message); 
            }
            else if (logType == LogType.Warning) 
            { 
                Debug.LogWarning(message); 
            }
            else 
            {  
                Debug.Log(message); 
            }

            // Restore the original stack trace setting
            Application.SetStackTraceLogType(logType, stackTraceLogType);
        }
    }
}
#endif