using Rcv.LabelTool.Contracts.Models;
using Serilog;
using System;
using System.IO;

namespace Rcv.LabelTool.Logic.Utilities
{
    /// <summary>
    /// Util to work with images.
    /// </summary>
    public static class ImageUtil
    {
        /// <summary>
        /// Get Dimension for image from request.
        /// </summary>
        /// <param name="imageStream">Image as stream</param>
        /// <returns>Rect with dimension of image</returns>
        public static Rect GetDimensionFromImage(Stream imageStream)
        {
            Robotron.Imagine.Model.Rect imageDimension = Robotron.Imagine.Util.ImageUtil.GetDimensionFromImage(imageStream);
            return ConvertFromRect(imageDimension);
        }

        /// <summary>
        /// Try to open bitmap. If image is corrupted, an exception will be thrown.
        /// </summary>
        /// <param name="imageStream">Stream of image</param>
        public static void TryOpenImage(Stream imageStream)
        {
            Robotron.Imagine.Util.ImageUtil.TryOpenImage(imageStream);
        }

        /// <summary>
        /// Check is image from stream is valid image.
        /// If image is no valid image, log will be written.
        /// </summary>
        /// <param name="imageStream">Stream of image to check</param>
        /// <param name="imageFileName">Name of image to check</param>
        /// <returns>TRUE if image is valid, FALSE is image is corrupted</returns>
        public static bool IsValidImage(Stream imageStream, string imageFileName)
        {
            #region validation

            if (imageStream == null)
            {
                throw new ArgumentNullException(nameof(imageStream));
            }

            if (string.IsNullOrEmpty(imageFileName))
            {
                throw new ArgumentNullException(nameof(imageFileName));
            }

            #endregion

            try
            {
                TryOpenImage(imageStream);
            }
            catch (Exception exc)
            {
                Log.Warning(exc, $"Image '{imageFileName}' is corrupted.");
                return false;
            }
            return true;
        }

        #region private methods

        /// <summary>
        /// Create rect for labeltool from image lib rect.
        /// </summary>
        /// <param name="rect">Rect to convert to labeltool rect</param>
        /// <returns>Labeltool rect</returns>
        private static Rect ConvertFromRect(Robotron.Imagine.Model.Rect rect)
        {
            return new Rect() { Left = rect.X, Top = rect.Y, Height = rect.Height, Width = rect.Width };
        }

        #endregion
    }
}
