using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using System.Net.Mime;

namespace Rcv.Base.Filter
{
    /// <summary>
    /// Filter which should be included in application if [FromBody]-annoatation is used
    /// in controller method. Swagger will not recognize [FromBody] correct, so the parameter
    /// will be sent as [FromQuery].
    /// 
    /// This filter adds all [FromBody] anontated parameters to body parameter collection.
    /// Currently this only works for POST-Requests!
    /// </summary>
    public class UseFromBodyParameters<T> : IDocumentFilter where T : ControllerBase
    {
        private OperationType OperationType { get; } = OperationType.Post;

        /// <summary>
        /// Overwritten functionality of apply.
        /// </summary>
        /// <param name="swaggerDoc">The swagger document</param>
        /// <param name="context">The context</param>
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            if (swaggerDoc != null)
            {
                var controller = typeof(T);
                string route = controller.GetCustomAttributes(true).OfType<RouteAttribute>().Single().Template;
                route = "/" + route;

                foreach (var path in swaggerDoc.Paths)
                {
                    if (path.Key == route && path.Value.Operations.ContainsKey(OperationType))
                    {
                        // get post operation
                        var postOperation = path.Value.Operations[OperationType];

                        foreach (var controllerMethod in controller.GetMethods())
                        {
                            if (controllerMethod.GetCustomAttributes(true).OfType<HttpPostAttribute>().Any())
                            {
                                foreach (var parameter in controllerMethod.GetParameters())
                                {
                                    if (parameter.GetCustomAttributes(true).OfType<FromBodyAttribute>().Any())
                                    {
                                        // get parameter with frombody attribute
                                        var param = postOperation.Parameters.Single(o => o.Name.Equals(parameter.Name, System.StringComparison.CurrentCultureIgnoreCase));

                                        // remove parameter from collection of query parameters
                                        postOperation.Parameters.Remove(param);

                                        // add parameter to body parameter collection
                                        var applicationJsonContent = postOperation.RequestBody.Content[MediaTypeNames.Application.Json];
                                        applicationJsonContent.Schema.Properties.Add(param.Name, param.Schema);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
