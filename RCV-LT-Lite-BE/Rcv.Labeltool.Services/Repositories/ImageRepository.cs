using Microsoft.AspNetCore.Http;
using Rcv.LabelTool.Contracts.Models;
using Rcv.LabelTool.Contracts.Repositories;
using Rcv.LabelTool.Logic.ControllerRouters;
using Rcv.LabelTool.Logic.Utilities;
using Rcv.LabelTool.Web.Utilities;
using RCV.FileContainer.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rcv.LabelTool.Web.Repositories
{
    public class ImageRepository : IImageRepository
    {
        #region members

        /// <summary>
        /// Repository of files.
        /// </summary>
        private IFileContainer FileContainer { get; }

        private IHttpContextAccessor HttpContextAccessor { get; }

        #endregion

        #region constructor

        public ImageRepository(IFileContainer fileContainer, IHttpContextAccessor httpContextAccessor)
        {
            FileContainer = fileContainer ?? throw new ArgumentNullException(nameof(fileContainer));
            HttpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        #endregion

        #region IImageRepository

        public IEnumerable<ImageLabel> GetAll(Topic topic, uint startPosition = 0)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            #endregion

            IEnumerable<ImageLabel> imageLabels = GetImageLabels(topic);

            if (startPosition > 0)
            {
                imageLabels = imageLabels.Skip((int)startPosition);
            }

            return imageLabels;
        }

        public ImageLabel GetImageByIndex(Topic topic, uint index)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            #endregion

            // get all imagefile paths
            IEnumerable<string> imageFilePaths = GetImagePaths(topic);
            if (imageFilePaths.Count() > index)
            {
                // get imagefilepath for specified index
                string imagePath = GetImagePaths(topic).ElementAt((int)index);

                // create default imagelabel and set index
                ImageLabel imageLabel = CreateImageLabel(imagePath);
                imageLabel.Index = index;
                imageLabel.SetImageUrl(HttpContextAccessor.HttpContext, topic);
                SetImageSize(topic, imageLabel);

                return imageLabel;
            }

            return null;
        }

        public ImageLabel GetImageLabelById(Topic topic, string imageId)
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

            // get all imagefile paths
            List<string> imageFilePaths = new List<string>(GetImagePaths(topic));

            foreach (string imageFilePath in imageFilePaths)
            {
                if (imageId.Equals(FileNameUtil.GetFileNameFromPath(imageFilePath)))
                {
                    //create default imagelabel and set index
                    ImageLabel imageLabel = CreateImageLabel(imageFilePath);
                    imageLabel.Index = imageFilePaths.IndexOf(imageFilePath);
                    imageLabel.SetImageUrl(HttpContextAccessor.HttpContext, topic);
                    SetImageSize(topic, imageLabel);


                    return imageLabel;
                }
            }

            return null;
        }

        public uint GetImageIndexByStoragePath(Topic topic, string imageStoragePath)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            if (string.IsNullOrEmpty(imageStoragePath))
            {
                throw new ArgumentNullException(nameof(imageStoragePath));
            }

            #endregion

            // get all imagefile paths
            List<string> imageFilePaths = new List<string>(GetImagePaths(topic));

            // get index of specified storage path
            int imageIndex = imageFilePaths.IndexOf(imageStoragePath);

            if (imageIndex != -1)
            {
                return (uint)imageIndex;
            }

            return 0;
        }

        public uint GetImageIndexByImageFileName(Topic topic, string imageFileName)
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

            // get all imagefile paths
            List<string> imageFilePaths = new List<string>(GetImagePaths(topic));

            // extract filenames from filepaths and search for equality
            uint imageIndex = 0;
            foreach (string imageFilePath in imageFilePaths)
            {
                string imageName = FileNameUtil.GetFileNameFromPath(imageFilePath);
                if (imageFileName == imageName)
                {
                    return imageIndex;
                }
                imageIndex++;
            }

            return 0;
        }

        public uint CountAll(Topic topic)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            #endregion

            // index all filenames and count
            return (uint)GetImagePaths(topic).Count();
        }

        public System.IO.Stream GetImage(Topic topic, string imageFileName)
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

            // get path to imagefolder
            string[] imageFolderPath = GetImageFolder(topic);

            return FileContainer.GetFileStream(imageFileName, imageFolderPath);
        }

        public bool ExistsImage(Topic topic, string imageName)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            if (string.IsNullOrEmpty(imageName))
            {
                throw new ArgumentNullException(nameof(imageName));
            }

            #endregion

            string[] imageFolder = GetImageFolder(topic);
            return FileContainer.ExistsFile(imageName, imageFolder);
        }

        public void SetImageSize(Topic topic, ImageLabel imageLabel)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            if (imageLabel == null)
            {
                throw new ArgumentNullException(nameof(imageLabel));
            }

            #endregion

            using (System.IO.Stream imageStream = FileContainer.GetFileStream(imageLabel.Id, GetImageFolder(topic)))
            {
                Rect imageDimension = ImageUtil.GetDimensionFromImage(imageStream);

                imageLabel.Height = imageDimension.Height;
                imageLabel.Width = imageDimension.Width;
            }
        }

        #endregion

        #region private helper

        private IEnumerable<ImageLabel> GetImageLabels(Topic topic)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            #endregion

            List<ImageLabel> imageLabelList = new List<ImageLabel>();

            long index = 0L;
            foreach (string imagePath in GetImagePaths(topic))
            {
                string imageFileName = FileNameUtil.GetFileNameFromPath(imagePath);

                ImageLabel imgLabel = CreateImageLabel(imagePath);
                imgLabel.Index = index++;
                imgLabel.SetImageUrl(HttpContextAccessor.HttpContext, topic);

                imageLabelList.Add(imgLabel);
            }

            return imageLabelList;
        }

        /// <summary>
        /// Get paths of all images (only with valid extension).
        /// </summary>
        /// <param name="topic">The topic</param>
        /// <returns>Paths to all images of topic</returns>
        private IEnumerable<string> GetImagePaths(Topic topic)
        {
            // get image path
            string[] imageFolderPath = GetImageFolder(topic);
            return ImagePathUtil.GetValidImagePaths(FileContainer.GetFiles(imageFolderPath));
        }

        private ImageLabel CreateImageLabel(string imageFilePath)
        {
            string imageFileName = FileNameUtil.GetFileNameFromPath(imageFilePath);

            return new ImageLabel()
            {
                StoragePath = imageFilePath,
                Id = imageFileName,
                HasLabels = false
            };
        }

        private string[] GetImageFolder(Topic topic)
        {
            return FileContainer.CreateDirectory("images", new[] { topic.FolderPath });
        }

        #endregion
    }
}