using UnityEngine;

namespace Thisaislan.Scriptables.Interfaces
{
    /// <summary>
    /// Defines methods for keeping and releasing ScriptableObject references in memory
    /// </summary>
    public interface IPinnable
    {
        /// <summary>
        /// Keeps the Scriptable alive by adding it to the reference list
        /// </summary>
        public void Pin();
        
        /// <summary>
        /// Releases the ScriptableObject by removing it from the reference list
        /// </summary>
        public void Unpin();
    }
}