using System;
using System.Net.Http;

namespace Rcv.HttpUtils.HttpClientUtils
{
    /// <summary>
    /// Represenation class of byte array result of httpclient request.
    /// </summary>
    static class ByteArrayResult
    {
        /// <summary>
        /// Checks if byteArray is empty.
        /// </summary>
        /// <param name="byteArray">ByteArray which should be checked for emptyness</param>
        /// <returns>TRUE if null or empty, otherwise FALSE</returns>
        public static bool IsEmpty(byte[] byteArray)
        {
            return byteArray == null || byteArray.Length == 0;
        }

        /// <summary>
        /// Read content of responseMessage as byte array.
        /// </summary>
        /// <param name="responseMessage">Response message from web request</param>
        /// <returns>Byte array representation of response message</returns>
        public static byte[] ReadAsByteArray(HttpResponseMessage responseMessage)
        {
            #region validation

            if (responseMessage == null)
            {
                throw new ArgumentNullException(nameof(responseMessage));
            }

            #endregion

            return responseMessage.Content.ReadAsByteArrayAsync().Result;
        }
    }
}
