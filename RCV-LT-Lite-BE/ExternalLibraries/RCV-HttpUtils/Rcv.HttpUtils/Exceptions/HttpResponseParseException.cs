using System;
using System.Net.Http;

namespace Rcv.HttpUtils.Exceptions
{
    /// <summary>
    /// Exception which will be thrown, if parsing of response message fails.
    /// </summary>
    public class HttpResponseParseException : Exception
    {
        /// <summary>
        /// Craetes a new instance of execption with information from response message.
        /// </summary>
        /// <param name="responseMessage">Response message of web request</param>
        /// <param name="exception">Exception which was thrown during message parsing</param>
        public HttpResponseParseException(HttpResponseMessage responseMessage, Exception exception) : base($"Unable to parse content of request {responseMessage.RequestMessage.RequestUri}", exception)
        {

        }
    }
}
