using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Rcv.LabelTool.Contracts.Models
{
    /// <summary>
    /// Structure for objectclass information.
    /// </summary>
    public class ObjectClass
    {
        /// <summary>
        /// If color can't be read from xml-file it's stored with this value.
        /// In logic it will be replaced with color which is needed.
        /// </summary>
        public static int UNDEFINED_COLOR = -1;

        /// <summary>
        /// ID of ObjectClass.
        /// </summary>
        public uint Id { get; set; }

        /// <summary>
        /// Name of ObjectClass
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Color of ObjectClass.
        /// </summary>        
        [JsonIgnore]
        public int Color { get; set; } = UNDEFINED_COLOR;

        [Required]
        [RegularExpression(@"^([0-9a-fA-F]){6}$")]
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
    }
}
