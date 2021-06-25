using Rcv.LabelTool.Contracts.Models;
using Rcv.LabelTool.Contracts.Repositories;
using Rcv.LabelTool.Contracts.Services;
using Rcv.LabelTool.Logic.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rcv.LabelTool.Web.Services
{
    /// <summary>
    /// Service defintion for images from file-system.
    /// </summary>
    public class ImageService : IImageService
    {
        #region members

        /// <summary>
        /// Repository for management of images.
        /// </summary>
        private IImageDataRepository ImageRepository { get; }

        private IImageFileRepository ImageFileRepository { get; }

        private ILabelService LabelService { get; }

        #endregion

        #region constructor

        public ImageService(IImageDataRepository imageRepository, IImageFileRepository imageFileRepository, ILabelService labelService)
        {
            ImageRepository = imageRepository ?? throw new ArgumentNullException(nameof(imageRepository));
            ImageFileRepository = imageFileRepository ?? throw new ArgumentNullException(nameof(imageFileRepository));
            LabelService = labelService ?? throw new ArgumentNullException(nameof(labelService));
        }

        #endregion

        public uint ImageCount(Topic topic)
        {
            #region valiation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic), "Parameter is mandatory but it is missing.");
            }

            #endregion

            return ImageRepository.CountAll(topic);
        }

        public void DeleteImage(Topic topic, string imageFileName)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            if (string.IsNullOrEmpty(imageFileName))
            {
                throw new ArgumentNullException(nameof(imageFileName));
            }

            #endregion

            ImageLabel imageLabel = CreateTemporaryImageLabel(topic, imageFileName);

            // remove all labels            
            LabelService.DeleteLabels(topic, imageLabel);

            // remove image
            ImageFileRepository.DeleteImageFile(topic, imageFileName);
            ImageRepository.DeleteImage(topic, imageFileName);
        }

        public Stream GetImage(Topic topic, string imageFileName)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            if (string.IsNullOrEmpty(imageFileName))
            {
                throw new ArgumentNullException(nameof(imageFileName));
            }

            #endregion

            return ImageFileRepository.GetImageFile(topic, imageFileName);
        }

        public IList<ImageLabel> GetImageLabels(Topic topic)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            #endregion

            return ImageRepository.GetImages(topic);
        }

        public bool ExistsImage(Topic topic, string imageName)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            if (string.IsNullOrEmpty(imageName))
            {
                throw new ArgumentNullException(nameof(imageName));
            }

            #endregion

            return ImageRepository.ExistsImage(topic, imageName);
        }

        public ImageLabel AddImage(Topic topic, string imageName, System.IO.Stream imageStream)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            if (string.IsNullOrEmpty(imageName))
            {
                throw new ArgumentNullException(nameof(imageName));
            }

            if (imageStream == null)
            {
                throw new ArgumentNullException(nameof(imageStream));
            }

            #endregion

            // check if image isn't corrupted
            // try to open image
            ImageUtil.TryOpenImage(imageStream);

            string[] topicImageFileName = ImageFileRepository.AddImageFile(topic, imageName, imageStream);

            Rect imageDimension = ImageUtil.GetDimensionFromImage(imageStream);

            ImageLabel imageLabel = new ImageLabel()
            {
                Id = topicImageFileName.Last(),
                Height = imageDimension.Height,
                Width = imageDimension.Width,
            };

            return ImageRepository.AddImage(topic, imageLabel);
        }

        public ImageLabel GetImageLabel(Topic topic, string imageId)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            if (string.IsNullOrEmpty(imageId))
            {
                throw new ArgumentNullException(nameof(imageId));
            }

            #endregion

            return ImageRepository.GetImageLabelById(topic, imageId);
        }

        #region helper functions

        private ImageLabel CreateTemporaryImageLabel(Topic topic, string imageFileName)
        {
            return ImageRepository.GetImageLabelById(topic, imageFileName);
        }

        #endregion
    }
}