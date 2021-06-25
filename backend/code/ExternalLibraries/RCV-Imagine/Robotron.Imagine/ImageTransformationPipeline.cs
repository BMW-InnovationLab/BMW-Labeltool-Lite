using Robotron.Imagine.Configuration;
using Robotron.Imagine.Extension;
using System;
using System.Collections.Generic;

namespace Robotron.Imagine
{
    /// <summary>
    /// Pipeline for automatically transform image.
    /// </summary>
    public class ImageTransformationPipeline
    {
        #region member

        private IEnumerable<ImageTransformationConfiguration> ImageTransformationConfigurations { get; set; }

        #endregion

        #region constructor

        /// <summary>
        /// Craetes a new instance of image transformation pipeline.
        /// </summary>
        /// <param name="imageTransformationConfigurations">Collection of transformtion configurations</param>
        public ImageTransformationPipeline(IEnumerable<ImageTransformationConfiguration> imageTransformationConfigurations)
        {
            #region validation

            if (imageTransformationConfigurations == null)
            {
                throw new ArgumentNullException(nameof(imageTransformationConfigurations));
            }

            #endregion

            ImageTransformationConfigurations = imageTransformationConfigurations;
        }

        #endregion

        #region IImageTransformationPipeline

        /// <summary>
        /// Transforms image stream with given parameters.
        /// </summary>
        /// <param name="imageStream">Image as stream to transform</param>
        /// <returns>New stream with transformed image</returns>
        public System.IO.Stream TransformImage(System.IO.Stream imageStream)
        {
            #region validation

            if (imageStream == null)
            {
                throw new ArgumentNullException(nameof(imageStream));
            }

            #endregion

            // rewind stream to begin
            imageStream.Rewind();

            // craete image service instance
            ImageService imageService = new ImageService();

            foreach (ImageTransformationConfiguration transformation in ImageTransformationConfigurations)
            {
                switch (transformation.TransformationMode)
                {
                    case EImageTransformationMode.None:
                        // no tranformation needed. Skip.
                        break;
                    case EImageTransformationMode.Crop:
                        // crop image
                        imageStream = imageService.CropSquareImage(imageStream);
                        break;
                    case EImageTransformationMode.Fill:
                        // fill image with color
                        imageStream = imageService.FillImage(imageStream, transformation.FillColor, transformation.FillWidth, transformation.FillHeight);
                        break;
                    case EImageTransformationMode.Scale:
                        // scale image to desired size
                        imageStream = imageService.ResizeImage(imageStream, transformation.ScaleWidth, transformation.ScaleHeight);
                        break;
                }
            }
            return imageStream.CopyStream();
        }

        #endregion
    }
}
