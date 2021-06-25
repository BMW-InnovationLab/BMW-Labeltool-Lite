using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Rcv.LabelTool.Contracts.Models
{
    /// <summary>
    /// Representation of Bounding-Box structure in
    /// frontend of labeltool.
    /// </summary>
    public class Label : IValidatableObject, ICloneable
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
        [Required]
        public string ObjectClassName { get; set; }

        /// <summary>
        /// Id of objectclass of object shown
        /// with bounding-box.
        /// </summary>
        [Required]
        public long ObjectClassId { get; set; }

        /// <summary>
        /// Navigation property to refeerenced objectclass.
        /// </summary>
        [JsonIgnore]
        public virtual ObjectClass ObjectClass { get; set; }

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

        /// Confidence of object-detection.
        /// Value is obtained from object-detection-service.
        /// </summary>
        public double? Confidence { get; set; }

        /// <summary>
        /// Id of referenced topic.
        /// </summary>
        [JsonIgnore]
        public long TopicId { get; set; }

        /// <summary>
        /// Navigation property to referenced topic.
        /// </summary>
        [JsonIgnore]
        public virtual Topic Topic { get; set; }

        /// <summary>
        /// Id of referenced image.
        /// </summary>
        [JsonIgnore]
        public long ImageId { get; set; }

        /// <summary>
        /// Navigation property to image.
        /// </summary>
        [JsonIgnore]
        public virtual ImageLabel Image { get; set; }

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

        #region methods

        /// <summary>
        /// Create a clone of the object.
        /// </summary>
        /// <returns>Cloned label</returns>
        public object Clone()
        {
            return new Label()
            {
                Id = Id,
                ObjectClassName = ObjectClassName,
                ObjectClassId = ObjectClassId,
                ObjectClass = ObjectClass,
                Left = Left,
                Top = Top,
                Right = Right,
                Bottom = Bottom,
                Confidence = Confidence,
                TopicId = TopicId,
                Topic = Topic,
                ImageId = ImageId,
                Image = Image
            };
        }

        #endregion
    }
}
