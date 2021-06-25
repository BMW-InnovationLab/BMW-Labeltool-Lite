using Rcv.LabelTool.Contracts.Models.View;
using Rcv.LabelTool.Contracts.Results;
using System;

namespace Rcv.LabelTool.Logic.Factories
{
    /// <summary>
    /// Factory to create image navigation result.
    /// </summary>
    public static class ImageNavigationResultFactory
    {
        /// <summary>
        /// Create view of image navigation result.
        /// </summary>
        /// <param name="imageNavigationResult">Navigation result to create view from</param>
        /// <returns>View of image navigation result</returns>
        public static ImageNavigationResultView CreateView(ImageNavigationResult imageNavigationResult)
        {
            #region valdiation

            if (imageNavigationResult == null)
            {
                throw new ArgumentNullException(nameof(imageNavigationResult));
            }

            #endregion

            return new ImageNavigationResultView
            {
                ImageLabel = ImageLabelFactory.CreateView(imageNavigationResult.ImageLabel),
                ImageCount = imageNavigationResult.ImageCount,
                HasNextBlank = imageNavigationResult.HasNextBlank,
                HasNext = imageNavigationResult.HasNext,
                HasPrevious = imageNavigationResult.HasPrevious,
                HasPreviousBlank = imageNavigationResult.HasPreviousBlank
            };
        }
    }
}
