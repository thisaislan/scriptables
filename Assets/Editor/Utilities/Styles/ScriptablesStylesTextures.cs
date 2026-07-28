#if UNITY_EDITOR
using UnityEngine;

namespace Thisaislan.Scriptables.Editor.Utilities.Styles
{
    /// <summary>
    /// Centralized texture provider for custom editor UI elements.
    /// Creates and caches solid color textures and rounded rectangle textures.
    /// </summary>
    internal static class ScriptablesStylesTextures
    {
        /// <summary>Rounded rectangle texture for card backgrounds.</summary>
        internal static Texture2D CardRoundedRectTexture;
        /// <summary>Rounded rectangle texture for help box backgrounds.</summary>
        internal static Texture2D HelpBoxRoundedRectTexture;

        /// <summary>Solid color texture for unselected window tab backgrounds.</summary>
        internal static Texture2D WindowTabUnselectedBgTexture;
        /// <summary>Solid color texture for selected window tab backgrounds.</summary>
        internal static Texture2D WindowTabSelectedBgTexture;
        /// <summary>Solid color texture for window search bar backgrounds.</summary>
        internal static Texture2D WindowSearchBarBgTexture;
        /// <summary>Solid color texture for window header backgrounds.</summary>
        internal static Texture2D WindowHeaderBgTexture;
        /// <summary>Solid color texture for hover state backgrounds.</summary>
        internal static Texture2D WindowHoverBgTexture;
        /// <summary>Solid color texture for normal window item backgrounds.</summary>
        internal static Texture2D WindowItemNormalBgTexture;
        /// <summary>Solid color texture for selected window item backgrounds.</summary>
        internal static Texture2D WindowItemSelectedBgTexture;
        /// <summary>Solid color texture for window info box backgrounds.</summary>
        internal static Texture2D WindowInfoBgTexture;
        /// <summary>Solid color texture for window bottom bar backgrounds.</summary>
        internal static Texture2D WindowBottomBarBgTexture;

        /// <summary>
        /// Gets or creates a 1x1 solid color texture.
        /// </summary>
        /// <param name="color">The color to fill the texture</param>
        /// <param name="cachedTexture">Reference to the cached texture</param>
        /// <returns>The cached or newly created solid color texture</returns>
        internal static Texture2D GetOrCreateSolidTexture(Color color, ref Texture2D cachedTexture)
        {
            if (cachedTexture == null)
            {
                cachedTexture = CreateSolidColorTexture(color);
            }

            return cachedTexture;
        }

        private static Texture2D CreateSolidColorTexture(Color color)
        {
            Texture2D tex = new Texture2D(1, 1, TextureFormat.ARGB32, false)
            {
                hideFlags = HideFlags.DontSave
            };

            tex.SetPixel(0, 0, color);
            tex.Apply();

            return tex;
        }

        /// <summary>
        /// Gets or creates a rounded rectangle texture with the given corner radius, background, and border colors.
        /// </summary>
        /// <param name="cornerRadius">The corner radius in pixels</param>
        /// <param name="backgroundColor">The fill color</param>
        /// <param name="borderColor">The border color</param>
        /// <param name="cachedTexture">Reference to the cached texture</param>
        /// <returns>The cached or newly created rounded rectangle texture</returns>
        internal static Texture2D GetRoundedRectTexture(int cornerRadius, Color backgroundColor, Color borderColor, ref Texture2D cachedTexture)
        {
            if (cachedTexture == null)
            {
                cachedTexture = CreateRoundedRectTexture(cornerRadius, backgroundColor, borderColor);
            }

            return cachedTexture;
        }

        private static Texture2D CreateRoundedRectTexture(int cornerRadius, Color bgColor, Color borderColor)
        {
            if (cornerRadius <= 0)
            {
                return CreateStraightTexture(bgColor, borderColor);
            }

            return CreateRoundedTexture(cornerRadius, bgColor, borderColor);
        }

        private static Texture2D CreateRoundedTexture(int cornerRadius, Color bgColor, Color borderColor)
        {
            int size = cornerRadius * 2 + 3;
            Texture2D tex = new Texture2D(size, size, TextureFormat.ARGB32, false);
            tex.hideFlags = HideFlags.DontSave;
            tex.filterMode = FilterMode.Point;

            float center = (size - 1) / 2f;
            float halfExtent = center;
            float radius = cornerRadius;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float px = x - center;
                    float py = y - center;

                    float ax = Mathf.Abs(px);
                    float ay = Mathf.Abs(py);

                    float qx = Mathf.Max(ax - (halfExtent - radius), 0f);
                    float qy = Mathf.Max(ay - (halfExtent - radius), 0f);

                    float dist = Mathf.Sqrt(qx * qx + qy * qy) - radius;

                    bool inside = dist <= 0f;
                    bool isBorder = inside && dist > -1f;

                    Color pixelColor = inside ? (isBorder ? borderColor : bgColor) : Color.clear;
                    tex.SetPixel(x, y, pixelColor);
                }
            }

            tex.Apply();

            return tex;
        }

        private static Texture2D CreateStraightTexture(Color bgColor, Color borderColor)
        {
            const int size = 3;

            Texture2D tex = new Texture2D(size, size, TextureFormat.ARGB32, false)
            {
                hideFlags = HideFlags.DontSave,
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    bool border = x == 0 || y == 0 || x == size - 1 || y == size - 1;
                    tex.SetPixel(x, y, border ? borderColor : bgColor);
                }
            }

            tex.Apply(false, true);
            
            return tex;
        }
    }
}
#endif
