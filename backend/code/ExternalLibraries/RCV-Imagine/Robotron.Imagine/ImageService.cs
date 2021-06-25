using Robotron.Imagine.Extension;
using Robotron.Imagine.Model;
using Robotron.Imagine.Util;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;

namespace Robotron.Imagine
{
    /// <summary>
    /// Implementation of image service.
    /// </summary>
    public class ImageService : IImageService
    {
        /// <inheritdoc />
        public System.IO.Stream GetImageDetail(System.IO.Stream imageStream, int x, int y, int width, int heigth)
        {
            #region validation

            if (imageStream == null)
            {
                throw new ArgumentNullException(nameof(imageStream));
            }

            #endregion

            using (Image<Rgba32> originalImage = LoadImageFromStream<Rgba32>(imageStream, out IImageFormat imageFormat))
            {
                // sometimes bounding boxes will be larger then image.
                // so we will shrink size of bounding box until they are matching our requirements

                if (x < 0)
                {
                    x = 0;
                }
                if (y < 0)
                {
                    y = 0;
                }
                if (x + width > originalImage.Width)
                {
                    width = originalImage.Width - x;
                }
                if (y + heigth > originalImage.Height)
                {
                    heigth = originalImage.Height - y;
                }

                Rectangle boundingBox = new Rectangle()
                {
                    X = x,
                    Y = y,
                    Width = width,
                    Height = heigth
                };

                using (Image<Rgba32> targetImage = originalImage.Clone())
                {
                    targetImage.Mutate(ctx => ctx.Crop(boundingBox));

                    return SaveAsStream(targetImage, imageFormat);
                }
            }
        }

        /// <inheritdoc />
        public System.IO.Stream ResizeImage(System.IO.Stream imageStream, int? targetImageWidth, int? targetImageHeight)
        {
            #region validation

            if (imageStream == null)
            {
                throw new ArgumentNullException(nameof(imageStream));
            }

            if (!targetImageHeight.HasValue && !targetImageWidth.HasValue)
            {
                throw new ArgumentNullException($"Either {nameof(targetImageHeight)} or {nameof(targetImageWidth)} must be provided!");
            }

            #endregion

            // both parameters (height and width) are given
            // no scaling has to be calculated
            int targetHeight = targetImageHeight ?? 0;
            int targetWidth = targetImageWidth ?? 0;

            // get original image dimensions
            Rect imageDimension = ImageUtil.GetDimensionFromImage(imageStream);

            if (!targetImageHeight.HasValue)
            {
                // only width of target is given
                // calculate height as relation
                double scalingFactor = (double)targetWidth / (double)imageDimension.Width;
                targetHeight = (int)(scalingFactor * imageDimension.Height);
            }
            else if (!targetImageWidth.HasValue)
            {
                // only height of target is given
                // calculate width as relation
                double scalingFactor = (double)targetHeight / (double)imageDimension.Height;
                targetWidth = (int)(scalingFactor * imageDimension.Width);
            }

            // create parameter for resizing image
            ResizeOptions resizeOptions = new ResizeOptions()
            {
                Size = new Size(targetWidth, targetHeight),
                Mode = ResizeMode.Stretch
            };

            using (Image<Rgba32> originalImage = LoadImageFromStream<Rgba32>(imageStream, out IImageFormat imageFormat))
            using (Image<Rgba32> targetImage = originalImage.Clone(ctx => ctx.Resize(resizeOptions)))
            {
                return SaveAsStream(targetImage, imageFormat);
            }
        }

        /// <inheritdoc />
        public System.IO.Stream CropSquareImage(System.IO.Stream imageStream)
        {
            #region validation

            if (imageStream == null)
            {
                throw new ArgumentNullException(nameof(imageStream));
            }

            #endregion

            // get original size of image
            Rect imageDimension = ImageUtil.GetDimensionFromImage(imageStream);

            if (imageDimension.IsHorizontalFormat())
            {
                // width is greater then height --> height is leading information
                // returns image with dimension: height*height
                // left and right will be cropped
                return GetImageDetail(imageStream, (imageDimension.Width - imageDimension.Height) / 2, 0, imageDimension.Height, imageDimension.Height);
            }
            else if (imageDimension.IsVerticalFormat())
            {
                // height is greater then width --> width is leading information
                // return image with dimension: width*width
                // top and bottom will be cropped
                return GetImageDetail(imageStream, 0, (imageDimension.Height - imageDimension.Width) / 2, imageDimension.Width, imageDimension.Width);
            }

            // height and width are equal
            // image will be returned as copy
            return imageStream.CopyStream();
        }

        /// <inheritdoc />
        public System.IO.Stream RotateImage(System.IO.Stream imageStream, double angle)
        {
            #region validation

            if (imageStream == null)
            {
                throw new ArgumentNullException(nameof(imageStream));
            }

            #endregion

            // angle outside interval 0 to 360 degrees is scaled by modulo
            angle %= 360;

            using (Image<Rgba32> originalImage = LoadImageFromStream<Rgba32>(imageStream, out IImageFormat imageFormat))
            {
                originalImage.Mutate(c => c.Rotate((float)angle));
                return SaveAsStream(originalImage, imageFormat);
            }
        }

        /// <inheritdoc />
        public System.IO.Stream FillSquareImage(System.IO.Stream imageStream, string fillColor)
        {
            Rect imageDimension = ImageUtil.GetDimensionFromImage(imageStream);

            if (imageDimension.IsSquare())
            {
                // height and width are equal
                // image will be returned as copy
                return imageStream.CopyStream();
            }

            int squareSize = Math.Max(imageDimension.Height, imageDimension.Width);
            return FillImage(imageStream, fillColor, squareSize, squareSize);
        }

        /// <inheritdoc />
        public System.IO.Stream FillImage(System.IO.Stream imageStream, string fillColor, int? targetImageWidth, int? targetImageHeight)
        {
            #region validation

            if (imageStream == null)
            {
                throw new ArgumentNullException(nameof(imageStream));
            }

            if (string.IsNullOrEmpty(fillColor))
            {
                throw new ArgumentNullException(nameof(fillColor));
            }

            Regex regex = HexColorUtil.GetHexColorRegex();
            if (!regex.IsMatch(fillColor))
            {
                throw new ArgumentException($"Color code {fillColor} is malformed!");
            }

            #endregion

            if (!targetImageWidth.HasValue || !targetImageHeight.HasValue)
            {
                return FillSquareImage(imageStream, fillColor);
            }

            // targetWidth or targetHeight has to be original size, because of positioning image into borders

            // get original size of image
            Rect imageDimension = ImageUtil.GetDimensionFromImage(imageStream);

            int targetWidth = imageDimension.Width;
            int targetHeight = imageDimension.Height;

            double scalingFactorWidth = (double)targetImageWidth / (double)imageDimension.Width;
            double scalingFactorHeight = (double)targetImageHeight / (double)imageDimension.Height;

            if (scalingFactorHeight == scalingFactorWidth)
            {
                // quadratic
            }
            else if (scalingFactorWidth > scalingFactorHeight)
            {
                targetWidth = (int)((double)targetImageWidth / (double)targetImageHeight * imageDimension.Height);
            }
            else
            {
                targetHeight = (int)((double)targetImageHeight / (double)targetImageWidth * imageDimension.Width);
            }

            // create parameter for resizing image
            ResizeOptions resizeOptions = new ResizeOptions()
            {
                Size = new Size(targetWidth, targetHeight),
                Mode = ResizeMode.BoxPad
            };

            Rgba32 backgroundColor = Rgba32Util.InitFromHex(fillColor);

            using (Image<Rgba32> originalImage = LoadImageFromStream<Rgba32>(imageStream, out IImageFormat imageFormat))
            using (Image<Rgba32> targetImage = originalImage.Clone(ctx => ctx.Resize(resizeOptions)))
            {
                targetImage.Mutate(x => x.BackgroundColor(backgroundColor));

                return SaveAsStream(targetImage, imageFormat);
            }
        }

        /// <inheritdoc />
        public System.IO.Stream FillPolygon(System.IO.Stream sourceImageStream, string polygonColorCode, Polygon polygon)
        {
            #region validation

            if (sourceImageStream == null)
            {
                throw new ArgumentNullException(nameof(sourceImageStream));
            }

            if (string.IsNullOrEmpty(polygonColorCode))
            {
                throw new ArgumentNullException(nameof(polygonColorCode));
            }

            Regex regex = HexColorUtil.GetHexColorRegex();
            if (!regex.IsMatch(polygonColorCode))
            {
                throw new ArgumentException($"Color code {polygonColorCode } is malformed!");
            }

            if (polygon == null)
            {
                throw new ArgumentNullException(nameof(polygon));
            }

            if (polygon.Points == null)
            {
                throw new ArgumentException(nameof(polygon.Points));
            }

            if (polygon.Points.Count() < 2)
            {
                throw new ArgumentException($"Polygon {nameof(polygon)} has to contain more than one entry!");
            }

            #endregion

            IBrush brush = new SolidBrush(Rgba32Util.InitFromHex(polygonColorCode));

            ShapeGraphicsOptions graphicsOptions = new ShapeGraphicsOptions()
            {
                GraphicsOptions = new GraphicsOptions()
                {
                    ColorBlendingMode = PixelColorBlendingMode.Normal
                }
            };

            using (Image<Rgba32> originalImage = LoadImageFromStream<Rgba32>(sourceImageStream, out IImageFormat imageFormat))
            {
                originalImage.Mutate(c => c.FillPolygon(graphicsOptions, brush, polygon.Points.Select(p => new PointF(p.X, p.Y)).ToArray()));

                return SaveAsStream(originalImage, imageFormat);
            }
        }

        /// <inheritdoc />
        public System.IO.Stream RecolorImage(System.IO.Stream imageStream, string sourceColorCode, string targetColorCode)
        {
            #region validation

            if (imageStream == null)
            {
                throw new ArgumentNullException(nameof(imageStream));
            }

            if (string.IsNullOrEmpty(sourceColorCode))
            {
                throw new ArgumentNullException(nameof(sourceColorCode));
            }

            if (string.IsNullOrEmpty(targetColorCode))
            {
                throw new ArgumentNullException(nameof(targetColorCode));
            }

            Regex regex = HexColorUtil.GetHexColorRegex();
            if (!regex.IsMatch(sourceColorCode))
            {
                throw new ArgumentException($"Color code {sourceColorCode} is malformed!");
            }

            if (!regex.IsMatch(targetColorCode))
            {
                throw new ArgumentException($"Color code {targetColorCode} is malformed!");
            }

            #endregion

            Rgba32 sourceColor = Rgba32Util.InitFromHex(sourceColorCode);
            Rgba32 targetColor = Rgba32Util.InitFromHex(targetColorCode);
            float threshold = 0.1f;

            // RecolorBrush recolorBrush = new RecolorBrush(sourceColor, targetColor, threshold);

            using (Image<Rgba32> originalImage = LoadImageFromStream<Rgba32>(imageStream, out IImageFormat imageFormat))
            using (Image<Rgba32> targetImage = originalImage.Clone())
            {
                // fill does not repace correctly. transparent colors will not be generated to image
                // targetImage.Mutate(x => x.Fill(recolorBrush));

                var rmin = sourceColor.R - (sourceColor.R * threshold);
                var rmax = sourceColor.R + (sourceColor.R * threshold);
                var gmin = sourceColor.G - (sourceColor.G * threshold);
                var gmax = sourceColor.G + (sourceColor.G * threshold);
                var bmin = sourceColor.B - (sourceColor.B * threshold);
                var bmax = sourceColor.B + (sourceColor.B * threshold);

                for (int x = 0; x < targetImage.Width; x++)
                {
                    for (int y = 0; y < targetImage.Height; y++)
                    {
                        var pixel = targetImage[x, y];
                        if (pixel.R > rmin && pixel.R < rmax && pixel.G > gmin && pixel.G < gmax && pixel.B > bmin && pixel.B < bmax)
                        {
                            targetImage[x, y] = targetColor;
                        }
                    }
                }

                return SaveAsStream(targetImage, imageFormat);
            }
        }

        /// <inheritdoc/>
        public System.IO.Stream CombineImages(System.IO.Stream sourceImageStream, System.IO.Stream detailImageStream, Rect detailImageRect, double angle = 0)
        {
            #region validation

            if (sourceImageStream == null)
            {
                throw new ArgumentNullException(nameof(sourceImageStream));
            }

            if (detailImageStream == null)
            {
                throw new ArgumentNullException(nameof(detailImageStream));
            }

            #endregion

            using Image<Rgba32> sourceImage = LoadImageFromStream<Rgba32>(sourceImageStream, out IImageFormat imageFormat);
            using Image<Rgba32> detailImage = LoadImageFromStream<Rgba32>(angle != 0 ? RotateImage(detailImageStream, angle) : detailImageStream, out IImageFormat detailImageFormat);
            var detailImagePosition = new SixLabors.ImageSharp.Point(0, 0);
            if (detailImageRect != null)
            {
                Size detailImageSize = new Size(detailImageRect.Width, detailImageRect.Height);
                detailImage.Mutate(x => x.Resize(detailImageSize));

                detailImagePosition = new SixLabors.ImageSharp.Point(detailImageRect.X, detailImageRect.Y);
            }

            sourceImage.Mutate(x => x.DrawImage(detailImage, detailImagePosition, 1.0f));

            return SaveAsStream(sourceImage, imageFormat);
        }

        /// <inheritdoc />
        public System.IO.Stream DrawRect(System.IO.Stream sourceImageStream, Rect rect, string rectColorCode)
        {
            #region validation

            if (sourceImageStream == null)
            {
                throw new ArgumentNullException(nameof(sourceImageStream));
            }

            if (rect == null)
            {
                throw new ArgumentNullException(nameof(rect));
            }

            Regex regex = HexColorUtil.GetHexColorRegex();
            if (!regex.IsMatch(rectColorCode))
            {
                throw new ArgumentException($"Color code {rectColorCode} is malformed!");
            }

            #endregion

            List<DrawableRect> drawableRects = new List<DrawableRect>
            {
                new DrawableRect()
                {
                    BorderColorCode = rectColorCode,
                    X = rect.X,
                    Y = rect.Y,
                    Width = rect.Width,
                    Height = rect.Height
                }
            };

            return DrawRects(sourceImageStream, drawableRects);
        }

        /// <inheritdoc />
        public System.IO.Stream DrawRects(System.IO.Stream sourceImageStream, IEnumerable<DrawableRect> rects)
        {
            #region validation

            if (sourceImageStream == null)
            {
                throw new ArgumentNullException(nameof(sourceImageStream));
            }

            if (rects == null)
            {
                throw new ArgumentNullException(nameof(rects));
            }

            Regex regex = HexColorUtil.GetHexColorRegex();
            foreach (DrawableRect rect in rects)
            {
                if (!regex.IsMatch(rect.BorderColorCode))
                {
                    throw new ArgumentException($"Color code {rect.BorderColorCode} is malformed!");
                }
            }

            #endregion

            using (Image<Rgba32> sourceImage = LoadImageFromStream<Rgba32>(sourceImageStream, out IImageFormat imageFormat))
            {
                foreach (DrawableRect rect in rects)
                {
                    Rgba32 rectColor = Rgba32Util.InitFromHex(rect.BorderColorCode);
                    SolidBrush brush = new SolidBrush(rectColor);

                    sourceImage.Mutate(x => x.DrawPolygon(brush, rect.BorderThickness, new PointF[] {
                        new Vector2(rect.X, rect.Y),
                        new Vector2(rect.X + rect.Width, rect.Y),
                        new Vector2(rect.X + rect.Width, rect.Y + rect.Height),
                        new Vector2(rect.X, rect.Y + rect.Height)
                    }));
                }

                return SaveAsStream(sourceImage, imageFormat);
            }
        }

        /// <inheritdoc />
        public System.IO.Stream DrawCuboids(System.IO.Stream sourceImageStream, IEnumerable<DrawableCuboid> cuboids)
        {
            #region validation

            if (sourceImageStream == null)
            {
                throw new ArgumentNullException(nameof(sourceImageStream));
            }

            if (cuboids == null)
            {
                throw new ArgumentNullException(nameof(cuboids));
            }

            Regex regex = HexColorUtil.GetHexColorRegex();
            foreach (DrawableCuboid rect in cuboids)
            {
                if (!regex.IsMatch(rect.BorderColorCode))
                {
                    throw new ArgumentException($"Color code {rect.BorderColorCode} is malformed!");
                }
            }

            #endregion

            using (Image<Rgba32> sourceImage = LoadImageFromStream<Rgba32>(sourceImageStream, out IImageFormat imageFormat))
            {
                foreach (DrawableCuboid cuboid in cuboids)
                {
                    Rgba32 rectColor = Rgba32Util.InitFromHex(cuboid.BorderColorCode);
                    SolidBrush brush = new SolidBrush(rectColor);

                    Rectangle background = new Rectangle(cuboid.XBackground, cuboid.YBackground, cuboid.WidthBackground, cuboid.HeightBackground);
                    Rectangle foreground = new Rectangle(cuboid.X, cuboid.Y, cuboid.Width, cuboid.Height);

                    Rgba32 backgroundColor = rectColor;
                    backgroundColor.A = (byte)(255 * 0.2);

                    // draw foreground
                    sourceImage.Mutate(x => x.FillPolygon(backgroundColor, new PointF[] {
                        new Vector2(cuboid.X, cuboid.Y),
                        new Vector2(cuboid.X + cuboid.Width, cuboid.Y),
                        new Vector2(cuboid.X + cuboid.Width, cuboid.Y + cuboid.Height),
                        new Vector2(cuboid.X, cuboid.Y + cuboid.Height)
                    }));
                    sourceImage.Mutate(x => x.DrawPolygon(brush, cuboid.BorderThickness, new PointF[] {
                        new Vector2(cuboid.X, cuboid.Y),
                        new Vector2(cuboid.X + cuboid.Width, cuboid.Y),
                        new Vector2(cuboid.X + cuboid.Width, cuboid.Y + cuboid.Height),
                        new Vector2(cuboid.X, cuboid.Y + cuboid.Height)
                    }));

                    SolidBrush backgroundBrush = new SolidBrush(backgroundColor);

                    // draw background
                    // upper line
                    if (cuboid.Y >= cuboid.YBackground)
                    {
                        sourceImage.Mutate(x => x.DrawLines(backgroundBrush, cuboid.BorderThickness, new PointF[] {
                            new Vector2(cuboid.XBackground, cuboid.YBackground),
                            new Vector2(cuboid.XBackground + cuboid.WidthBackground, cuboid.YBackground)
                        }));
                    }

                    // left line
                    if (cuboid.X >= cuboid.XBackground)
                    {
                        sourceImage.Mutate(x => x.DrawLines(backgroundBrush, cuboid.BorderThickness, new PointF[] {
                            new Vector2(cuboid.XBackground, cuboid.YBackground),
                            new Vector2(cuboid.XBackground, cuboid.YBackground + cuboid.HeightBackground)
                        }));
                    }

                    // lower line
                    if (cuboid.Y + cuboid.Height <= cuboid.YBackground + cuboid.HeightBackground)
                    {
                        sourceImage.Mutate(x => x.DrawLines(backgroundBrush, cuboid.BorderThickness, new PointF[] {
                            new Vector2(cuboid.XBackground + cuboid.WidthBackground, cuboid.YBackground + cuboid.HeightBackground),
                            new Vector2(cuboid.XBackground, cuboid.YBackground + cuboid.HeightBackground)
                        }));
                    }

                    // right line
                    if (cuboid.X + cuboid.Width <= cuboid.XBackground + cuboid.WidthBackground)
                    {
                        sourceImage.Mutate(x => x.DrawLines(backgroundBrush, cuboid.BorderThickness, new PointF[] {
                            new Vector2(cuboid.XBackground + cuboid.WidthBackground, cuboid.YBackground),
                            new Vector2(cuboid.XBackground + cuboid.WidthBackground, cuboid.YBackground + cuboid.HeightBackground)
                        }));
                    }


                    // connect upper left corners if not hidden by foreground
                    if (foreground.Top > background.Top || foreground.Left > background.Left)
                    {
                        sourceImage.Mutate(x => x.DrawLines(backgroundBrush, cuboid.BorderThickness, new PointF[] {
                            new Vector2(cuboid.X, cuboid.Y),
                            new Vector2(cuboid.XBackground, cuboid.YBackground)
                        }));
                    }

                    // connect upper right corners if not hidden by foreground
                    if (foreground.Top > background.Top || foreground.Right < background.Right)
                    {
                        sourceImage.Mutate(x => x.DrawLines(backgroundBrush, cuboid.BorderThickness, new PointF[] {
                            new Vector2(cuboid.X + cuboid.Width, cuboid.Y),
                            new Vector2(cuboid.XBackground + cuboid.WidthBackground, cuboid.YBackground)
                        }));
                    }

                    // connect lower left corners if not hidden by foreground
                    if (foreground.Bottom < background.Bottom || foreground.Left > background.Left)
                    {
                        sourceImage.Mutate(x => x.DrawLines(backgroundBrush, cuboid.BorderThickness, new PointF[] {
                            new Vector2(cuboid.X, cuboid.Y + cuboid.Height),
                            new Vector2(cuboid.XBackground, cuboid.YBackground + cuboid.HeightBackground)
                        }));
                    }


                    // connect lower right corners if not hidden by foreground
                    if (foreground.Bottom < background.Bottom || foreground.Right < background.Right)
                    {
                        sourceImage.Mutate(x => x.DrawLines(backgroundBrush, cuboid.BorderThickness, new PointF[] {
                            new Vector2(cuboid.X + cuboid.Width, cuboid.Y + cuboid.Height),
                            new Vector2(cuboid.XBackground + cuboid.WidthBackground, cuboid.YBackground + cuboid.HeightBackground)
                        }));
                    }
                }

                return SaveAsStream(sourceImage, imageFormat);
            }
        }

        /// <inheritdoc />
        public System.IO.Stream DrawObjects(System.IO.Stream sourceImageStream, IEnumerable<IDrawable> objects)
        {
            #region validation

            if (sourceImageStream == null)
            {
                throw new ArgumentNullException(nameof(sourceImageStream));
            }

            if (objects == null)
            {
                throw new ArgumentNullException(nameof(objects));
            }

            #endregion

            using (Image<Rgba32> sourceImage = LoadImageFromStream<Rgba32>(sourceImageStream, out IImageFormat imageFormat))
            {
                foreach (IDrawable drawable in objects)
                {
                    drawable.Draw(sourceImage);
                }

                return SaveAsStream(sourceImage, imageFormat);
            }
        }

        /// <inheritdoc />
        public System.IO.Stream ConvertToPng(System.IO.Stream sourceImageStream)
        {
            #region validation

            if (sourceImageStream == null)
            {
                throw new ArgumentNullException(nameof(sourceImageStream));
            }

            #endregion

            using (Image<Rgba32> image = LoadImageFromStream<Rgba32>(sourceImageStream, out IImageFormat imageFormat))
            {
                return SaveAsStream(image, PngFormat.Instance);
            }
        }

        /// <inheritdoc />
        public System.IO.Stream RemoveAlphaChannel(System.IO.Stream sourceImageStream)
        {
            #region validation

            if (sourceImageStream == null)
            {
                throw new ArgumentNullException(nameof(sourceImageStream));
            }

            #endregion

            using (Image<Rgb24> image = LoadImageFromStream<Rgb24>(sourceImageStream, out IImageFormat imageFormat))
            {
                return SaveAsStream(image, imageFormat);
            }
        }

        /// <inheritdoc />
        public System.IO.Stream CreateImage(Rect dimension, string backgroundColorCode)
        {
            #region validation

            if (dimension == null)
            {
                throw new ArgumentNullException(nameof(dimension));

            }

            Regex regex = HexColorUtil.GetHexColorRegex();
            if (!regex.IsMatch(backgroundColorCode))
            {
                throw new ArgumentException($"Color code {backgroundColorCode} is malformed!");
            }

            #endregion

            using (Image<Rgba32> image = new Image<Rgba32>(SixLabors.ImageSharp.Configuration.Default, dimension.Width, dimension.Height, Rgba32Util.InitFromHex(backgroundColorCode)))
            {
                return SaveAsStream(image, PngFormat.Instance);
            }
        }

        /// <inheritdoc />
        public System.IO.Stream ColorizePixel(System.IO.Stream imageStream, GetPixelColor getPixelColor)
        {
            #region validation

            if (imageStream == null)
            {
                throw new ArgumentNullException(nameof(imageStream));
            }

            if (getPixelColor == null)
            {
                throw new ArgumentNullException(nameof(getPixelColor));
            }

            #endregion

            // create map for colorcode-representation
            // stores already translated HEX-colors
            Dictionary<string, Rgba32> colorMap = new Dictionary<string, Rgba32>();

            using (Image<Rgba32> image = LoadImageFromStream<Rgba32>(imageStream, out IImageFormat imageFormat))
            {
                // iterate over all pixels of image
                for (int x = 0; x < image.Width; x++)
                {
                    for (int y = 0; y < image.Height; y++)
                    {
                        // get color for pixel
                        // if null, background color should be used
                        string pixelColor = getPixelColor(x, y);
                        if (!string.IsNullOrEmpty(pixelColor))
                        {
                            if (!colorMap.ContainsKey(pixelColor))
                            {
                                colorMap.Add(pixelColor, new Rgba32(HexColorUtil.GetR(pixelColor), HexColorUtil.GetG(pixelColor), HexColorUtil.GetB(pixelColor), HexColorUtil.GetA(pixelColor)));
                            }

                            image[x, y] = colorMap[pixelColor];
                        }
                    }
                }

                return SaveAsStream(image, imageFormat);
            }
        }

        /// <inheritdoc />
        public System.IO.Stream CreateImageMask(System.IO.Stream sourceImageStream, string maskColorCode)
        {
            #region validation

            if (sourceImageStream == null)
            {
                throw new ArgumentNullException(nameof(sourceImageStream));
            }

            Regex regex = HexColorUtil.GetHexColorRegex();
            if (!regex.IsMatch(maskColorCode))
            {
                throw new ArgumentException($"Color code {maskColorCode} is malformed!");
            }

            #endregion

            // create solid color brush to draw mask
            IBrush brush = new SolidBrush(Rgba32Util.InitFromHex(maskColorCode));

            // create graphic options for overlay of alpha-channel-image with solid color brush
            //   non-transparent pixels of image should be overlayed with solid color brush
            GraphicsOptions graphicsOptions = new GraphicsOptions()
            {
                AlphaCompositionMode = PixelAlphaCompositionMode.SrcIn,
                ColorBlendingMode = PixelColorBlendingMode.Normal
            };

            using (Image<Rgba32> originalImage = LoadImageFromStream<Rgba32>(sourceImageStream, out IImageFormat imageFormat))
            {
                originalImage.Mutate(x => x.Fill(graphicsOptions, brush));
                return SaveAsStream(originalImage, imageFormat);
            }
        }

        /// <inheritdoc />
        public double GetDepth(System.IO.Stream sourceImageStream, Rect depthArea)
        {
            #region validation

            if (sourceImageStream == null)
            {
                throw new ArgumentNullException(nameof(sourceImageStream));
            }

            if (depthArea == null)
            {
                throw new ArgumentNullException(nameof(depthArea));
            }

            #endregion

            List<ushort> depths = new List<ushort>();
            // depth is stored in images with 16-bit depth.
            // each pixel describes the distance from the lense to the object
            using (Image<L16> depthImage = LoadImageFromStream<L16>(sourceImageStream, out IImageFormat imageFormat))
            {
                for (int x = depthArea.X; x < depthArea.X + depthArea.Width; x++)
                {
                    for (int y = depthArea.Y; y < depthArea.Y + depthArea.Height; y++)
                    {
                        depths.Add(depthImage[x, y].PackedValue);
                    }
                }
            }

            // convert result to meters (given in milimeters)
            return depths.Average(o => o) * 0.001;
        }

        /// <inheritdoc />
        public System.IO.Stream AutoOrientImage(System.IO.Stream sourceImageStream)
        {
            #region validation

            if (sourceImageStream == null)
            {
                throw new ArgumentNullException(nameof(sourceImageStream));
            }

            #endregion

            using (Image<Rgba32> originalImage = LoadImageFromStream<Rgba32>(sourceImageStream, out IImageFormat imageFormat))
            {
                originalImage.Mutate(x => x.AutoOrient());
                return SaveAsStream(originalImage, imageFormat);
            }
        }

        #region private helper

        /// <summary>
        /// Loads image from given stream.
        /// </summary>
        /// <typeparam name="TPixel">Pixeltype of image to load</typeparam>
        /// <param name="imageStream">Imagestream to load image from</param>
        /// <param name="imageFormat">Format of image which has been loaded</param>
        /// <returns>Loaded image from stream</returns>
        private Image<TPixel> LoadImageFromStream<TPixel>(System.IO.Stream imageStream, out IImageFormat imageFormat) where TPixel : unmanaged, IPixel<TPixel>
        {
            // rewind stream to begin
            imageStream.Rewind();
            // load image from stream
            return SixLabors.ImageSharp.Image.Load<TPixel>(imageStream, out imageFormat);
        }

        /// <summary>
        /// Save image as memory stream.
        /// </summary>
        /// <typeparam name="TPixel">Pixeltype of image to save</typeparam>
        /// <param name="image">Image to store as memory stream</param>
        /// <param name="imageFormat">Format of image which was loaded</param>
        /// <returns>Stream from image</returns>
        private System.IO.Stream SaveAsStream<TPixel>(Image<TPixel> image, IImageFormat imageFormat) where TPixel : unmanaged, IPixel<TPixel>
        {
            MemoryStream memoryStream = new MemoryStream();
            image.Save(memoryStream, imageFormat);

            memoryStream.Rewind();
            return memoryStream;
        }

        #endregion
    }
}
