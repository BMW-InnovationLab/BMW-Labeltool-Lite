using Rcv.LabelTool.Contracts.Enumerations;
using System;
using System.Net.Http;

namespace Rcv.LabelTool.Logic.Logic
{
    /// <summary>
    /// Extension methods for http client factory.
    /// </summary>
    public static class HttpClientFactoryExtensions
    {
        /// <summary>
        /// Get http client from factory.
        /// </summary>
        /// <param name="httpClientFactory">Factory for http clients</param>
        /// <param name="httpClient">Type of httpClient</param>
        /// <returns>Http client to communicate with</returns>
        public static HttpClient GetHttpClient(this IHttpClientFactory httpClientFactory, EHttpClients httpClient)
        {
            #region validation

            if (httpClientFactory is null)
            {
                throw new ArgumentNullException(nameof(httpClientFactory));
            }

            #endregion

            return httpClientFactory.CreateClient(httpClient.ToString());
        }

        /// <summary>
        /// Get http client for communication with inference container.
        /// </summary>
        /// <param name="httpClientFactory">Factory for http clients</param>
        /// <returns>Http client for communication with segmentation backend</returns>
        public static HttpClient GetInferenceContainerHttpClient(this IHttpClientFactory httpClientFactory)
        {
            #region validation

            if (httpClientFactory is null)
            {
                throw new ArgumentNullException(nameof(httpClientFactory));
            }

            #endregion

            return httpClientFactory.GetHttpClient(EHttpClients.InferenceContainerClient);
        }
    }
}
