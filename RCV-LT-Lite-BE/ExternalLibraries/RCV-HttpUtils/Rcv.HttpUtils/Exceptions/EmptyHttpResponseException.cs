using System;
using System.Net.Http;

namespace Rcv.HttpUtils.Exceptions
{
    /// <summary>
    /// Exception which will be thrown, if an empty result is returned by webapi
    /// request.
    /// </summary>
    public class EmptyHttpResponseException : Exception
    {
        /// <summary>
        /// Creates a new instance of exception with information from response message.
        /// </summary>
        /// <param name="responseMessage">Response message of web request</param>
        public EmptyHttpResponseException(HttpResponseMessage responseMessage) : base($"Response of request {responseMessage.RequestMessage.RequestUri} is empty, but status code is {responseMessage.StatusCode}")
        {

        }
    }
}
