using SixLabors.ImageSharp.PixelFormats;
using System;

namespace Robotron.Imagine.Util
{
    /// <summary>
    /// Utilities for Rgba32-color.
    /// </summary>
    internal static class Rgba32Util
    {
        /// <summary>
        /// Init Rgba32 color from hexadecimal color string.
        /// </summary>
        /// <param name="hexColorCode">Color code to use for rgba32 color</param>
        /// <returns>Rgba32 color with representation of hexadecimal color code</returns>
        public static Rgba32 InitFromHex(string hexColorCode)
        {
            #region validation

            if (string.IsNullOrEmpty(hexColorCode))
            {
                throw new ArgumentNullException(nameof(hexColorCode));
            }

            if (!HexColorUtil.GetHexColorRegex().IsMatch(hexColorCode))
            {
                throw new ArgumentException($"Invalid hexadecimal color code {hexColorCode}");
            }

            #endregion

            // convert color
            byte r = HexColorUtil.GetR(hexColorCode);
            byte g = HexColorUtil.GetG(hexColorCode);
            byte b = HexColorUtil.GetB(hexColorCode);
            byte a = HexColorUtil.GetA(hexColorCode);

            return new Rgba32(r, g, b, a);
        }
    }
}
