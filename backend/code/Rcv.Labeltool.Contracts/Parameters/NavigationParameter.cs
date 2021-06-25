using Rcv.LabelTool.Contracts.Enumerations;
using System;

namespace Rcv.LabelTool.Contracts.Parameters
{
    /// <summary>
    /// Parameter structure for navigation through labeltool dataset.
    /// </summary>
    public class NavigationParameter
    {
        /// <summary>
        /// Index of navigation. Index of elements is 0-based.
        /// First image will be requested with index of 0.
        /// </summary>
        public int Index { get; set; } = 0;

        /// <summary>
        /// Mode of labeltool. Navigation result depends on
        /// labelmode.
        /// </summary>
        public ELabelMode LabelMode { get; set; } = ELabelMode.ObjectDetection;

        /// <summary>
        /// Mode of navigation.
        /// </summary>
        public ENavigationMode NavigationMode { get; set; } = ENavigationMode.Standard;

        /// <summary>
        /// Shuffle navigation to get random images.
        /// </summary>
        [Obsolete("Use NavigationMode = ENavigationMode.Shuffle instead!")]
        public bool Shuffle { get; set; } = false;
    }
}
