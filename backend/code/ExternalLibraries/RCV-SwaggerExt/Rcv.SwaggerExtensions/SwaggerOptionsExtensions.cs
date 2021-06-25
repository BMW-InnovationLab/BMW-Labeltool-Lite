using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;

namespace Rcv.SwaggerExtensions
{
    /// <summary>
    /// Extension methods for SwaggerOptions.
    /// </summary>
    public static class SwaggerOptionsExtensions
    {
        /// <summary>
        /// Configure swagger:
        ///  - set host and scheme of spec
        /// </summary>
        /// <param name="c">SwaggerOption parameter</param>
        public static void ConfigureSwagger(this SwaggerOptions c)
        {
            #region validation

            if (c == null)
            {
                throw new ArgumentNullException(nameof(c));
            }

            #endregion

            c.SetHostAndSchemes();
        }

        /// <summary>
        /// Set host and scheme for Swagger Spec regarding on current
        /// HttpContext.
        /// </summary>
        /// <param name="c">SwaggerOption parameter</param>
        public static void SetHostAndSchemes(this SwaggerOptions c)
        {
            #region validation

            if (c == null)
            {
                throw new ArgumentNullException(nameof(c));
            }

            #endregion

            c.PreSerializeFilters.Add((swaggerDoc, httpRequest) =>
            {
                // set scheme to request scheme and host to request host adress
                swaggerDoc.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{httpRequest.Scheme}://{httpRequest.Host.Value}" } };
            });
        }
    }
}
