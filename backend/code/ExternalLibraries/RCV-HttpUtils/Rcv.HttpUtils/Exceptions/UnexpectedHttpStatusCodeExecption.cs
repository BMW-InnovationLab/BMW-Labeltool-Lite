using System;
using System.Net.Http;

namespace Rcv.HttpUtils.Exceptions
{

    /// <summary>
    /// Exception which will be thrown, if an unexpected http status code is returned by 
    /// webapi request.
    /// </summary>
    public class UnexpectedHttpStatusCodeExecption : Exception
    {
        /// <summary>
        /// Creates a new instance of exception with information from response message.
        /// </summary>
        /// <param name="responseMessage">Response message of web request</param>
        public UnexpectedHttpStatusCodeExecption(HttpResponseMessage responseMessage) : base($"Unexpected status code {responseMessage.StatusCode} while request {responseMessage.RequestMessage.RequestUri}")
        {

        }
    }
}
