using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace Rcv.Base.WebApi.Controller.Validation
{
    /// <summary>
    /// Validator for FormFiles.
    /// </summary>
    public static class FormFileValidator
    {
        /// <summary>
        /// List of valid image content types.
        /// </summary>
        public static IEnumerable<string> ValidImageContentTypes { get; set; }

        /// <summary>
        /// List of valid video content types.
        /// </summary>
        public static IEnumerable<string> ValidVideoContentTypes { get; set; }

        /// <summary>
        /// Checks if formFile has valid image content type.
        /// </summary>
        /// <param name="formFile">Formfile to check</param>
        /// <returns>TRUE if contenttype is valid, otherwise FALSE</returns>
        public static bool HasValidImageContentType(this IFormFile formFile)
        {
            return formFile.HasValidContentType(ValidImageContentTypes);
        }

        /// <summary>
        /// Checks if formFile has valid video content type.
        /// </summary>
        /// <param name="formFile">Formfile to check</param>
        /// <returns>TRUE if contenttype is valid, otherwise FALSE</returns>
        public static bool HasValidVideoContentType(this IFormFile formFile)
        {
            return formFile.HasValidContentType(ValidVideoContentTypes);
        }

        #region private helper

        /// <summary>
        /// Checks if content types are valid.
        /// </summary>
        /// <param name="formFile">FormFile to check for valid contenttype</param>
        /// <param name="validContentTypes">List of valid content types</param>
        /// <returns>TRUE if contenttype is valid or no content type is defined</returns>
        private static bool HasValidContentType(this IFormFile formFile, IEnumerable<string> validContentTypes)
        {
            if (validContentTypes == null || validContentTypes.Count() == 0)
            {
                return true;
            }

            return validContentTypes.Any(o => formFile.ContentType == o);
        }

        #endregion
    }
}
