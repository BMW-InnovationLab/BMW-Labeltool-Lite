using System;
using System.Net.Http;

namespace Rcv.HttpUtils.HttpClientUtils
{
    /// <summary>
    /// Represenation class of stream result of httpclient request.
    /// </summary>
    static class StreamResult
    {
        /// <summary>
        /// Checks if stream is empty.
        /// </summary>
        /// <param name="stream">Stream which should be checked for emptyness</param>
        /// <returns>TRUE if null or empty, otherwise FALSE</returns>
        public static bool IsEmpty(System.IO.Stream stream)
        {
            return stream == null || stream.Length == 0;
        }

        /// <summary>
        /// Read content of responseMessage as stream.
        /// </summary>
        /// <param name="responseMessage">Response message from web request</param>
        /// <returns>Stream representation of response message</returns>
        public static System.IO.Stream ReadAsStream(HttpResponseMessage responseMessage)
        {
            #region validation

            if (responseMessage == null)
            {
                throw new ArgumentNullException(nameof(responseMessage));
            }

            #endregion

            return responseMessage.Content.ReadAsStreamAsync().Result;
        }
    }
}
