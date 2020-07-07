using Microsoft.AspNetCore.Mvc;
using Rcv.LabelTool.Backend.Statics;
using Rcv.LabelTool.Contracts.Services;
using System;
using This = Rcv.LabelTool.Logic.ControllerRouters.ImageControllerPaths;

namespace Rcv.LabelTool.Backend.Controllers
{
    /// <summary>
    /// Controller for simple image operations.
    /// </summary>
    [Route(This.ControllerRoute)]
    public class ImagesController : BaseTopicController
    {
        #region member

        /// <summary>
        /// Image service to work with.
        /// </summary>
        private IImageService ImageService { get; set; }


        #endregion

        #region constructor

        public ImagesController(IImageService imageService, ITopicService topicService) : base(topicService)
        {
            #region validation

            if (imageService == null)
            {
                throw new ArgumentNullException(nameof(imageService));
            }

            #endregion

            ImageService = imageService;
        }

        #endregion

        #region ImagesController

        /// <summary>
        /// Get image from topic with given name.
        /// </summary>
        /// <param name="topicId">Id of topic where image is stored</param>
        /// <param name="imageId">Name of image</param>
        /// <returns>Image from topic</returns>
        /// <response code="200">Requested image</response>
        /// <response code="400">Image file name must be provided</response>
        /// <response code="404">The topic or image cannot be found</response>
        /// <response code="500">Unexpected error occures</response>
        [HttpGet]
        [Route(This.GetImageRoute)]
        [ResponseCache(CacheProfileName = CacheProfileNames.Never)]
        public ActionResult GetImage(long topicId, string imageId)
        {
            return Execute(topicId, (topic) =>
            {
                #region validation

                CheckMandatoryParameter(nameof(imageId), imageId);

                if (!ImageService.ExistsImage(topic, imageId))
                {
                    return NotFound();
                }

                #endregion

                return new FileStreamResult(ImageService.GetImage(topic, imageId), MimeMapping.MimeUtility.GetMimeMapping(imageId));
            });
        }

        /// <summary>
        /// Get number of images which are avaiable for topic with given topicId.
        /// </summary>
        /// <param name="topicId">Id of topic</param>
        /// <returns>Number of images in topic</returns>
        /// <response code="200">Number of images in topic</response>
        /// <response code="404">Topic with id not found</response>
        /// <response code="500">Unexpected error occures</response>
        [HttpGet]
        [Route(This.GetImageCountRoute)]
        public ActionResult<uint> ImageCount(long topicId)
        {
            return Execute(topicId, (topic) =>
            {
                return Ok(ImageService.ImageCount(topic));
            });
        }

        #endregion
    }
}