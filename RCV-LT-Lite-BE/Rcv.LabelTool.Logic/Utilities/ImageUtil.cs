using Rcv.LabelTool.Contracts.Models;

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
        public static Rect GetDimensionFromImage(System.IO.Stream imageStream)
        {
            Robotron.Imagine.Model.Rect imageDimension = Robotron.Imagine.Util.ImageUtil.GetDimensionFromImage(imageStream);
            return new Rect()
            {
                Width = imageDimension.Width,
                Height = imageDimension.Height
            };
        }
    }
}
