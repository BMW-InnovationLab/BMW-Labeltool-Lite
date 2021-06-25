using Rcv.FileUtils;
using Rcv.LabelTool.Contracts.Models;
using Rcv.LabelTool.Contracts.Repositories;
using Rcv.LabelTool.Logic.Utilities;
using RCV.FileContainer.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rcv.LabelTool.Web.Repositories
{
    public class ImageRepository : IImageDataRepository
    {
        #region members

        /// <summary>
        /// Repository of files.
        /// </summary>
        private IFileContainer FileContainer { get; }

        private ILabelRepository LabelRepository { get; }

        #endregion

        #region constructor

        public ImageRepository(IFileContainer fileContainer, ILabelRepository labelRepository)
        {
            FileContainer = fileContainer ?? throw new ArgumentNullException(nameof(fileContainer));
            LabelRepository = labelRepository ?? throw new ArgumentNullException(nameof(labelRepository));
        }

        #endregion

        #region IImageRepository

        public IList<ImageLabel> GetImages(Topic topic)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            #endregion

            IList<ImageLabel> images = GetImagesInternal(topic);

            SetHasLabels(topic, images);

            return images;
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
                    return CreateImage(topic, imageFilePaths.IndexOf(imageFilePath), imageFilePath, true);
                }
            }

            return null;
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

        public void DeleteImage(Topic topic, string imageFileName)
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

            // nothing to do
            // because it is done throug ImageFileRepository
        }

        public ImageLabel AddImage(Topic topic, ImageLabel image)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            #endregion

            // nothing to do
            // because it is done throug ImageFileRepository

            return GetImageLabelById(topic, image.Id);
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

        public void SetImageSize(Topic topic, ImageLabel image)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            #endregion

            using (System.IO.Stream imageStream = FileContainer.GetFileStream(image.Id, GetImageFolder(topic)))
            {
                if (ImageUtil.IsValidImage(imageStream, image.Id))
                {
                    Rect imageDimension = ImageUtil.GetDimensionFromImage(imageStream);

                    image.Height = imageDimension.Height;
                    image.Width = imageDimension.Width;
                }
            }
        }

        #endregion

        #region private helper

        private IList<ImageLabel> GetImagesInternal(Topic topic)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            #endregion

            List<ImageLabel> images = new List<ImageLabel>();

            long index = 0L;
            foreach (string imagePath in GetImagePaths(topic))
            {
                FileNameUtil.GetFileNameFromPath(imagePath);
                ImageLabel image = CreateImage(topic, index++, imagePath);
                images.Add(image);
            }

            return images;
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

        /// <summary>
        /// Create image structure for image from given topic.
        /// 
        /// Fullfilled image contains of:
        ///   <ul>
        ///     <li>Width and height of image</li>
        ///     <li>HasClassifications - Flag</li>
        ///     <li>HasLabels - Flag</li>
        ///     <li>HasSegments - Flag</li>
        ///   </ul>
        /// </summary>
        /// <param name="topic">Topic where image is stored</param>
        /// <param name="index">Index of image in topic</param>
        /// <param name="imageFilePath">Path to image in filesystem</param>
        /// <param name="full">TRUE if fullfilled object shoul be returned</param>
        /// <returns>ImageLabelstructure</returns>
        private ImageLabel CreateImage(Topic topic, long index, string imageFilePath, bool full = false)
        {
            string imageFileName = FileNameUtil.GetFileNameFromPath(imageFilePath);

            ImageLabel result = new ImageLabel()
            {
                StoragePath = imageFilePath,
                Id = imageFileName,
                Index = index
            };

            if (full)
            {
                SetHasLabels(topic, new List<ImageLabel>() { result });

                SetImageSize(topic, result);
            }

            return result;
        }

        private string[] GetImageFolder(Topic topic)
        {
            return FileContainer.CreateDirectory("images", new[] { topic.FolderPath });
        }

        private void SetHasLabels(Topic topic, IEnumerable<ImageLabel> images)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            if (images == null)
            {
                throw new ArgumentNullException(nameof(images));
            }

            #endregion

            IEnumerable<string> labeledImageFilesWithoutExtension = LabelRepository.GetLabelFileNames(topic);
            foreach (ImageLabel image in images)
            {
                image.HasLabels = labeledImageFilesWithoutExtension.Contains(FileNameUtil.GetFileNameWithoutExtension(image.Id));
            }
        }

        #endregion
    }
}