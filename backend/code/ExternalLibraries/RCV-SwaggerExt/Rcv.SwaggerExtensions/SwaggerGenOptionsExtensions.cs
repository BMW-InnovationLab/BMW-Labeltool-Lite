using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;

namespace Rcv.SwaggerExtensions
{
    /// <summary>
    /// Extension methods for SwaggerGenOptions.
    /// </summary>
    public static class SwaggerGenOptionsExtensions
    {
        /// <summary>
        /// Create default configuration for SwaggerGen. Includes:
        ///  - set information at swagger site header
        ///  - set documentation path
        ///  - set operation id selector
        ///  - describe all enums as string
        /// </summary>
        /// <param name="c">SwaggerGenOpeartions parameter</param>
        /// <param name="applicationVersion">Version of application</param>
        /// <param name="applicationName">Name of application</param>
        public static void ConfigureSwaggerGen(this SwaggerGenOptions c, string applicationVersion, string applicationName)
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

            c.SetSwaggerInfo(applicationVersion, applicationName);
            c.SetDocumentationPath();
            c.SetControllerActionOperationIdSelector();
        }

        /// <summary>
        /// Set information to swagger site header.
        /// </summary>
        /// <param name="c">SwaggerGenOpeartions parameter</param>
        /// <param name="applicationVersion">Version of application</param>
        /// <param name="applicationName">Name of application</param>
        public static void SetSwaggerInfo(this SwaggerGenOptions c, string applicationVersion, string applicationName)
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

            c.SwaggerDoc(applicationVersion, new OpenApiInfo
            {
                Title = applicationName,
                Version = applicationVersion,
                Description = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString()
            });
        }

        /// <summary>
        /// Set documentation path where swagger consumes XML-documentation from.
        /// Documentation should be stored in appcontext base directory with name of entry assembly.
        /// </summary>
        /// <param name="c">SwaggerGenOpeartions parameter</param>
        public static void SetDocumentationPath(this SwaggerGenOptions c)
        {
            #region validation

            if (c == null)
            {
                throw new ArgumentNullException(nameof(c));
            }

            #endregion

            string xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, $"{System.Reflection.Assembly.GetEntryAssembly().GetName().Name}.xml");
            c.IncludeXmlComments(xmlPath);
        }

        /// <summary>
        /// Set OperationId-Selector to generate custom operation ids.
        /// This generator will create unique operationIds with
        ///  - Name of controller
        ///  - Name of method
        ///  - Name of all parameters
        /// </summary>
        /// <param name="c">SwaggerGenOpeartions parameter</param>
        public static void SetControllerActionOperationIdSelector(this SwaggerGenOptions c)
        {
            #region validation

            if (c == null)
            {
                throw new ArgumentNullException(nameof(c));
            }

            #endregion

            c.SwaggerGeneratorOptions.OperationIdSelector = GetControllerActionOperationId;
        }

        /// <summary>
        /// Get operationId for method depending on description of api.
        /// Use controller name and action name.
        /// </summary>
        /// <param name="apiDescription">Description of api</param>
        /// <returns>OperationId of method</returns>
        private static string GetControllerActionOperationId(ApiDescription apiDescription)
        {
            if (apiDescription.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                return $"{controllerActionDescriptor.ControllerName}_{controllerActionDescriptor.ActionName}";
            }
            return apiDescription.ActionDescriptor.DisplayName;
        }
    }
}
