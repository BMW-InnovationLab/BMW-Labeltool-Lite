using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rcv.Base.ModelBinder
{
    /// <summary>
    /// Modelbinder for JSON-Content.
    /// 
    /// Useful for endpoints which should consume IFormFile and JsonContent at once.
    /// Use JsonModelBinder as attribute of endpoint parameter.
    /// </summary>
    /// <example>
    /// <code>
    ///    [HttpPost("{serviceId}")]
    ///    public ActionResult DoSomething(string pathparam, [ModelBinder(BinderType = typeof(JsonModelBinder))] Foo jsonContent, IFormFile file)
    /// </code>
    /// </example>
    public class JsonModelBinder : IModelBinder
    {
        /// <summary>
        /// Bind model using the model binding context.
        /// </summary>
        /// <param name="bindingContext">Binding context</param>
        /// <returns>Task</returns>
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            // Check the value sent in
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult != ValueProviderResult.None)
            {
                bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

                // Attempt to convert the input value
                var valueAsString = valueProviderResult.FirstValue;
                var result = JsonSerializer.Deserialize(valueAsString, bindingContext.ModelType);
                if (result != null)
                {
                    bindingContext.Result = ModelBindingResult.Success(result);
                    return Task.CompletedTask;
                }
            }

            return Task.CompletedTask;
        }
    }
}
