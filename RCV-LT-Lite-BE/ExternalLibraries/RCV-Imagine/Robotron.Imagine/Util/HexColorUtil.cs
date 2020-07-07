using System;
using System.Text.RegularExpressions;

namespace Robotron.Imagine.Util
{
    /// <summary>
    /// Util for hexadecimal color representation.
    /// </summary>
    public static class HexColorUtil
    {
        /// <summary>
        /// Get regex for valid hexadecimal color representation.
        /// </summary>
        /// <returns></returns>
        public static Regex GetHexColorRegex()
        {
            return new Regex("^#?(([0-9A-F]){3}|([0-9A-F]){4}|([0-9A-F]){6}|([0-9A-F]){8})$", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Get red from hexColor.
        /// </summary>
        /// <param name="hexColor">Color in hexadecimal representation</param>
        /// <returns>Red color</returns>
        public static byte GetR(string hexColor)
        {
            return GetRGBA(hexColor, EColorPosition.R);
        }

        /// <summary>
        /// Get green from hexColor.
        /// </summary>
        /// <param name="hexColor">Color in hexadecimal representation</param>
        /// <returns>Green color</returns>
        public static byte GetG(string hexColor)
        {
            return GetRGBA(hexColor, EColorPosition.G);
        }

        /// <summary>
        /// Get blue from hexColor.
        /// </summary>
        /// <param name="hexColor">Color in hexadecimal representation</param>
        /// <returns>Blue color</returns>
        public static byte GetB(string hexColor)
        {
            return GetRGBA(hexColor, EColorPosition.B);
        }

        /// <summary>
        /// Get alpha from hexColor.
        /// </summary>
        /// <param name="hexColor">Color in hexadecimal representation</param>
        /// <returns>Alpha color</returns>
        public static byte GetA(string hexColor)
        {
            return GetRGBA(hexColor, EColorPosition.A);
        }

        #region private helper

        /// <summary>
        /// Position of color-channel inside hexadecimal color representation.
        /// </summary>
        private enum EColorPosition : int
        {
            /// <summary>
            /// Red.
            /// </summary>
            R = 0,

            /// <summary>
            /// Green.
            /// </summary>
            G,

            /// <summary>
            /// Blue.
            /// </summary>
            B,

            /// <summary>
            /// Alpha.
            /// </summary>
            A
        }

        /// <summary>
        /// Get color for specific channel.
        /// </summary>
        /// <param name="hexColor">Hexadecimal color representation</param>
        /// <param name="colorPosition">Position of color to get.</param>
        /// <returns>Color for specified channel</returns>
        private static byte GetRGBA(string hexColor, EColorPosition colorPosition)
        {
            #region validation

            if (string.IsNullOrEmpty(hexColor))
            {
                throw new ArgumentNullException(nameof(hexColor));
            }

            #endregion

            string plainColorCode = hexColor.Trim();
            if (plainColorCode.StartsWith("#"))
            {
                plainColorCode = plainColorCode.Substring(1);
            }

            // if alpha channel is requested but not coded in hexColor
            if (plainColorCode.Length % 3 == 0 && colorPosition == EColorPosition.A)
            {
                return byte.MaxValue;
            }

            switch (plainColorCode.Length)
            {
                case 3:
                case 4:
                    return (byte)(Convert.ToByte(plainColorCode.Substring((int)colorPosition, 1), 16) * 0x10);
                case 6:
                case 8:
                    return Convert.ToByte(plainColorCode.Substring((int)colorPosition * 2, 2), 16);
            }

            throw new ArgumentOutOfRangeException(nameof(hexColor));
        }

        #endregion
    }
}
