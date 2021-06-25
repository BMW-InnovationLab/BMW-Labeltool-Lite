using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Rcv.LabelTool.Contracts.Models.View
{
    /// <summary>
    /// Representation of Bounding-Box structure in
    /// frontend of labeltool.
    /// </summary>
    public class LabelView : IValidatableObject
    {
        /// <summary>
        /// Id of label. Newly created labels will
        /// have an id of -1.
        /// Id is unique overall labels of image.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of objectclass of object shown
        /// with bounding-box.
        /// </summary>
        public string ObjectClassName { get; set; }

        /// <summary>
        /// Id of objectclass of object shown
        /// with bounding-box.
        /// </summary>
        [Required]
        public long ObjectClassId { get; set; }

        /// <summary>
        /// Distance of left upper corner to left border.
        /// </summary>
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "{0} value must be between {1} and {2}.")]
        public int Left { get; set; }

        /// <summary>
        /// Distance of left upper corner to top border.
        /// </summary>
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "{0} value must be between {1} and {2}.")]
        public int Top { get; set; }

        /// <summary>
        /// Distance of right lower corner to left border.
        /// </summary>
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "{0} value must be between {1} and {2}.")]
        public int Right { get; set; }

        /// <summary>
        /// Distance of right lower corner to top border.
        /// </summary>
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "{0} value must be between {1} and {2}.")]
        public int Bottom { get; set; }

        /// <summary>
        /// Confidence of object-detection.
        /// Value is obtained from object-detection-service.
        /// </summary>
        public double? Confidence { get; set; }

        /// <summary>
        /// Validates label coordinates matching 2D or 3D structure.
        /// </summary>
        /// <param name="validationContext">object instance to validate</param>
        /// <returns>List of coordinate validation error messages</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Left > Right)
            {
                yield return new ValidationResult("Left coordinate can not be larger than right coordinate.", new List<string>() { nameof(Left), nameof(Right) });
            }
            if (Top > Bottom)
            {
                yield return new ValidationResult("Top coordinate can not be larger than bottom coordinate.", new List<string>() { nameof(Top), nameof(Bottom) });
            }
        }
    }
}
