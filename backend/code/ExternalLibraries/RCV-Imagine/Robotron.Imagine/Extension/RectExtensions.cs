using Robotron.Imagine.Model;

namespace Robotron.Imagine.Extension
{
    /// <summary>
    /// Extensions for rect structure.
    /// </summary>
    internal static class RectExtensions
    {
        /// <summary>
        /// Checks if rect is square/quadratically.
        /// Rect is square if width and height are equal.
        /// </summary>
        /// <param name="rect">Rect to check</param>
        /// <returns>TRUE if rect is square, otherwise FALSE</returns>
        public static bool IsSquare(this Rect rect)
        {
            return rect.Width == rect.Height;
        }

        /// <summary>
        /// Checks if rect has horizontal format.
        /// Rect has horizontal format if width greater than height.
        /// </summary>
        /// <param name="rect">Rect to check</param>
        /// <returns>TRUE if rect has horizontal format, otherwise FALSE</returns>
        public static bool IsHorizontalFormat(this Rect rect)
        {
            return rect.Width > rect.Height;
        }

        /// <summary>
        /// Checks if rect has vertical format.
        /// Rect has vertical format if height greater than width.
        /// </summary>
        /// <param name="rect">Rect to check</param>
        /// <returns>TRUE if rect has vertical format, otherwise FALSE</returns>
        public static bool IsVerticalFormat(this Rect rect)
        {
            return rect.Height > rect.Width;
        }
    }
}
