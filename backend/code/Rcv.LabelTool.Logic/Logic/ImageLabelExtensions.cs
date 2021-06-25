using Rcv.LabelTool.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rcv.LabelTool.Web.Logic
{
    /// <summary>
    /// Extension methods for imagelabel.
    /// </summary>
    public static class ImageLabelExtensions
    {
        /// <summary>
        /// Set labels of image label structure.
        /// Fills automatically HasLabels property of imagelabel depending on labels list.
        /// </summary>
        /// <param name="image">ImageLabel where to set labels</param>
        /// <param name="labels">Labels to set at image label structure</param>
        public static void SetLabels(this ImageLabel image, IEnumerable<Label> labels)
        {
            #region validation

            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            #endregion

            image.Labels = (labels != null) ? labels.ToList() : new List<Label>();
            image.HasLabels = image.Labels.Count > 0;
        }
    }
}