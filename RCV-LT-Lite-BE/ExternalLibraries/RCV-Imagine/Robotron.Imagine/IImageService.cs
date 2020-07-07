using Robotron.Imagine.Model;

namespace Robotron.Imagine
{
    /// <summary>
    /// Definiton of imageservice.
    /// Handles several operations to work with images.
    /// </summary>
    public interface IImageService
    {
        /// <summary>
        /// Cuts of part of image. Used for cut of bounding-box and classify bounding-box content.
        /// </summary>
        /// <param name="imageStream">Image as stream</param>
        /// <param name="x">Left coordinate of bounding-box</param>
        /// <param name="y">Upper coordinate of bounding-box</param>
        /// <param name="width">Width of bounding-box</param>
        /// <param name="heigth">Height of bounding-box</param>
        /// <returns>Bounding-box imagedata as stream</returns>
        System.IO.Stream GetImageDetail(System.IO.Stream imageStream, int x, int y, int width, int heigth);

        /// <summary>
        /// Resize image with desired dimensions.
        /// If width and height are given, image will be resized to desired size.
        /// If only width is given, height will be resized in relation to original height.
        /// If only height is given, widht will be resized in relation to original widht.
        /// If width and height are both null, exception will be thrown.
        /// </summary>
        /// <param name="imageStream">Stream with source image</param>
        /// <param name="targetImageWidth">Width of target image OR null</param>
        /// <param name="targetImageHeight">Height of target image OR null</param>
        /// <returns>Resized image as stream</returns>
        System.IO.Stream ResizeImage(System.IO.Stream imageStream, int? targetImageWidth, int? targetImageHeight);

        /// <summary>
        /// Returns cropped square image as stream.
        /// Cuts image to dimensions of minimal side width (height or width).
        /// If width is greater then height, resulting image will be height x height pixel.
        /// If height is greater then widht, resulting image will be widht x width pixel.
        /// Information will be lost.
        /// </summary>
        /// <param name="imageStream">Stream with source image</param>
        /// <returns>Stream with resulting cropped square image</returns>
        System.IO.Stream CropSquareImage(System.IO.Stream imageStream);

        /// <summary>
        /// Returns filled square image as stream.
        /// Fills border with given color. 
        /// Resulting image will have size of maximal side width (height or widht).
        /// If width is greater then height, resulting image will be width x width pixel.
        /// If height is greater then widht, resulting image will be height x height pixel.
        /// No information will be lost.
        /// </summary>
        /// <remarks>
        /// FillColor is used as hex-string. Only uppercase-letters are valid.
        /// Leading # is necessary!
        /// </remarks>
        /// <param name="imageStream">Stream with source image</param>
        /// <param name="fillColor">Color for filling. Use HEX: #FF21AB</param>
        /// <returns>Stream with resulting filled image</returns>
        System.IO.Stream FillSquareImage(System.IO.Stream imageStream, string fillColor);

        /// <summary>
        /// Recolors image. Replaces all pixel with sourceColorCode with color 
        /// from targetColorCode
        /// </summary>
        /// <param name="imageStream">Stream with source image</param>
        /// <param name="sourceColorCode">Color to replace</param>
        /// <param name="targetColorCode">Replacement color</param>
        /// <returns>Stream with result image</returns>
        System.IO.Stream RecolorImage(System.IO.Stream imageStream, string sourceColorCode, string targetColorCode);

        /// <summary>
        /// Draw image from detailImageStream on top of sourceImageStream. Detail image will positionized 
        /// with information from detailImageRect.
        /// </summary>
        /// <param name="sourceImageStream">Stream with source image to draw detail on</param>
        /// <param name="detailImageStream">Stream with detail image</param>
        /// <param name="detailImageRect">Position and size of detail image on source image</param>
        /// <returns>Image with combined content</returns>
        System.IO.Stream CombineImages(System.IO.Stream sourceImageStream, System.IO.Stream detailImageStream, Rect detailImageRect = null);

        /// <summary>
        /// Draws rectangle on given source image with given coordinates from rect and color from colorcode.
        /// </summary>
        /// <param name="sourceImageStream">Stream with source image to draw rect on</param>
        /// <param name="rect">Rect with coordinates of resulting rect on image</param>
        /// <param name="rectColorCode">Color code of rectangle</param>
        /// <returns>Image with rect inside</returns>
        System.IO.Stream DrawRect(System.IO.Stream sourceImageStream, Rect rect, string rectColorCode);
    }
}
