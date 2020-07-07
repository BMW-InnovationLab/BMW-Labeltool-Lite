using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Rcv.LabelTool.Contracts.Models
{
    /// <summary>
    /// Representation of Bounding-Box structure in 
    /// frontend of labeltool.
    /// </summary>
    public class Label : IValidatableObject
    {
        /// <summary>
        /// Id of label. Newly created labels will
        /// have an id of -1.
        /// Id is unique overall labels of image.
        /// </summary>
        public int Id { get; set; } = -1;

        /// <summary>
        /// Name of objectclass of object shown
        /// with bounding-box.
        /// </summary>
        [Required]
        public string ObjectClassName { get; set; }

        /// <summary>
        /// Id of objectclass of object shown
        /// with bounding-box.
        /// </summary>
        [Required]
        public uint ObjectClassId { get; set; }

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
