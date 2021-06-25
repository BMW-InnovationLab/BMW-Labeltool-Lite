using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Rcv.LabelTool.Contracts.Models
{
    /// <summary>
    /// Structure for objectclass information.
    /// </summary>
    public class ObjectClass : ICloneable
    {
        /// <summary>
        /// Regular expression for name of objectclass.
        /// </summary>
        public const string REGEX_NAME = @"^[^\/\\\:\*\?\^\<\>\|]*$";

        /// <summary>
        /// Regular expression for colorcode of objectclass.
        /// </summary>
        public const string REGEX_COLOR_CODE = @"^(#|0x|0X)?([0-9a-fA-F]){6}$";

        /// <summary>
        /// If color can't be read from xml-file it's stored with this value.
        /// In logic it will be replaced with color which is needed.
        /// </summary>
        public static int UNDEFINED_COLOR = -1;

        /// <summary>
        /// ID of ObjectClass.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Sort column of objectclasses.
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "{0} value must be between {1} and {2}.")]
        public long? Sort { get; set; }

        /// <summary>
        /// Name of ObjectClass
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Color of ObjectClass.
        /// </summary>
        [JsonIgnore]
        public int Color { get; set; } = UNDEFINED_COLOR;

        /// <summary>
        /// Color code of objectclass as 6 letter/digit hex value.
        /// </summary>
        public string ColorCode
        {
            get
            {
                if (Color != UNDEFINED_COLOR)
                {
                    return Color.ToString("x6");
                }
                return null;
            }
            set
            {
                string colorValue = value;
                colorValue = colorValue.Replace("#", "");
                colorValue = colorValue.Replace("0x", "");
                colorValue = colorValue.Replace("0X", "");

                Color = int.Parse(colorValue, System.Globalization.NumberStyles.HexNumber);
            }
        }

        /// <summary>
        /// Id of referenced parent objectclass.
        /// </summary>
        public long? ParentObjectClassId { get; set; }

        /// <summary>
        /// Navigation property of referenced parent objectclass.
        /// </summary>
        [JsonIgnore]
        public virtual ObjectClass ParentObjectClass { get; set; }

        /// <summary>
        /// Collection of child objectclasses.
        /// </summary>
        [JsonIgnore]
        public virtual IEnumerable<ObjectClass> ObjectClasses { get; set; }

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

        public object Clone()
        {
            return new ObjectClass()
            {
                Id = Id,
                Sort = Sort,
                Name = Name,
                ColorCode = ColorCode,
                ParentObjectClassId = ParentObjectClassId,
                TopicId = TopicId,
            };
        }
    }
}
