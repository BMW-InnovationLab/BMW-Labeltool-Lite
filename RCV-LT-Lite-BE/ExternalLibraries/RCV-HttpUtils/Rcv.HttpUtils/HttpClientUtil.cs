using Rcv.HttpUtils.Exceptions;
using Rcv.HttpUtils.HttpClientUtils;
using Serilog;
using System;
using System.Net.Http;

namespace Rcv.HttpUtils
{
    /// <summary>
    /// Utility for HttpClient.
    /// </summary>
    public static class HttpClientUtil
    {
        /// <summary>
        /// Delegate method for parsing string based response body.
        /// </summary>
        /// <typeparam name="T">Return type of response message</typeparam>
        /// <param name="responseBody">Response body of request as string</param>
        /// <returns>Typed result</returns>
        public delegate T ParseResponse<T>(string responseBody);

        /// <summary>
        /// Get http web request and parse return value to given type.
        /// </summary>
        /// <typeparam name="T">Result type of response</typeparam>
        /// <param name="httpClient">HttpClient to use for web request</param>
        /// <param name="requestUri">POST URI of request</param>
        /// <param name="parseResponseMethod">Method to parse string based response to result</param>
        /// <returns>Typed result of response</returns>
        public static T Get<T>(this HttpClient httpClient, Uri requestUri, ParseResponse<T> parseResponseMethod)
        {
            #region validation

            if (requestUri == null)
            {
                throw new ArgumentNullException(nameof(requestUri));
            }

            if (parseResponseMethod == null)
            {
                throw new ArgumentNullException(nameof(parseResponseMethod));
            }

            #endregion

            Log.Information($"GET HTTP request {requestUri}");

            HttpResponseMessage response = httpClient.GetAsync(requestUri).Result;
            return ParseResponseMessageAsString<T>(response, parseResponseMethod);
        }

        /// <summary>
        /// Get http web request with an emtpy response.
        /// </summary>
        /// <param name="httpClient">HttpClient to use for web request</param>
        /// <param name="requestUri">POST URI of request</param>
        public static void Get(this HttpClient httpClient, Uri requestUri)
        {
            #region validation

            if (requestUri == null)
            {
                throw new ArgumentNullException(nameof(requestUri));
            }

            #endregion

            Log.Information($"GET HTTP request {requestUri}");

            HttpResponseMessage response = httpClient.GetAsync(requestUri).Result;
            ParseResponseMessage(response);
        }

        /// <summary>
        /// Post http web request and parse return value to given type.
        /// </summary>
        /// <typeparam name="T">Result type of response</typeparam>
        /// <param name="httpClient">HttpClient to use for web request</param>
        /// <param name="requestUri">POST URI of request</param>
        /// <param name="httpContent">Content of request</param>
        /// <param name="parseResponseMethod">Method to parse string based response to result</param>
        /// <returns>Typed result of response</returns>
        public static T Post<T>(this HttpClient httpClient, Uri requestUri, HttpContent httpContent, ParseResponse<T> parseResponseMethod)
        {
            #region validation

            if (requestUri == null)
            {
                throw new ArgumentNullException(nameof(requestUri));
            }

            if (httpContent == null)
            {
                throw new ArgumentNullException(nameof(httpContent));
            }

            if (parseResponseMethod == null)
            {
                throw new ArgumentNullException(nameof(parseResponseMethod));
            }

            #endregion

            Log.Information($"POST HTTP request {requestUri}");

            HttpResponseMessage response = httpClient.PostAsync(requestUri, httpContent).Result;
            return ParseResponseMessageAsString<T>(response, parseResponseMethod);
        }

        /// <summary>
        /// Post http web request and parse return value to given type.
        /// </summary>
        /// <typeparam name="T">Result type of response</typeparam>
        /// <param name="httpClient">HttpClient to use for web request</param>
        /// <param name="requestUri">POST URI of request</param>
        /// <param name="parseResponseMethod">Method to parse string based response to result</param>
        /// <returns>Typed result of response</returns>
        public static T Post<T>(this HttpClient httpClient, Uri requestUri, ParseResponse<T> parseResponseMethod)
        {
            #region validation

            if (requestUri == null)
            {
                throw new ArgumentNullException(nameof(requestUri));
            }

            if (parseResponseMethod == null)
            {
                throw new ArgumentNullException(nameof(parseResponseMethod));
            }

            #endregion

            Log.Information($"POST HTTP request {requestUri}");

            HttpResponseMessage response = httpClient.PostAsync(requestUri, null).Result;
            return ParseResponseMessageAsString<T>(response, parseResponseMethod);
        }

        /// <summary>
        /// Post http web request with an emtpy result.
        /// </summary>
        /// <param name="httpClient">HttpClient to use for web request</param>
        /// <param name="requestUri">POST URI of request</param>
        /// <param name="httpContent">Content of request</param>
        public static void Post(this HttpClient httpClient, Uri requestUri, HttpContent httpContent = null)
        {
            #region validation

            if (requestUri == null)
            {
                throw new ArgumentNullException(nameof(requestUri));
            }

            #endregion

            Log.Information($"POST HTTP request {requestUri}");

            HttpResponseMessage response = httpClient.PostAsync(requestUri, httpContent).Result;
            ParseResponseMessage(response);
        }

        /// <summary>
        /// Post http web request and parse return value to byte array.
        /// </summary>
        /// <param name="httpClient">HttpClient to use for web request</param>
        /// <param name="requestUri">POST URI of request</param>
        /// <param name="httpContent">Content of request</param>
        /// <returns>Byte array result of response</returns>
        public static byte[] PostReadByteArray(this HttpClient httpClient, Uri requestUri, HttpContent httpContent)
        {
            #region validation

            if (requestUri == null)
            {
                throw new ArgumentNullException(nameof(requestUri));
            }

            if (httpContent == null)
            {
                throw new ArgumentNullException(nameof(httpContent));
            }

            #endregion

            Log.Information($"POST HTTP request {requestUri} with byte array response");

            HttpResponseMessage response = httpClient.PostAsync(requestUri, httpContent).Result;
            return ParseResponseMessageAsByteArray(response);
        }

        /// <summary>
        /// Post http web request and parse return value to stream.
        /// </summary>
        /// <param name="httpClient">HttpClient to use for web request</param>
        /// <param name="requestUri">POST URI of request</param>
        /// <param name="httpContent">Content of request</param>
        /// <returns>Stream result of response</returns>
        public static System.IO.Stream PostReadStream(this HttpClient httpClient, Uri requestUri, HttpContent httpContent)
        {
            #region validation

            if (requestUri == null)
            {
                throw new ArgumentNullException(nameof(requestUri));
            }

            if (httpContent == null)
            {
                throw new ArgumentNullException(nameof(httpContent));
            }

            #endregion

            Log.Information($"POST HTTP request {requestUri} with stream response");

            HttpResponseMessage response = httpClient.PostAsync(requestUri, httpContent).Result;
            return ParseResponseMessageAsStream(response);
        }

        /// <summary>
        /// Put http web request and parse return value to given type.
        /// </summary>
        /// <typeparam name="T">Result type of response</typeparam>
        /// <param name="httpClient">HttpClient to use for web request</param>
        /// <param name="requestUri">PUT URI of request</param>
        /// <param name="httpContent">Content of request</param>
        /// <param name="parseResponseMethod">Method to parse string based response to result</param>
        /// <returns>Typed result of response</returns>
        public static T Put<T>(this HttpClient httpClient, Uri requestUri, HttpContent httpContent, ParseResponse<T> parseResponseMethod)
        {
            #region validation

            if (requestUri == null)
            {
                throw new ArgumentNullException(nameof(requestUri));
            }

            if (httpContent == null)
            {
                throw new ArgumentNullException(nameof(httpContent));
            }

            if (parseResponseMethod == null)
            {
                throw new ArgumentNullException(nameof(parseResponseMethod));
            }

            #endregion

            Log.Information($"PUT HTTP request {requestUri}");

            HttpResponseMessage response = httpClient.PutAsync(requestUri, httpContent).Result;
            return ParseResponseMessageAsString<T>(response, parseResponseMethod);
        }

        /// <summary>
        /// Put http web request and parse return value to given type.
        /// </summary>
        /// <typeparam name="T">Result type of response</typeparam>
        /// <param name="httpClient">HttpClient to use for web request</param>
        /// <param name="requestUri">PUT URI of request</param>
        /// <param name="parseResponseMethod">Method to parse string based response to result</param>
        /// <returns>Typed result of response</returns>
        public static T Put<T>(this HttpClient httpClient, Uri requestUri, ParseResponse<T> parseResponseMethod)
        {
            #region validation

            if (requestUri == null)
            {
                throw new ArgumentNullException(nameof(requestUri));
            }

            if (parseResponseMethod == null)
            {
                throw new ArgumentNullException(nameof(parseResponseMethod));
            }

            #endregion

            Log.Information($"PUT HTTP request {requestUri}");

            HttpResponseMessage response = httpClient.PutAsync(requestUri, null).Result;
            return ParseResponseMessageAsString<T>(response, parseResponseMethod);
        }

        /// <summary>
        /// Put http web request with an empty response.
        /// </summary>
        /// <param name="httpClient">HttpClient to use for web request</param>
        /// <param name="requestUri">PUT URI of request</param>
        public static void Put(this HttpClient httpClient, Uri requestUri, HttpContent httpContent = null)
        {
            #region validation

            if (requestUri == null)
            {
                throw new ArgumentNullException(nameof(requestUri));
            }

            #endregion

            Log.Information($"PUT HTTP request {requestUri}");

            HttpResponseMessage response = httpClient.PutAsync(requestUri, null).Result;
            ParseResponseMessage(response);
        }

        /// <summary>
        /// Delete http web request and parse return value to given type.
        /// </summary>
        /// <typeparam name="T">Result type of response</typeparam>
        /// <param name="httpClient">HttpClient to use for web request</param>
        /// <param name="requestUri">PUT URI of request</param>
        /// <param name="parseResponseMethod">Method to parse string based response to result</param>
        /// <returns>Typed result of response</returns>
        public static T Delete<T>(this HttpClient httpClient, Uri requestUri, ParseResponse<T> parseResponseMethod)
        {
            #region validation

            if (requestUri == null)
            {
                throw new ArgumentNullException(nameof(requestUri));
            }

            if (parseResponseMethod == null)
            {
                throw new ArgumentNullException(nameof(parseResponseMethod));
            }

            #endregion

            Log.Information($"DELETE HTTP request {requestUri}");

            HttpResponseMessage response = httpClient.DeleteAsync(requestUri).Result;
            return ParseResponseMessageAsString<T>(response, parseResponseMethod);
        }

        /// <summary>
        /// Delete http web request with an empty response.
        /// </summary>
        /// <param name="httpClient">HttpClient to use for web request</param>
        /// <param name="requestUri">PUT URI of request</param>
        public static void Delete(this HttpClient httpClient, Uri requestUri)
        {
            #region validation

            if (requestUri == null)
            {
                throw new ArgumentNullException(nameof(requestUri));
            }

            #endregion

            Log.Information($"DELETE HTTP request {requestUri}");

            HttpResponseMessage response = httpClient.DeleteAsync(requestUri).Result;
            ParseResponseMessage(response);
        }

        #region private helper

        /// <summary>
        /// Delegate method to check for empty results.
        /// </summary>
        /// <typeparam name="T">Type of result</typeparam>
        /// <param name="element">Value to check for emptyness</param>
        /// <returns>TRUE if value is null or empty, otherwise FALSE</returns>
        private delegate bool IsEmptyCheck<T>(T value);

        /// <summary>
        /// Delegate method to read content from http response message.
        /// </summary>
        /// <typeparam name="T">Type of result</typeparam>
        /// <param name="responseMessage">Response message of web request to read content from</param>
        /// <returns>Typed result value</returns>
        private delegate T ReadContent<T>(HttpResponseMessage responseMessage);

        /// <summary>
        /// Parse string based response message to result type.
        /// </summary>
        /// <typeparam name="T">Type of response result</typeparam>
        /// <param name="responseMessage">Response message from request</param>
        /// <param name="parseResponseMethod">Method to parse result object</param>
        /// <returns>Typed result of response</returns>
        private static T ParseResponseMessageAsString<T>(HttpResponseMessage responseMessage, ParseResponse<T> parseResponseMethod)
        {
            string responseBody = ParseResponseMessage(responseMessage, StringResult.IsEmpty, StringResult.ReadAsString);
            Log.Debug($"Response is {responseBody}");
            try
            {
                return parseResponseMethod(responseBody);
            }
            catch (Exception exc)
            {
                Log.Error(exc, $"Parsing of response fails!");
                throw new HttpResponseParseException(responseMessage, exc);
            }
        }

        /// <summary>
        /// Parse response from web request as byte array.
        /// </summary>
        /// <param name="responseMessage">Response message from request</param>
        /// <returns>Byte array result of response</returns>
        private static byte[] ParseResponseMessageAsByteArray(HttpResponseMessage responseMessage)
        {
            byte[] responseBody = ParseResponseMessage(responseMessage, ByteArrayResult.IsEmpty, ByteArrayResult.ReadAsByteArray);
            Log.Debug($"Response is byte array with length {responseBody.Length}");

            return responseBody;
        }

        /// <summary>
        /// Parse response from web request as stream.
        /// </summary>
        /// <param name="responseMessage">Response message from request</param>
        /// <returns>Stream result of response</returns>
        private static System.IO.Stream ParseResponseMessageAsStream(HttpResponseMessage responseMessage)
        {
            System.IO.Stream responseBody = ParseResponseMessage(responseMessage, StreamResult.IsEmpty, StreamResult.ReadAsStream);
            Log.Debug($"Response is stream with length {responseBody.Length}");

            return responseBody;
        }

        /// <summary>
        /// Parse response message and check for emptyness.
        /// </summary>
        /// <typeparam name="T">Type of response</typeparam>
        /// <param name="responseMessage">Response message which has been received</param>
        /// <param name="isEmpty">Check if content is empty</param>
        /// <param name="readContent">Method to read content</param>
        /// <returns>Typed response</returns>
        private static T ParseResponseMessage<T>(HttpResponseMessage responseMessage, IsEmptyCheck<T> isEmpty, ReadContent<T> readContent)
        {
            if (responseMessage.IsSuccessStatusCode)
            {
                Log.Information($"Status code of response is {responseMessage.StatusCode}");

                T content = readContent(responseMessage);

                if (!isEmpty(content))
                {
                    return content;
                }

                Log.Error($"Response of {responseMessage.RequestMessage.RequestUri} is empty!");
                throw new EmptyHttpResponseException(responseMessage);
            }

            Log.Error($"Unexpected status code of response {responseMessage.StatusCode} when request {responseMessage.RequestMessage.RequestUri}!");
            throw new UnexpectedHttpStatusCodeExecption(responseMessage);
        }

        /// <summary>
        /// Parse empty response or response where content is not relevant for usecase.
        /// </summary>
        /// <param name="responseMessage">Response message which has been received</param>
        private static void ParseResponseMessage(HttpResponseMessage responseMessage)
        {
            if (responseMessage.IsSuccessStatusCode)
            {
                Log.Information($"Status code of response is {responseMessage.StatusCode}");
                return;
            }

            Log.Error($"Unexpected status code of response {responseMessage.StatusCode} when request {responseMessage.RequestMessage.RequestUri}!");
            throw new UnexpectedHttpStatusCodeExecption(responseMessage);
        }

        #endregion
    }
}
