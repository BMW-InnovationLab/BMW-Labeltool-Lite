using System;
using System.IO;
using System.Net.Http;

namespace Rcv.HttpUtils
{
    /// <summary>
    /// Util for sending web requests and building content.
    /// </summary>
    public static class RequestUtil
    {
        /// <summary>
        /// Add stream as byte array respresentation to multiparform data content of request.
        /// </summary>
        /// <param name="formDataContent">Multipartformdata content to add file to</param>
        /// <param name="fileStream">Stream of file to add to content</param>
        /// <param name="parameterName">Name of parameter which is used to consume file</param>
        /// <param name="fileName">Name of file (used for content type determination)</param>
        public static void AddFileAsByteContent(this MultipartFormDataContent formDataContent, Stream fileStream, string parameterName, string fileName)
        {
            #region validation

            if (fileStream == null)
            {
                throw new ArgumentNullException(nameof(fileStream));
            }

            if (string.IsNullOrEmpty(parameterName))
            {
                throw new ArgumentNullException(nameof(parameterName));
            }

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            #endregion

            byte[] byteContent = fileStream.ConvertToByteArray();
            formDataContent.AddFileAsByteContent(byteContent, parameterName, fileName);
        }

        /// <summary>
        /// Add stream as byte array respresentation to multiparform data content of request.
        /// </summary>
        /// <param name="formDataContent">Multipartformdata content to add file to</param>
        /// <param name="fileStream">Stream of file to add to content</param>
        /// <param name="parameterName">Name of parameter which is used to consume file</param>
        /// <param name="fileName">Name of file (used for content type determination)</param>
        public static void AddFileAsByteContent(this MultipartFormDataContent formDataContent, byte[] fileBytes, string parameterName, string fileName)
        {
            #region validation

            if (fileBytes == null)
            {
                throw new ArgumentNullException(nameof(fileBytes));
            }

            if (string.IsNullOrEmpty(parameterName))
            {
                throw new ArgumentNullException(nameof(parameterName));
            }

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            #endregion

            ByteArrayContent byteArrayContent = new ByteArrayContent(fileBytes, 0, fileBytes.Length);
            byteArrayContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(MimeMapping.MimeUtility.GetMimeMapping(fileName));
            formDataContent.Add(byteArrayContent, parameterName, fileName);
        }
    }
}
