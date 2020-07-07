using RCV.Babel.Models;
using System.Collections.Generic;

namespace RCV.Babel.DataContracts
{
    /// <summary>
    /// Interface definition for entities which includes translatable content.
    /// </summary>
    public interface ITranslatable
    {
        /// <summary>
        /// Collection of translations.
        /// </summary>
        ICollection<Translation> Translations { get; set; }
    }
}
