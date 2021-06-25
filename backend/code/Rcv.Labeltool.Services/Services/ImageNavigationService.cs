using Rcv.LabelTool.Contracts.Enumerations;
using Rcv.LabelTool.Contracts.Models;
using Rcv.LabelTool.Contracts.Repositories;
using Rcv.LabelTool.Contracts.Results;
using Rcv.LabelTool.Contracts.Services;
using System;

namespace Rcv.LabelTool.Services.Services
{
    public class ImageNavigationService : IImageNavigationService
    {
        #region member

        private IImageLabelNavigationRepository ImageLabelNavigationRepository { get; }

        #endregion

        #region constructor

        public ImageNavigationService(IImageLabelNavigationRepository imageLabelNavigationRepository)
        {
            ImageLabelNavigationRepository = imageLabelNavigationRepository ?? throw new ArgumentNullException(nameof(imageLabelNavigationRepository));
        }

        #endregion

        #region IImageNavigationService

        public ImageNavigationResult Navigate(Topic topic, long startIndex, ELabelMode labelMode, ENavigationDirection navigationDirection)
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

            return ImageLabelNavigationRepository.GetNavigationResult(topic, startIndex, labelMode, navigationDirection);
        }

        #endregion
    }
}
