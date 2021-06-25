using Robotron.Imagine.Model;
using System.Collections.Generic;

namespace Robotron.Imagine
{
    /// <summary>
    /// Delegate to get pixel color in HEX-notation for specified coordinates.
    /// </summary>
    /// <param name="x">X-Coordinate of pixel</param>
    /// <param name="y">Y-Coordinate of pixel</param>
    /// <returns>Color code for specified pixel, or NULL if background color should be used</returns>
    public delegate string GetPixelColor(int x, int y);

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
        /// Returns rotated image as stream.
        /// Rotates image clockwise by given angle in degrees.
        /// </summary>
        /// <param name="imageStream">Stream with source image</param>
        /// <param name="angle">Angle of rotation in degrees</param>
        /// <returns>Stream with resulting rotated image</returns>
        System.IO.Stream RotateImage(System.IO.Stream imageStream, double angle);

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
        /// <param name="angle">Angle of rotation in degrees</param>
        /// <returns>Image with combined content</returns>
        System.IO.Stream CombineImages(System.IO.Stream sourceImageStream, System.IO.Stream detailImageStream, Rect detailImageRect = null, double angle = 0);

        /// <summary>
        /// Draws rectangle on given source image with given coordinates from rect and color from colorcode.
        /// </summary>
        /// <param name="sourceImageStream">Stream with source image to draw rect on</param>
        /// <param name="rect">Rect with coordinates of resulting rect on image</param>
        /// <param name="rectColorCode">Color code of rectangle</param>
        /// <returns>Image with rect inside</returns>
        System.IO.Stream DrawRect(System.IO.Stream sourceImageStream, Rect rect, string rectColorCode);

        /// <summary>
        /// Draw list of drawable rects to image.
        /// </summary>
        /// <param name="sourceImageStream">Stream with source image to draw rects on</param>
        /// <param name="rects">List of rects to draw on image</param>
        /// <returns>Image with rects inside</returns>
        System.IO.Stream DrawRects(System.IO.Stream sourceImageStream, IEnumerable<DrawableRect> rects);

        /// <summary>
        /// Draw list of drawable cuboids to image.
        /// </summary>
        /// <param name="sourceImageStream">Stream with source image to draw cuboids on</param>
        /// <param name="cuboids">List of cuboids to draw on image</param>
        /// <returns>Image with cuboids inside</returns>
        System.IO.Stream DrawCuboids(System.IO.Stream sourceImageStream, IEnumerable<DrawableCuboid> cuboids);

        /// <summary>
        /// Draw list of objects to image.
        /// </summary>
        /// <param name="sourceImageStream">Stream with souce image to draw cuboids on</param>
        /// <param name="objects">List of objects to draw on image</param>
        /// <returns>Image with objects drawn</returns>
        System.IO.Stream DrawObjects(System.IO.Stream sourceImageStream, IEnumerable<IDrawable> objects);

        /// <summary>
        /// Convert given image stream to png image.
        /// </summary>
        /// <param name="sourceImageStream">Image stream to convert to png</param>
        /// <returns>Image converted as png</returns>
        System.IO.Stream ConvertToPng(System.IO.Stream sourceImageStream);

        /// <summary>
        /// Remove alpha channel from given image stream.
        /// </summary>
        /// <param name="sourceImageStream">Image stream where to remove alpha channel</param>
        /// <returns>Image stream without alpha channel</returns>
        System.IO.Stream RemoveAlphaChannel(System.IO.Stream sourceImageStream);

        /// <summary>
        /// Create image with given dimension and given background color code (hex-structure).
        /// </summary>
        /// <param name="dimension">Dimension of new image</param>
        /// <param name="backgroundColorCode">Background color code of image</param>
        /// <returns>Newly created image</returns>
        System.IO.Stream CreateImage(Rect dimension, string backgroundColorCode);

        /// <summary>
        /// Creates mask from given image and marks all non-transparent pixels with given
        /// maskColorCode. All transparent pixel stay transparent.
        /// </summary>
        /// <param name="sourceImageStream">Source image to get mask from</param>
        /// <param name="maskColorCode">Color code of represenation mask</param>
        /// <returns>Image with imagemask of original</returns>
        System.IO.Stream CreateImageMask(System.IO.Stream sourceImageStream, string maskColorCode);

        /// <summary>
        /// Get average depth of area defined by depthArea from source image stream.
        /// Source image stream should be a depth image.
        /// </summary>
        /// <param name="sourceImageStream">Image stream with depth information</param>
        /// <param name="depthArea">Area to get the average depth from</param>
        /// <returns>Average depth of area in meters</returns>
        double GetDepth(System.IO.Stream sourceImageStream, Rect depthArea);

        /// <summary>
        /// Colorize pixel with color which is defined by getPixelColor-method.
        /// </summary>
        /// <param name="imageStream">Stream of original image</param>
        /// <param name="getPixelColor">Method to get color of specified pixel</param>
        /// <returns>Image with colorized pixels</returns>
        System.IO.Stream ColorizePixel(System.IO.Stream imageStream, GetPixelColor getPixelColor);

        /// <summary>
        /// Fill outline of polygon with given color;
        /// </summary>
        /// <param name="sourceImageStream">Stream of original image</param>
        /// <param name="polygonColorCode">Color code of polygon</param>
        /// <param name="polygon">Outline points of polygon</param>
        /// <returns>Image with color filled polygon</returns>
        System.IO.Stream FillPolygon(System.IO.Stream sourceImageStream, string polygonColorCode, Polygon polygon);

        /// <summary>
        /// Fix orientation of image based on EXIF-data which is stored for image.
        /// The properties of the image will be corrected according to the EXIF-data.
        /// </summary>
        /// <param name="sourceImageStream">Stream of original image</param>
        /// <returns>Image which is orientated as defined in EXIF-data</returns>
        System.IO.Stream AutoOrientImage(System.IO.Stream sourceImageStream);
    }
}
