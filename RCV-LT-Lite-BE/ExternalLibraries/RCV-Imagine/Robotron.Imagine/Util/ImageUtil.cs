using Robotron.Imagine.Extension;
using Robotron.Imagine.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;

namespace Robotron.Imagine.Util
{
    /// <summary>
    /// Utility class for work with images.
    /// </summary>
    public static class ImageUtil
    {
        #region GetDimensions

        /// <summary>
        /// Get Dimension for image.
        /// </summary>
        /// <param name="imageStream">Image as stream</param>
        /// <param name="width">Width of iamge</param>
        /// <param name="height">Height of image</param>
        public static void GetDimensionFromImage(System.IO.Stream imageStream, out int width, out int height)
        {
            Rect rect = GetDimensionFromImage(imageStream);
            width = rect.Width;
            height = rect.Height;
        }

        /// <summary>
        /// Get Dimension for image.
        /// </summary>
        /// <param name="imageStream">Image as stream</param>
        /// <returns>Rect with dimension of image</returns>
        public static Rect GetDimensionFromImage(System.IO.Stream imageStream)
        {
            #region validation

            if (imageStream == null)
            {
                throw new ArgumentNullException(nameof(imageStream));
            }

            #endregion

            imageStream.Rewind();
            using (Image<Rgba32> image = Image.Load(imageStream))
            {
                imageStream.Rewind();
                return new Rect()
                {
                    Width = image.Width,
                    Height = image.Height
                };
            }
        }

        /// <summary>
        /// Get Dimension for image.
        /// </summary>
        /// <param name="imageBytes">Image as byte array</param>
        /// <param name="width">Width of iamge</param>
        /// <param name="height">Height of image</param>
        public static void GetDimensionFromImage(byte[] imageBytes, out int width, out int height)
        {
            Rect rect = GetDimensionFromImage(imageBytes);
            width = rect.Width;
            height = rect.Height;
        }

        /// <summary>
        /// Get Dimension for image.
        /// </summary>
        /// <param name="imageBytes">Image as byte array</param>
        /// <returns>Rect with dimension of image</returns>
        public static Rect GetDimensionFromImage(byte[] imageBytes)
        {
            #region validation

            if (imageBytes == null)
            {
                throw new ArgumentNullException(nameof(imageBytes));
            }

            #endregion

            using (Image<Rgba32> image = Image.Load(imageBytes))
            {
                return new Rect()
                {
                    Width = image.Width,
                    Height = image.Height
                };
            }
        }

        #endregion

        #region TryOpenImage

        /// <summary>
        /// Try to open bitmap. If image is corrupted, an exception will be thrown.
        /// </summary>
        /// <param name="imageStream">Stream of image</param>        
        public static void TryOpenImage(System.IO.Stream imageStream)
        {
            #region validation

            if (imageStream == null)
            {
                throw new ArgumentNullException(nameof(imageStream));
            }

            #endregion

            imageStream.Rewind();
            using (Image<Rgba32> image = Image.Load(imageStream))
            {
                imageStream.Rewind();
            }
        }

        /// <summary>
        /// Try to open bitmap. If image is corrupted, an exception will be thrown.
        /// </summary>
        /// <param name="imageBytes">Bytes of image</param>        
        public static void TryOpenImage(byte[] imageBytes)
        {
            #region validation

            if (imageBytes == null)
            {
                throw new ArgumentNullException(nameof(imageBytes));
            }

            #endregion

            using (Image<Rgba32> image = Image.Load(imageBytes))
            {
            }
        }

        #endregion
    }
}
