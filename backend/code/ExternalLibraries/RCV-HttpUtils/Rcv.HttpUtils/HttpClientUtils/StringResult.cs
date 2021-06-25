using System;
using System.Net.Http;

namespace Rcv.HttpUtils.HttpClientUtils
{
    /// <summary>
    /// Represenation class of string result of httpclient request.
    /// </summary>
    static class StringResult
    {
        /// <summary>
        /// Checks if string is empty.
        /// </summary>
        /// <param name="str">String which should be checked for emptyness</param>
        /// <returns>TRUE if null or empty, otherwise FALSE</returns>
        public static bool IsEmpty(string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// Read content of responseMessage as string.
        /// </summary>
        /// <param name="responseMessage">Response message from web request</param>
        /// <returns>String representation of response message</returns>
        public static string ReadAsString(HttpResponseMessage responseMessage)
        {
            #region validation

            if (responseMessage == null)
            {
                throw new ArgumentNullException(nameof(responseMessage));
            }

            #endregion

            return responseMessage.Content.ReadAsStringAsync().Result;
        }
    }
}
