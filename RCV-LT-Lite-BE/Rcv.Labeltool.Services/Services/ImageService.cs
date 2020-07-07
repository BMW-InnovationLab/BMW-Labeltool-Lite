using Rcv.LabelTool.Contracts.Models;
using Rcv.LabelTool.Contracts.Repositories;
using Rcv.LabelTool.Contracts.Services;
using Rcv.LabelTool.Web.Utilities;
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
        private IImageRepository ImageRepository { get; }

        private ILabelService LabelService { get; }

        #endregion

        #region constructor

        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="imageRepository">ImageRepository to work with</param>
        public ImageService(IImageRepository imageRepository, ILabelService labelService)
        {
            ImageRepository = imageRepository ?? throw new ArgumentNullException(nameof(imageRepository));
            LabelService = labelService ?? throw new ArgumentNullException(nameof(labelService));
        }

        #endregion

        public ImageLabel Navigate(Topic topic, uint startPosition)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic), "Parameter is mandatory but it is missing.");
            }

            #endregion

            ImageLabel image = ImageRepository.GetImageByIndex(topic, startPosition);
            if (image != null)
            {
                // set label information
                LabelService.GetLabels(topic, image);
            }
            return image;
        }

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

        public IEnumerable<Label> UpdateImageLabel(Topic topic, ImageLabel imageLabel)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            if (imageLabel == null)
            {
                throw new ArgumentException(nameof(imageLabel));
            }

            #endregion

            return LabelService.SaveLabels(topic, imageLabel);
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

            return ImageRepository.GetImage(topic, imageFileName);
        }

        public IEnumerable<ImageLabel> GetImageLabels(Topic topic)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            #endregion

            IEnumerable<ImageLabel> result = ImageRepository.GetAll(topic);
            FillLabelAndSegmentInformation(topic, result);

            return result;
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

        /// <summary>
        /// Gets list of imagelabels with additional information about
        ///     - having labels
        ///     - having segments
        /// </summary>
        private void FillLabelAndSegmentInformation(Topic topic, IEnumerable<ImageLabel> allImages)
        {
            IEnumerable<string> labeledImageFilesWithoutExtension = LabelService.GetLabeledImageFileNames(topic);
            foreach (ImageLabel imageLabel in allImages)
            {
                imageLabel.HasLabels = labeledImageFilesWithoutExtension.Contains(FileNameUtil.GetFileNameWithoutExtension(imageLabel.Id));
            }
        }

        #endregion
    }
}