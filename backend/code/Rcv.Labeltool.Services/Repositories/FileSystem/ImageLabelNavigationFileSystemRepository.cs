using Rcv.LabelTool.Contracts.Enumerations;
using Rcv.LabelTool.Contracts.Models;
using Rcv.LabelTool.Contracts.Repositories;
using Rcv.LabelTool.Contracts.Results;
using Rcv.LabelTool.Web.Logic;
using System;
using System.Collections.Generic;

namespace Rcv.LabelTool.Services.Repositories.FileSystem
{
    public class ImageLabelNavigationFileSystemRepository : IImageLabelNavigationRepository
    {
        #region member

        private IImageDataRepository ImageRepository { get; }

        #endregion

        #region constructor

        public ImageLabelNavigationFileSystemRepository(IImageDataRepository imageRepository)
        {
            ImageRepository = imageRepository ?? throw new ArgumentNullException(nameof(imageRepository));
        }

        #endregion

        #region IImageNavigationRepository

        public ImageNavigationResult GetNavigationResult(Topic topic, long startIndex, ELabelMode labelMode, ENavigationDirection navigationDirection)
        {
            #region valdiation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            if (!Enum.IsDefined(typeof(ELabelMode), labelMode))
            {
                throw new ArgumentException(nameof(labelMode));
            }

            if (!Enum.IsDefined(typeof(ENavigationDirection), navigationDirection))
            {
                throw new ArgumentException(nameof(navigationDirection));
            }

            #endregion

            // get list of all image labels
            IList<ImageLabel> images = ImageRepository.GetImages(topic);

            // fill imagelabel-structure of image label
            ImageLabel image = images.Navigate(labelMode, navigationDirection, (int)startIndex);

            return CreateImageNavigationResult(topic, image, images, labelMode);
        }       

        #endregion

        #region private helper


        private ImageNavigationResult CreateImageNavigationResult(Topic topic, ImageLabel image, IList<ImageLabel> images, ELabelMode labelMode)
        {
            // no image found for navigation request, return null
            if (image == null)
            {
                return null;
            }

            // get fullfilled image structure
            return new ImageNavigationResult
            {
                ImageLabel = ImageRepository.GetImageLabelById(topic, image.Id),
                ImageCount = images.Count,
                HasNextBlank = images.HasNextBlank(image, labelMode),
                HasNext = image.Index < images.Count - 1,
                HasPrevious = image.Index > 0,
                HasPreviousBlank = images.HasPreviousBlank(image, labelMode)
            };
        }

        #endregion
    }
}
