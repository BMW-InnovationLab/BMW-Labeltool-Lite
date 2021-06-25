using Rcv.LabelTool.Contracts.Models;
using Rcv.LabelTool.Contracts.Repositories;
using Rcv.LabelTool.Contracts.Services;
using Rcv.LabelTool.Logic.Utilities;
using System;
using System.IO;
using System.Linq;

namespace Rcv.LabelTool.Services.Services
{
    /// <summary>
    /// Service defintion for images upload from file-system.
    /// </summary>
    public class ImageUploadService : IImageUploadService
    {
        #region members

        /// <summary>
        /// Repository for management of images.
        /// </summary>
        private IImageDataRepository ImageRepository { get; }

        private IImageFileRepository ImageFileRepository { get; }

        #endregion

        #region constructor

        public ImageUploadService(IImageDataRepository imageRepository, IImageFileRepository imageFileRepository)
        {
            ImageRepository = imageRepository ?? throw new ArgumentNullException(nameof(imageRepository));
            ImageFileRepository = imageFileRepository ?? throw new ArgumentNullException(nameof(imageFileRepository));
        }

        #endregion

        public ImageLabel UploadImage(Topic topic, string imageName, Stream imageStream)
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
    }
}
