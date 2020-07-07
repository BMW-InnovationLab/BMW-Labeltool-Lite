using Microsoft.AspNetCore.Http;
using Rcv.LabelTool.Contracts.Models;
using System;

namespace Rcv.LabelTool.Logic.ControllerRouters
{
    /// <summary>
    /// Paths of image controller.
    /// </summary>
    public static class ImageControllerPaths
    {
        #region routes

        /// <summary>
        /// Route of controller.
        /// </summary>
        public const string ControllerRoute = "api/images";

        /// <summary>
        /// Route of GET image endpoint.
        /// </summary>
        public const string GetImageRoute = "v2/" + TopicRouteParameter + "/" + ImageFileNameRouteParameter;

        /// <summary>
        /// Route of GET image count endpoint.
        /// </summary>
        public const string GetImageCountRoute = TopicRouteParameter + "/imagecount";

        /// <summary>
        /// Route of DELETE image endpoint.
        /// </summary>
        public const string DeleteImageRoute = TopicRouteParameter + "/" + ImageFileNameRouteParameter + "/e";

        #endregion

        #region parameter

        /// <summary>
        /// Parameter representation of topicId route parameter.
        /// </summary>
        public const string TopicRouteParameter = "{topicId}";

        /// <summary>
        /// Parameter representation of imageFileName route parameter.
        /// </summary>
        public const string ImageFileNameRouteParameter = "{imageId}";

        #endregion

        #region static methods

        /// <summary>
        /// Set imageurl of imagelabel.
        /// </summary>
        /// <param name="imageLabel">Imagelabel to set imageurl of</param>
        /// <param name="httpContext">Http context of application</param>
        /// <param name="topic">Topic of imagelabel</param>
        public static void SetImageUrl(this ImageLabel imageLabel, HttpContext httpContext, Topic topic)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            #endregion

            imageLabel.Url = GetImageUrl(httpContext, topic, imageLabel.Id);
            imageLabel.Path = GetImagePath(topic, imageLabel.Id);
        }

        /// <summary>
        /// Get imageurl for given topic and imagefilename.
        /// </summary>
        /// <param name="httpContext">Http context of application</param>
        /// <param name="topic">Topic of imagefile</param>
        /// <param name="imageFileName">Name of imagefile</param>
        /// <returns>Url of image</returns>
        public static string GetImageUrl(HttpContext httpContext, Topic topic, string imageFileName)
        {
            #region validation

            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            if (string.IsNullOrEmpty(imageFileName))
            {
                throw new ArgumentNullException(nameof(imageFileName));
            }

            #endregion

            string route = $"{httpContext.Request.Scheme}://{httpContext.Request.Host.Host}";
            if (httpContext.Request.Host.Port.HasValue)
            {
                route += $":{httpContext.Request.Host.Port}";
            }
            route += GetImagePath(topic, imageFileName);

            return route;
        }

        /// <summary>
        /// Get imagepath of given topic and imagefilename.
        /// </summary>
        /// <param name="topic">Topic of imagefile</param>
        /// <param name="imageFileName">Name of imagefile</param>
        /// <returns>Url of image</returns>
        public static string GetImagePath(Topic topic, string imageFileName)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            if (string.IsNullOrEmpty(imageFileName))
            {
                throw new ArgumentNullException(nameof(imageFileName));
            }

            #endregion

            string route = $"/{ControllerRoute}/{GetImageRoute}";
            route = route.Replace(TopicRouteParameter, topic.Id.ToString());
            route = route.Replace(ImageFileNameRouteParameter, imageFileName);

            return route;
        }

        #endregion
    }
}