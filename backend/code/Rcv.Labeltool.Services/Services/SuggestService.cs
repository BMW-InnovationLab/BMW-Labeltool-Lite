using Rcv.HttpUtils;
using Rcv.LabelTool.Contracts.Configurations;
using Rcv.LabelTool.Contracts.Models;
using Rcv.LabelTool.Contracts.Models.Results.Suggest;
using Rcv.LabelTool.Contracts.Repositories;
using Rcv.LabelTool.Contracts.Services;
using Rcv.LabelTool.Logic.Logic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;

namespace Rcv.LabelTool.Services.Services
{
    public class SuggestService : ISuggestService
    {
        #region member

        private IImageFileRepository ImageFileRepository { get; }

        private IHttpClientFactory HttpClientFactory { get; }

        private IInferenceConfiguration InferenceConfiguration { get; }

        #endregion

        #region constructor

        public SuggestService(IHttpClientFactory httpClientFactory, IImageFileRepository imageRepository, IInferenceConfiguration inferenceConfiguration)
        {
            ImageFileRepository = imageRepository ?? throw new ArgumentNullException(nameof(imageRepository));
            HttpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            InferenceConfiguration = inferenceConfiguration ?? throw new ArgumentNullException(nameof(inferenceConfiguration));
        }

        #endregion

        #region ISuggestService

        public IEnumerable<Label> GetBoundingBoxes(Topic topic, string imageId)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            if (string.IsNullOrEmpty(imageId))
            {
                throw new ArgumentNullException(nameof(imageId));
            }

            #endregion

            var modelId = LoadModel();

            MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
            using (Stream imageFileStream = ImageFileRepository.GetImageFile(topic, imageId))
            {
                multipartFormDataContent.AddFileAsByteContent(imageFileStream, "image", imageId);
            }
            multipartFormDataContent.Add(new StringContent(modelId), "model");

            HttpClient httpClient = HttpClientFactory.GetInferenceContainerHttpClient();

            Uri uri = UriUtil.GetUri(httpClient.BaseAddress, "detect");

            using MemoryStream memoryStream = new MemoryStream(httpClient.PostReadByteArray(uri, multipartFormDataContent));
            using StreamReader streamReader = new StreamReader(memoryStream);
            return ParseLabelResult(streamReader.ReadToEnd());
        }

        #endregion

        #region private helper

        /// <summary>
        /// Parse response from detectnet webapi and convert to list of labels.
        /// </summary>
        /// <param name="response">Response from detectnet webapi</param>
        /// <returns>List of labels which have been found</returns>
        public static IEnumerable<Label> ParseLabelResult(string response)
        {
            #region validation

            if (string.IsNullOrEmpty(response))
            {
                throw new ArgumentNullException(nameof(response));
            }

            #endregion

            DetectNetDetectionResponse result = JsonSerializer.Deserialize<DetectNetDetectionResponse>(response);

            List<Label> resultList = new List<Label>();
            foreach (DetectNetBoundingBox boundingBox in result.BoundingBoxes)
            {
                resultList.Add(new Label()
                {
                    Left = boundingBox.Coordinates.Left,
                    Right = boundingBox.Coordinates.Right,
                    Top = boundingBox.Coordinates.Top,
                    Bottom = boundingBox.Coordinates.Bottom,
                    Confidence = boundingBox.Confidence,
                    ObjectClassName = boundingBox.ObjectClassName,
                    ObjectClassId = boundingBox.ObjectClassId
                });
            }
            return resultList;
        }

        /// <summary>
        /// Parse response from webapi and convert to model.
        /// </summary>
        /// <param name="response">Response of webapi</param>
        /// <returns>Model to use for boundingbox prediction</returns>
        public static string ParseModelResult(string response)
        {
            Dictionary<string, string> models = JsonSerializer.Deserialize<Dictionary<string, string>>(response);
            return models.First().Value;
        }

        /// <summary>
        /// Load model to use for boundingbox prediction.
        /// </summary>
        /// <returns>Model name</returns>
        public string LoadModel()
        {
            HttpClient httpClient = HttpClientFactory.GetInferenceContainerHttpClient();

            Uri uri = UriUtil.GetUri(httpClient.BaseAddress, "load");

            return httpClient.Get<string>(uri, ParseModelResult);
        }

        #endregion

    }
}
