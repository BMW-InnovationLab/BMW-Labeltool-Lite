using Rcv.LabelTool.Contracts.Models;
using Rcv.LabelTool.Contracts.Repositories;
using Rcv.LabelTool.Contracts.Services;
using System;

namespace Rcv.LabelTool.Services.Services
{
    public class ImageDataService : IImageDataService
    {
        #region members

        /// <summary>
        /// Repository for management of images.
        /// </summary>
        private IImageDataRepository ImageRepository { get; }

        #endregion

        #region constructor

        public ImageDataService(IImageDataRepository imageRepository)
        {
            ImageRepository = imageRepository ?? throw new ArgumentNullException(nameof(imageRepository));
        }

        # endregion

        public ImageData GetImageData(Topic topic, string imageId)
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

            ImageLabel imageLabel = ImageRepository.GetImageLabelById(topic, imageId);

            return CreateImageData(imageLabel);
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

        #region private helper

        private static ImageData CreateImageData(ImageLabel imageLabel)
        {
            ImageData result = new ImageData()
            {
                Id = imageLabel.Id,
                Width = imageLabel.Width,
                Height = imageLabel.Height
            };

            return result;
        }

        #endregion private helper
    }
}
