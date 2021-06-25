using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rcv.Base.Filter;
using Rcv.Base.Mime;
using Rcv.Base.WebApi.Controller.Validation;
using Serilog;
using System;

namespace Rcv.Base.WebApi.Controller
{
    /// <summary>
    /// Abstract controller for general controller actions in backend space.
    /// 
    /// Controller consumes 'application/json' and produces 'application/json'
    /// </summary>
    [Consumes(ContentTypeNames.Application.Json)]
    [Produces(ContentTypeNames.Application.Json)]
    [LogActions]
    public abstract class AbstractController : ControllerBase
    {
        #region PreValidationException

        /// <summary>
        /// Prevalidation exception for validation of parameters.
        /// </summary>
        private class PreValidationException : Exception
        {
            /// <summary>
            /// Create a new instance.
            /// </summary>
            /// <param name="message">Message of prevalidation</param>
            public PreValidationException(string message) : base(message)
            {

            }
        }

        #endregion

        #region UnsupportedContentTypeException

        /// <summary>
        /// Prevalidation exception for unsupported media types of files.
        /// </summary>
        private class UnsupportedContentTypeException : Exception
        {
            /// <summary>
            /// Create a new instance.
            /// </summary>
            /// <param name="message">Message of validation</param>
            public UnsupportedContentTypeException(string message) : base(message)
            {

            }
        }

        #endregion

        /// <summary>
        /// Delegate for perform controller action.
        /// </summary>
        /// <returns>Action result of action</returns>
        protected delegate ActionResult ControllerAction();

        /// <summary>
        /// Execute action and catch all potential errors.
        /// </summary>
        /// <param name="controllerAction">Action to perform</param>
        /// <returns>Result of action</returns>
        protected ActionResult Execute(ControllerAction controllerAction)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Log.Warning($"Invalid model state prevents action execution!");
                    return BadRequest(ModelState);
                }

                Log.Debug($"Invoke controller action");
                return controllerAction.Invoke();
            }
            catch (PreValidationException preValidationException)
            {
                // validation of request fails
                Log.Warning(preValidationException, $"Prevalidation of request fails!");
                return BadRequest(preValidationException.Message);
            }
            catch (UnsupportedContentTypeException unsupportedContentTypeException)
            {
                // unsupported content type detected
                Log.Warning(unsupportedContentTypeException, $"Requests includes content with unsupported media type!");
                return new UnsupportedMediaTypeResult();
            }
            catch (Exception exc)
            {
                // unexpected error occures
                Log.Error(exc, $"Unexpected error occures while processing the request! Response will be http-500.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }

        #region validation

        /// <summary>
        /// Check mandatory string parameter that this parameter is not null or empty.
        /// </summary>
        /// <param name="parameterName">Name of parameter to check</param>
        /// <param name="value">Value of parameter</param>
        protected void CheckMandatoryParameter(string parameterName, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new PreValidationException($"Parameter {parameterName} is required!");
            }
        }

        /// <summary>
        /// Check mandatory object parameter that this parameter is not null.
        /// </summary>
        /// <param name="parameterName">Name of parameter to check</param>
        /// <param name="value">Value of parameter</param>
        protected void CheckMandatoryParameter(string parameterName, object value)
        {
            if (value == null)
            {
                throw new PreValidationException($"Parameter {parameterName} is required!");
            }
        }

        /// <summary>
        /// Check file parameter for valid content types.
        /// </summary>
        /// <param name="parameterName">Name of file parameter</param>
        /// <param name="imageFile">File to check for valid content type</param>
        protected void CheckImageContentType(string parameterName, IFormFile imageFile)
        {
            if (!imageFile.HasValidImageContentType())
            {
                throw new UnsupportedContentTypeException($"File {imageFile.FileName} ({parameterName}) has unsupported content type {imageFile.ContentType}");
            }
        }

        /// <summary>
        /// Check file parameter for valid content types.
        /// </summary>
        /// <param name="parameterName">Name of file parameter</param>
        /// <param name="videoFile">File to check for valid content type</param>
        protected void CheckVideoContentType(string parameterName, IFormFile videoFile)
        {
            if (!videoFile.HasValidVideoContentType())
            {
                throw new UnsupportedContentTypeException($"File {videoFile.FileName} ({parameterName}) has unsupported content type {videoFile.ContentType}");
            }
        }

        #endregion        
    }
}
