using System;
using UnityEngine;

#if UNITY_EDITOR
using Thisaislan.Scriptables.Editor.Abstracts;
using Thisaislan.Scriptables.Editor.Utilities;
#endif

namespace Thisaislan.Scriptables.Abstracts
{
#if UNITY_EDITOR
    /// <summary>
    /// Generic abstract scriptable object class that extends <see cref="ScriptableEditorDebbugable"/>.
    /// This is used for creating editor-debuggable scriptable objects bound to a specific <see cref="Data"/> type.
    /// </summary>
    public abstract class Scriptable<T> : ScriptableEditorDebbugable where T : Data
#else
    /// <summary>
    /// Generic abstract scriptable object class used in builds (non-editor).
    /// This version excludes debugging functionality to reduce runtime overhead.
    /// </summary>
    /// <typeparam name="T">The data type associated with this scriptable object.</typeparam>
    public abstract class Scriptable<T> : ScriptableObject where T : Data
#endif
    {
        internal Scriptable()
        {
            // Avoid external heritage
        }

        [NonSerialized]
        private T data;

        public virtual T Data
        {
            get
            {
                return data;
            }
            protected set
            {  
                data = value;
            }
        }

#if UNITY_EDITOR
        public override void PrintDataDebugEditor()
        {
            Printer.PrintMessage($"{typeof(T).ToString().Replace("+","<")}>\n{GetStringData()}");
        }

        internal override string GetStringData()
        {
            return JsonUtility.ToJson(data, true);
        }

        internal override object GetDataObject()
        {
            return data;
        }

        internal override void ResetToDefaultState()
        {
            // Base implementation does nothing
        }
#endif
    }
}