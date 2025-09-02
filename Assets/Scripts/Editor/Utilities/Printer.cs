#if UNITY_EDITOR
using UnityEngine;

namespace Thisaislan.Scriptables.Editor.Utilities
{
    internal static class Printer
    {
        internal static void PrintMessage(string message)
        {
            Print(LogType.Log, message);
        }

        internal static void PrintWarning(string message)
        {
            Print(LogType.Warning, message);
        }

        internal static void PrintError(string message)
        {
            Print(LogType.Error, message);
        }
        
        private static void Print(LogType logType, string message)
        {
            var stackTraceLogType = Application.GetStackTraceLogType(logType);

            Application.SetStackTraceLogType(logType, StackTraceLogType.None);

            if (logType == LogType.Error) { Debug.LogError(message); }
            else if (logType == LogType.Warning) { Debug.LogWarning(message); }
            else {  Debug.Log(message); }

            Application.SetStackTraceLogType(logType, stackTraceLogType);
        }
    }
}
#endif