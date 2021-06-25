using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rcv.Base.Mime;
using Rcv.LabelTool.Contracts.Models;
using Rcv.LabelTool.Contracts.Services;
using System;

namespace Rcv.LabelTool.Backend.Controllers
{
    /// <summary>
    /// Controller for image upload operations.
    /// </summary>
    [Route("api/images")]
    public class ImagesUploadController : BaseTopicController
    {
        #region member

        /// <summary>
        /// Image service to work with.
        /// </summary>
        private IImageUploadService ImageUploadService { get; }

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new controller instance.
        /// </summary>
        /// <param name="imageUploadService">Image upload service</param>
        /// <param name="topicService">Topic service</param>
        public ImagesUploadController(IImageUploadService imageUploadService, ITopicService topicService) : base(topicService)
        {
            ImageUploadService = imageUploadService ?? throw new ArgumentNullException(nameof(imageUploadService));
        }

        #endregion

        #region ImagesUploadController

        /// <summary>
        /// Add image to given topic. Image is stored in request files.
        /// Only first image of request will be processed
        /// </summary>
        /// <param name="topicId">Id of topic where to add image</param>
        /// <param name="file">Image to add to topic</param>
        /// <returns>Imagelabel structure of added image</returns>
        /// <response code="200">Imagelabel structure of added image</response>
        /// <response code="400">One or more parameters are not valid</response>
        /// <response code="404">Topic with id not found</response>
        /// <response code="415">Unsupported content type of file</response>
        /// <response code="500">Unexpected error occures</response>
        [HttpPost("{topicId}")]
        [Consumes(ContentTypeNames.Multipart.FormData)]
        public ActionResult<ImageLabel> AddImage(long topicId, IFormFile file)
        {
            return Execute(topicId, (topic) =>
            {
                #region validation

                CheckMandatoryParameter(nameof(file), file);
                CheckImageContentType(nameof(file), file);

                #endregion

                ImageLabel result = ImageUploadService.UploadImage(topic, System.IO.Path.GetFileName(file.FileName), file.OpenReadStream());

                return Ok(result);
            });

        }

        #endregion
    }
}