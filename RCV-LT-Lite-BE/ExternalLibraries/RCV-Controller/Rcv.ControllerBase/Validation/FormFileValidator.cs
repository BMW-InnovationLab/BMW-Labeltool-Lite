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
        /// Checks if formFile has valid content type.
        /// </summary>
        /// <param name="formFile">Formfile to check</param>
        /// <returns>TRUE if contenttype is valid, otherwise FALSE</returns>
        public static bool HasValidImageContentType(this IFormFile formFile)
        {
            if (ValidImageContentTypes == null || ValidImageContentTypes.Count() == 0)
            {
                return true;
            }

            return ValidImageContentTypes.Any(o => formFile.ContentType == o);
        }
    }
}
