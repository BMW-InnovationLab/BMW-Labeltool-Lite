using System.ComponentModel.DataAnnotations;

namespace Rcv.LabelTool.Contracts.Models.View
{
    /// <summary>
    /// Structure for objectclass information.
    /// </summary>
    public class ObjectClassView
    {
        /// <summary>
        /// ID of ObjectClass.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Sort of Objectclass.
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "{0} value must be between {1} and {2}.")]
        public long? Sort { get; set; }

        /// <summary>
        /// Name of ObjectClass
        /// </summary>
        [Required]
        [RegularExpression(ObjectClass.REGEX_NAME)]
        public string Name { get; set; }

        /// <summary>
        /// Color code of objectclass as 6 letter/digit hex value.
        /// </summary>
        [Required]
        [RegularExpression(ObjectClass.REGEX_COLOR_CODE)]
        public string ColorCode { get; set; }

        /// <summary>
        /// Id of referenced parent objectclass.
        /// </summary>
        public long? ParentObjectClassId { get; set; }
    }
}
