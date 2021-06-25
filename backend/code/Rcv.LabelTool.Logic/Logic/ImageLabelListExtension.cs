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
        /// <summary>
        /// Shrink given list to list of images without additional elements (depends on labelmode).
        /// For example: Shrink image list to list of images without labels in labelmode objectdetection.
        /// </summary>
        /// <param name="imageLabels">List of labels</param>
        /// <param name="labelMode">Mode of navigation</param>
        /// <param name="navigationDirection">Direction of navigation</param>
        /// <param name="currentIndex">Current index of navigation</param>
        /// <returns>Shrinked list</returns>
        public static IEnumerable<ImageLabel> ShrinkToBlankList(this IEnumerable<ImageLabel> imageLabels, ELabelMode labelMode, ENavigationDirection navigationDirection, int currentIndex)
        {
            #region validation

            if (imageLabels == null)
            {
                throw new ArgumentNullException(nameof(imageLabels));
            }

            #endregion

            switch (navigationDirection)
            {
                case ENavigationDirection.Blank:
                    return imageLabels.Where(HasNoElements(labelMode));
                case ENavigationDirection.NextBlank:
                    return imageLabels.Skip(currentIndex + 1).Where(HasNoElements(labelMode));
                case ENavigationDirection.PreviousBlank:
                    return imageLabels.Take(currentIndex).Where(HasNoElements(labelMode));
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Navigate through list of imagelabels for segments.
        /// </summary>
        /// <param name="imageLabels">List of imagelabels to navigate.</param>
        /// <param name="navigationDirection">Direction of navigation</param>
        /// <param name="labelMode">Label mode</param>
        /// <param name="currentIndex">Current index</param>
        /// <returns>Imagelabel of navigation</returns>
        public static ImageLabel Navigate(this IList<ImageLabel> imageLabels, ELabelMode labelMode, ENavigationDirection navigationDirection, int currentIndex)
        {
            #region validation

            if (imageLabels == null)
            {
                throw new ArgumentNullException(nameof(imageLabels));
            }

            #endregion

            switch (navigationDirection)
            {
                case ENavigationDirection.Direct:
                    return imageLabels.ElementAtOrDefault(currentIndex);
                case ENavigationDirection.Blank:
                case ENavigationDirection.NextBlank:
                    return imageLabels.Next(currentIndex, HasNoElements(labelMode));
                case ENavigationDirection.Next:
                    return imageLabels.ElementAtOrDefault(currentIndex + 1);
                case ENavigationDirection.Previous:
                    return imageLabels.ElementAtOrDefault(currentIndex - 1);
                case ENavigationDirection.PreviousBlank:
                    return imageLabels.Previous(currentIndex, HasNoElements(labelMode));
                case ENavigationDirection.LastBlank:
                    int? lastIndex = imageLabels.LastIndex(HasElements(labelMode));
                    return imageLabels.ElementAtOrDefault(lastIndex.HasValue ? lastIndex.Value + 1 : 0);
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get information if imagelabels without segments or labels exists after current imagelabel.
        /// </summary>
        /// <param name="imageLabels">List of imagelabels</param>
        /// <param name="imageLabel">Imagelabel to find position for</param>
        /// <param name="labelMode">Mode of labels</param>
        /// <returns>TRUE if next imagelabels without segments or labels are avaiable, otherwise FALSE</returns>
        public static bool HasNextBlank(this IList<ImageLabel> imageLabels, ImageLabel imageLabel, ELabelMode labelMode)
        {
            #region validation

            if (imageLabels == null)
            {
                throw new ArgumentNullException(nameof(imageLabels));
            }

            if (imageLabel == null)
            {
                throw new ArgumentNullException(nameof(imageLabel));
            }

            #endregion

            return imageLabels.HasNext(imageLabel, HasNoElements(labelMode));
        }

        /// <summary>
        /// Get information if imagelabels without segments or labels exists before current imagelabel.
        /// </summary>
        /// <param name="imageLabels">List of imagelabels</param>
        /// <param name="imageLabel">Imagelabel to find position for</param>
        /// <param name="labelMode">Mode of labels</param>
        /// <returns>TRUE if previous imagelabels without segments or labels are avaiable, otherwise FALSE</returns>
        public static bool HasPreviousBlank(this IList<ImageLabel> imageLabels, ImageLabel imageLabel, ELabelMode labelMode)
        {
            #region validation

            if (imageLabels == null)
            {
                throw new ArgumentNullException(nameof(imageLabels));
            }

            if (imageLabel == null)
            {
                throw new ArgumentNullException(nameof(imageLabel));
            }

            #endregion

            return imageLabels.HasPrevious(imageLabel, HasNoElements(labelMode));
        }

        #region private helper

        /// <summary>
        /// Get function to check if image has elements (labels, segments or classifications).
        /// </summary>
        /// <param name="labelMode">Mode of navigation</param>
        /// <returns>Function to get information if elements are avaiable</returns>
        private static Func<ImageLabel, bool> HasElements(ELabelMode labelMode)
        {
            switch (labelMode)
            {
                case ELabelMode.ObjectDetection:
                    return o => o.HasLabels;
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get function to check if image has no elements (labels, segments or  classifications).
        /// </summary>
        /// <param name="labelMode">Mode of navigation</param>
        /// <returns></returns>
        private static Func<ImageLabel, bool> HasNoElements(ELabelMode labelMode)
        {
            return o => !HasElements(labelMode).Invoke(o);
        }

        /// <summary>
        /// Get information if imagelabels without labels exists after current imagelabel.
        /// </summary>
        /// <param name="imageLabels">List of imagelabels</param>
        /// <param name="imageLabel">Imagelabel to find position for</param>
        /// <returns>TRUE if next imagelabels without labels are avaiable, otherwise FALSE</returns>
        private static bool HasNext(this IList<ImageLabel> imageLabels, ImageLabel imageLabel, Func<ImageLabel, bool> hasNotFunc)
        {
            int index = imageLabels.IndexOf(imageLabel);
            return imageLabels.Skip(index + 1).Any(hasNotFunc);
        }

        /// <summary>
        /// Get next unlabeled imagelabel.
        /// </summary>
        /// <param name="imageLabels">List of imagelabels</param>
        /// <param name="currentIndex">Current index</param>
        /// <returns>Imagelabel without labels or NULL</returns>
        private static ImageLabel Next(this IList<ImageLabel> imageLabels, int currentIndex, Func<ImageLabel, bool> hasNotFunc)
        {
            return imageLabels.Skip(currentIndex + 1).FirstOrDefault(hasNotFunc);
        }

        /// <summary>
        /// Get information if imagelabels without labels exists before current imagelabel.
        /// </summary>
        /// <param name="imageLabels">List of imagelabels</param>
        /// <param name="imageLabel">Imagelabel to find position for</param>
        /// <returns>TRUE if previous imagelabels without labels are avaiable, otherwise FALSE</returns>
        private static bool HasPrevious(this IList<ImageLabel> imageLabels, ImageLabel imageLabel, Func<ImageLabel, bool> hasNotFunc)
        {
            int index = imageLabels.IndexOf(imageLabel);
            return imageLabels.Take(index).Any(hasNotFunc);
        }

        /// <summary>
        /// Get previous unlabeled imagelabel.
        /// </summary>
        /// <param name="imageLabels">List of imagelabels</param>
        /// <param name="currentIndex">Current index</param>
        /// <returns>Imagelabel without labels or NULL</returns>
        private static ImageLabel Previous(this IList<ImageLabel> imageLabels, int currentIndex, Func<ImageLabel, bool> hasNotFunc)
        {
            return imageLabels.Take(currentIndex).LastOrDefault(hasNotFunc);
        }

        /// <summary>
        /// Get index of last labeled imagelabel in imageLabels.
        /// </summary>
        /// <param name="imageLabels">List of imagelabels</param>
        /// <returns>Index of last labeled imageLabel or NULL</returns>
        private static int? LastIndex(this IList<ImageLabel> imageLabels, Func<ImageLabel, bool> hasFunc)
        {
            ImageLabel last = imageLabels.LastOrDefault(hasFunc);
            if (last == null)
            {
                return null;
            }
            return imageLabels.IndexOf(last);
        }

        #endregion
    }
}