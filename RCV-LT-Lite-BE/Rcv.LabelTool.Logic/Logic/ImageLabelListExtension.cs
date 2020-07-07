using Rcv.LabelTool.Contracts.Enumerations;
using Rcv.LabelTool.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rcv.LabelTool.Web.Logic
{
    /// <summary>
    /// Extension methods for image label list.
    /// </summary>
    public static class ImageLabelListExtension
    {
        public static IEnumerable<ImageLabel> ShrinkToBlankList(this IList<ImageLabel> imageLabels, ENavigationDirection navigationDirection, int currentIndex)
        {
            return imageLabels.ShrinkToUnlabeledList(navigationDirection, currentIndex);
        }

        private static IEnumerable<ImageLabel> ShrinkToUnlabeledList(this IList<ImageLabel> imageLabels, ENavigationDirection navigationDirection, int currentIndex)
        {
            switch (navigationDirection)
            {
                case ENavigationDirection.Blank:
                    return imageLabels.Where(o => !o.HasLabels);
                case ENavigationDirection.NextBlank:
                    return imageLabels.Skip(currentIndex + 1).Where(o => !o.HasLabels);
                case ENavigationDirection.PreviousBlank:
                    return imageLabels.Take(currentIndex).Where(o => !o.HasLabels);
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Navigate through list of imagelabels for segments.
        /// </summary>
        /// <param name="imageLabels">List of imagelabels to navigate.</param>
        /// <param name="navigationDirection">Direction of navigation</param>
        /// <param name="currentIndex">Current index</param>
        /// <returns>Imagelabel of navigation</returns>
        public static ImageLabel Navigate(this IList<ImageLabel> imageLabels, ENavigationDirection navigationDirection, int currentIndex)
        {
            return imageLabels.NavigateLabels(navigationDirection, currentIndex);
        }

        /// <summary>
        /// Navigate through list of imagelabels for labels.
        /// </summary>
        /// <param name="imageLabels">List of imagelabels to navigate.</param>
        /// <param name="navigationDirection">Direction of navigation</param>
        /// <param name="currentIndex">Current index</param>
        /// <returns>Imagelabel of navigation</returns>
        private static ImageLabel NavigateLabels(this IList<ImageLabel> imageLabels, ENavigationDirection navigationDirection, int currentIndex)
        {
            switch (navigationDirection)
            {
                case ENavigationDirection.Direct:
                    return imageLabels.ElementAtOrDefault(currentIndex);
                case ENavigationDirection.Blank:
                case ENavigationDirection.NextBlank:
                    return imageLabels.NextUnlabeled(currentIndex);
                case ENavigationDirection.Next:
                    return imageLabels.ElementAtOrDefault(currentIndex + 1);
                case ENavigationDirection.Previous:
                    return imageLabels.ElementAtOrDefault(currentIndex - 1);
                case ENavigationDirection.PreviousBlank:
                    return imageLabels.PreviousUnlabeled(currentIndex);
                case ENavigationDirection.LastBlank:
                    int? lastLabeledIndex = imageLabels.LastLabeledIndex();
                    return imageLabels.ElementAtOrDefault(lastLabeledIndex.HasValue ? lastLabeledIndex.Value + 1 : 0);
            }
            throw new NotImplementedException();
        }


        /// <summary>
        /// Get next unlabeled imagelabel.
        /// </summary>
        /// <param name="imageLabels">List of imagelabels</param>
        /// <param name="currentIndex">Current index</param>
        /// <returns>Imagelabel without labels or NULL</returns>
        public static ImageLabel NextUnlabeled(this IList<ImageLabel> imageLabels, int currentIndex)
        {
            return imageLabels.Skip(currentIndex + 1).FirstOrDefault(o => !o.HasLabels);
        }

        /// <summary>
        /// Get previous unlabeled imagelabel.
        /// </summary>
        /// <param name="imageLabels">List of imagelabels</param>
        /// <param name="currentIndex">Current index</param>
        /// <returns>Imagelabel without labels or NULL</returns>
        public static ImageLabel PreviousUnlabeled(this IList<ImageLabel> imageLabels, int currentIndex)
        {
            return imageLabels.Take(currentIndex).LastOrDefault(o => !o.HasLabels);
        }

        /// <summary>
        /// Get information if imagelabels without segments or labels exists after current imagelabel.
        /// </summary>
        /// <param name="imageLabels">List of imagelabels</param>
        /// <param name="imageLabel">Imagelabel to find position for</param>
        /// <returns>TRUE if next imagelabels without segments or labels are avaiable, otherwise FALSE</returns>
        public static bool HasNextBlank(this IList<ImageLabel> imageLabels, ImageLabel imageLabel)
        {
            return imageLabels.HasNextUnlabeled(imageLabel);
        }

        /// <summary>
        /// Get information if imagelabels without labels exists after current imagelabel.
        /// </summary>
        /// <param name="imageLabels">List of imagelabels</param>
        /// <param name="imageLabel">Imagelabel to find position for</param>
        /// <returns>TRUE if next imagelabels without labels are avaiable, otherwise FALSE</returns>
        public static bool HasNextUnlabeled(this IList<ImageLabel> imageLabels, ImageLabel imageLabel)
        {
            int index = imageLabels.IndexOf(imageLabel);
            return imageLabels.Skip(index + 1).Any(o => !o.HasLabels);
        }

        /// <summary>
        /// Get information if imagelabels without segments or labels exists before current imagelabel.
        /// </summary>
        /// <param name="imageLabels">List of imagelabels</param>
        /// <param name="imageLabel">Imagelabel to find position for</param>
        /// <returns>TRUE if previous imagelabels without segments or labels are avaiable, otherwise FALSE</returns>
        public static bool HasPreviousBlank(this IList<ImageLabel> imageLabels, ImageLabel imageLabel)
        {
            return imageLabels.HasPreviousUnlabeled(imageLabel);
        }

        /// <summary>
        /// Get information if imagelabels without labels exists before current imagelabel.
        /// </summary>
        /// <param name="imageLabels">List of imagelabels</param>
        /// <param name="imageLabel">Imagelabel to find position for</param>
        /// <returns>TRUE if previous imagelabels without labels are avaiable, otherwise FALSE</returns>
        public static bool HasPreviousUnlabeled(this IList<ImageLabel> imageLabels, ImageLabel imageLabel)
        {
            int index = imageLabels.IndexOf(imageLabel);
            return imageLabels.Take(index).Any(o => !o.HasLabels);
        }

        /// <summary>
        /// Get information if imagelabel is last in list of imagelabels.
        /// </summary>
        /// <param name="imageLabels">List of imagelabels</param>
        /// <param name="imageLabel">Imagelabel to find position for</param>
        /// <returns>TRUE if next imagelabels are avaiable, otherwise FALSE</returns>
        public static bool HasNext(this IList<ImageLabel> imageLabels, ImageLabel imageLabel)
        {
            return imageLabels.IndexOf(imageLabel) < imageLabels.Count - 1;
        }

        /// <summary>
        /// Get information if imagelabel is first in list of imagelabels.
        /// </summary>
        /// <param name="imageLabels">List of imagelabels</param>
        /// <param name="imageLabel">Imagelabel to find position for</param>
        /// <returns>TRUE if previous imagelabels are avaiable, otherwise FALSE</returns>
        public static bool HasPrevious(this IList<ImageLabel> imageLabels, ImageLabel imageLabel)
        {
            return imageLabels.IndexOf(imageLabel) != 0;
        }

        /// <summary>
        /// Get index of last labeled imagelabel in imageLabels.
        /// </summary>
        /// <param name="imageLabels">List of imagelabels</param>
        /// <returns>Index of last labeled imageLabel or NULL</returns>
        public static int? LastLabeledIndex(this IList<ImageLabel> imageLabels)
        {
            ImageLabel lastLabeled = imageLabels.LastOrDefault(o => o.HasLabels);
            if (lastLabeled == null)
            {
                return null;
            }
            return imageLabels.IndexOf(lastLabeled);
        }
    }
}