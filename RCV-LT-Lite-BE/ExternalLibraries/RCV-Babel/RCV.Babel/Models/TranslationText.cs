namespace RCV.Babel.Models
{
    /// <summary>
    /// Data contract for translated text.
    /// </summary>
    public class TranslationText
    {
        /// <summary>
        /// Name of translated field.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Translation of field value.
        /// </summary>
        public string Text { get; set; }
    }
}
