using Microsoft.AspNetCore.Builder;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;

namespace Rcv.SwaggerExtensions
{
    /// <summary>
    /// Extension methods for SwaggerUIOptions.
    /// </summary>
    public static class SwaggerUIOptionsExtensions
    {
        /// <summary>
        /// Configure SwaggerUI to use default swagger json.
        /// </summary>
        /// <param name="c">SwaggerUIOptions parameter</param>
        /// <param name="applicationName">Name of application</param>
        /// <param name="applicationVersion">Version of application</param>
        public static void ConfigureSwaggerUI(this SwaggerUIOptions c, string applicationName, string applicationVersion)
        {
            #region validation

            if (c == null)
            {
                throw new ArgumentNullException(nameof(c));
            }

            if (string.IsNullOrEmpty(applicationVersion))
            {
                throw new ArgumentNullException(nameof(applicationVersion));
            }

            if (string.IsNullOrEmpty(applicationName))
            {
                throw new ArgumentNullException(nameof(applicationName));
            }

            #endregion

            c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{applicationName} {applicationVersion}");
            c.DefaultModelsExpandDepth(-1);
        }
    }
}
