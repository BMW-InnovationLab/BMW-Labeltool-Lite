using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace Rcv.Base.Filter
{
    /// <summary>
    /// Filterattribute for controllers to log all incoming requests and
    /// outgoing responses.
    /// </summary>
    class LogActions : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // incoming request
            Log.Debug($"Execute action {context.ActionDescriptor.DisplayName}");

            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            // outgoing response
            Log.Debug($"Executed action {context.ActionDescriptor.DisplayName}");

            base.OnActionExecuted(context);

        }
    }
}
