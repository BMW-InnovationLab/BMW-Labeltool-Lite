using Robotron.Imagine.Extension;
using Robotron.Imagine.Model;
using Robotron.Imagine.Util;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Numerics;
using System.Text.RegularExpressions;

namespace Robotron.Imagine
{
    public class ImageService : IImageService
    {
        public System.IO.Stream GetImageDetail(System.IO.Stream imageStream, int x, int y, int width, int heigth)
        {
            #region validation

            if (imageStream == null)
            {
                throw new ArgumentNullException(nameof(imageStream));
            }

            #endregion            

            using (Image<Rgba32> originalImage = LoadImageFromStream(imageStream))
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

                SixLabors.Primitives.Rectangle boundingBox = new SixLabors.Primitives.Rectangle()
                {
                    X = x,
                    Y = y,
                    Width = width,
                    Height = heigth
                };

                using (Image<Rgba32> targetImage = originalImage.Clone())
                {
                    targetImage.Mutate(ctx => ctx.Crop(boundingBox));

                    return SaveAsStream(targetImage);
                }
            }
        }

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
            int targetHeight = targetImageHeight.HasValue ? targetImageHeight.Value : 0;
            int targetWidth = targetImageWidth.HasValue ? targetImageWidth.Value : 0;

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
                // calculate width as releation
                double scalingFactor = (double)targetHeight / (double)imageDimension.Height;
                targetWidth = (int)(scalingFactor * imageDimension.Width);
            }

            // create parameter for resizing image
            ResizeOptions resizeOptions = new ResizeOptions()
            {
                Size = new SixLabors.Primitives.Size(targetWidth, targetHeight),
                Mode = ResizeMode.Stretch
            };

            using (Image<Rgba32> originalImage = LoadImageFromStream(imageStream))
            {
                using (Image<Rgba32> targetImage = originalImage.Clone(ctx => ctx.Resize(resizeOptions)))
                {
                    return SaveAsStream(targetImage);
                }
            }
        }

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
                // widht is greater then height --> height is leading information
                // returns image with dimension: height*height
                // left and right will be cropped
                return GetImageDetail(imageStream, (imageDimension.Width - imageDimension.Height) / 2, 0, imageDimension.Height, imageDimension.Height);
            }
            else if (imageDimension.IsVerticalFormat())
            {
                // height is greater then width --> width is leading information
                // return image with dimension: widht*width
                // top and bottom will be croped
                return GetImageDetail(imageStream, 0, (imageDimension.Height - imageDimension.Width) / 2, imageDimension.Width, imageDimension.Width);
            }

            // heigth and widht are equal
            // image will be returned as copy
            return imageStream.CopyStream();
        }

        public System.IO.Stream FillSquareImage(System.IO.Stream imageStream, string fillColor)
        {
            Rect imageDimension = ImageUtil.GetDimensionFromImage(imageStream);

            if (imageDimension.IsSquare())
            {
                // heigth and widht are equal
                // image will be returned as copy
                return imageStream.CopyStream();
            }

            int squareSize = Math.Max(imageDimension.Height, imageDimension.Width);
            return FillImage(imageStream, fillColor, squareSize, squareSize);
        }

        public System.IO.Stream FillImage(System.IO.Stream imageStream, string fillColor, int? targetImageWidth, int? targetImageHeight)
        {
            #region valiation

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
                Size = new SixLabors.Primitives.Size(targetWidth, targetHeight),
                Mode = ResizeMode.BoxPad
            };

            Rgba32 backgroundColor = Rgba32Util.InitFromHex(fillColor);

            using (Image<Rgba32> originalImage = LoadImageFromStream(imageStream))
            using (Image<Rgba32> targetImage = originalImage.Clone(ctx => ctx.Resize(resizeOptions)))
            {
                targetImage.Mutate(x => x.BackgroundColor(backgroundColor));

                return SaveAsStream(targetImage);
            }
        }

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

            RecolorBrush<Rgba32> recolorBrush = new RecolorBrush<Rgba32>(sourceColor, targetColor, 0.2f);

            using (Image<Rgba32> originalImage = LoadImageFromStream(imageStream))
            using (Image<Rgba32> targetImage = originalImage.Clone())
            {
                targetImage.Mutate(x => x.Fill(recolorBrush));

                return SaveAsStream(targetImage);
            }
        }

        public System.IO.Stream CombineImages(System.IO.Stream sourceImageStream, System.IO.Stream detailImageStream, Rect detailImageRect)
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

            using (Image<Rgba32> sourceImage = LoadImageFromStream(sourceImageStream))
            using (Image<Rgba32> detailImage = LoadImageFromStream(detailImageStream))
            {
                SixLabors.Primitives.Point detailImagePosition = new SixLabors.Primitives.Point(0, 0);
                if (detailImageRect != null)
                {
                    SixLabors.Primitives.Size detailImageSize = new SixLabors.Primitives.Size(detailImageRect.Width, detailImageRect.Height);
                    detailImage.Mutate(x => x.Resize(detailImageSize));

                    detailImagePosition = new SixLabors.Primitives.Point(detailImageRect.X, detailImageRect.Y);
                }

                sourceImage.Mutate(x => x.DrawImage(detailImage, 1.0f, detailImagePosition));

                return SaveAsStream(sourceImage);
            }
        }

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

            Rgba32 rectColor = Rgba32Util.InitFromHex(rectColorCode);

            SolidBrush<Rgba32> brush = new SolidBrush<Rgba32>(rectColor);
            using (Image<Rgba32> sourceImage = LoadImageFromStream(sourceImageStream))
            {
                sourceImage.Mutate(x => x.DrawPolygon(brush, 5.0f, new SixLabors.Primitives.PointF[] {
                    new Vector2(rect.X, rect.Y),
                    new Vector2(rect.X + rect.Width, rect.Y),
                    new Vector2(rect.X + rect.Width, rect.Y + rect.Height),
                    new Vector2(rect.X, rect.Y + rect.Height)
                 }));

                return SaveAsStream(sourceImage);
            }
        }

        public System.IO.Stream ConvertToPng(System.IO.Stream sourceImageStream)
        {
            Image<Rgba32> image = LoadImageFromStream(sourceImageStream);

            return SaveAsStream(image);
        }

        #region private helper

        /// <summary>
        /// Loads image from given stream.
        /// </summary>
        /// <param name="imageStream">Imagestream to load image from</param>
        /// <returns>Loaded image from stream</returns>
        private Image<Rgba32> LoadImageFromStream(System.IO.Stream imageStream)
        {
            // rewind stream to begin
            imageStream.Rewind();
            // load image from stream
            return SixLabors.ImageSharp.Image.Load(imageStream);
        }

        /// <summary>
        /// Save image as memory stream.
        /// </summary>
        /// <param name="image">Image to store as memory stream</param>
        /// <returns>Stream from image</returns>
        private System.IO.Stream SaveAsStream(Image<Rgba32> image)
        {
            MemoryStream memoryStream = new MemoryStream();
            image.SaveAsPng(memoryStream);

            memoryStream.Rewind();
            return memoryStream;
        }

        #endregion
    }
}
