using Rcv.LabelTool.Contracts.Models;
using Rcv.LabelTool.Contracts.Models.View;
using System;
using System.Collections.Generic;

namespace Rcv.LabelTool.Logic.Factories
{
    /// <summary>
    /// Factory for creation of imagelabel-representation objects.
    /// </summary>
    public static class ImageLabelFactory
    {
        /// <summary>
        /// Create imagelabel view object from imagelabel.
        /// </summary>
        /// <param name="image">Source imagelabel object</param>
        /// <returns>View representation of imagelabel</returns>
        public static ImageLabelView CreateView(ImageLabel image)
        {
            #region validation

            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            #endregion

            return new ImageLabelView()
            {
                Labels = new List<LabelView>(),
                Id = image.Id,
                Index = image.Index,
                Width = image.Width,
                Height = image.Height,
                HasLabels = image.HasLabels
            };
        }
    }
}
