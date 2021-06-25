using Rcv.LabelTool.Contracts.Exceptions;
using Rcv.LabelTool.Contracts.Models;
using Rcv.LabelTool.Logic.Logic;
using System.Collections.Generic;

namespace Rcv.LabelTool.Web.Utilities
{
    /// <summary>
    /// Util for validate label information.
    /// </summary>
    public static class LabelValidityUtil
    {
        /// <summary>
        /// Checks if objectclass of label is defined in ObjectClassStore. 
        /// Uses ObjectClassId and ObjectClassName out of label.
        /// </summary>
        /// <param name="objectClasses">ClassStore with all objectClasses</param>
        /// <param name="imageLabel">ImageLabel as parent of label</param>
        /// <param name="label">Label to check for objectclass-vadility</param>
        /// <exception cref="LabelFormatExceptions">Objectclass referenced by label is not known in objectClassStore</exception>
        public static void CheckObjectClass(IEnumerable<ObjectClass> objectClasses, ImageLabel imageLabel, Label label)
        {
            ObjectClass objectClass = objectClasses.GetObjectClass(label.ObjectClassId, label.ObjectClassName);
            if (objectClass == null)
            {
                System.Diagnostics.Trace.TraceError($"Label {imageLabel.Id} has invalid objectclass-information! ID {label.ObjectClassId} doesn't match {label.ObjectClassName}.");
                throw new LabelFormatExceptions("Stored label contains no valid ObjectClass-Id/Name pair (Json).");
            }
        }

        /// <summary>
        /// Check if coordinates of label are mirror-inverted.
        /// Error will occure, when right smaller than left or bottom smaller than top
        ///   --> bottom is distance from top of image to end of bounding box
        ///   --> right is distance from left (image border) to end of bounding box
        /// </summary>
        /// <param name="imageLabel">ImageLabel as parent of label</param>
        /// <param name="label">Label to check for mirror-inverted coordinates</param>
        /// <exception cref="LabelFormatExceptions">Coordinates of label are mirror-inverted</exception>
        public static void CheckMirrorInvertedCoordinates(ImageLabel imageLabel, Label label)
        {
            if (label.Bottom < label.Top || label.Right < label.Left)
            {
                System.Diagnostics.Trace.TraceWarning($"Label {imageLabel.Id} has invalid coordinates (top/bottom are twisted OR left/right are twisted)! ID {label.Id}.");
                throw new LabelFormatExceptions("Stored label has invalid coordinates (JSON). Coordinates are mirror-inverted.");
            }
        }

        /// <summary>
        /// Check if coordinates of label are bigger than image-size.
        /// Error will ocure, when label is larger than witdh or height of image.
        /// </summary>
        /// <param name="imageLabel">ImageLabel as parent of label</param>
        /// <param name="label">Label to check for coordinate-correctness</param>
        /// <exception cref="LabelFormatExceptions">Coordinates of label are larger than size of image</exception>
        public static void CheckBoundsOfCoordinates(ImageLabel imageLabel, Label label)
        {
            if (label.Bottom > imageLabel.Height || label.Right > imageLabel.Width)
            {
                System.Diagnostics.Trace.TraceWarning($"Label {imageLabel.Id} has invalid coordinates (label is larger than image)! ID {label.Id}.");
                throw new LabelFormatExceptions("Stored label has invalid coordinates (JSON). Coordinates are bigger than image.");
            }
        }

        /// <summary>
        /// Performs all known checks for labels defined in imageLabel.
        /// Check includes:
        ///     - correct object class information (correlation between id and name)
        ///     - mirror inverted coordinates
        ///     - bounds of boundingboxes
        /// </summary>
        /// <param name="imageLabel">ImageLabel with all labels to check for.</param>
        /// <param name="objectClasses">Objectclasses defined in topic</param>
        public static void CheckAllLabels(ImageLabel imageLabel, IEnumerable<Label> labels, IEnumerable<ObjectClass> objectClasses)
        {
            if (imageLabel.Labels != null)
            {
                foreach (Label label in labels)
                {
                    // check objectclasses
                    CheckObjectClass(objectClasses, imageLabel, label);
                    // check mirror inverted coordinates
                    CheckMirrorInvertedCoordinates(imageLabel, label);
                    // check bounds of coordinates
                    CheckBoundsOfCoordinates(imageLabel, label);
                }
            }
        }
    }
}