#if UNITY_EDITOR
using UnityEngine;

namespace Thisaislan.Scriptables.Editor.Utilities
{
    /// <summary>
    /// Provides utility to shorten text strings to fit within a given pixel width,
    /// appending "..." when truncation is needed.
    /// </summary>
    internal static class StringShortener
    {
        private const string TildeSymbol = "...";
        private const int MaxRemoveCountLimit = 3;
        private const int MinRemoveCount = 1;

        private static readonly GUIContent MeasureContent = new GUIContent();

        /// <summary>
        /// Shortens the given text so that its rendered width does not exceed <paramref name="availableWidth"/>.
        /// </summary>
        /// <param name="text">The text to shorten</param>
        /// <param name="availableWidth">The maximum pixel width available</param>
        /// <param name="style">The <see cref="GUIStyle"/> used to measure the text width</param>
        /// <returns>The shortened text with "..." appended, or the original text if it already fits</returns>
        internal static string Shorten(string text, float availableWidth, GUIStyle style)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            MeasureContent.text = text;
            float fullWidth = style.CalcSize(MeasureContent).x;

            if (fullWidth <= availableWidth)
            {
                return text;
            }

            string shortened = text;
            int removeCount = MinRemoveCount;

            while (removeCount < shortened.Length)
            {
                string testString = shortened.Substring(0, shortened.Length - removeCount) + TildeSymbol;
                MeasureContent.text = testString;
                float testWidth = style.CalcSize(MeasureContent).x;

                if (testWidth <= availableWidth)
                {
                    return testString;
                }

                removeCount++;

                if (removeCount > shortened.Length - MaxRemoveCountLimit)
                {
                    return TildeSymbol;
                }
            }

            return shortened;
        }
    }
}
#endif
