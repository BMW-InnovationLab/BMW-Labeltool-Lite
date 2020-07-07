using System.Collections.Generic;

namespace RCV.Babel.Models
{
    /// <summary>
    /// Data contract for text translations of specific culture.
    /// </summary>
    public class Translation
    {
        /// <summary>
        /// Culture with pattern xx-XX of translation.
        /// </summary>
        public string Culture { get; set; }

        /// <summary>
        /// Key value list of translation texts.
        /// </summary>
        public IEnumerable<TranslationText> Texts { get; set; }
    }
}
