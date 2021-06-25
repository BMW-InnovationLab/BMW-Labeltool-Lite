using Rcv.FileUtils;
using Rcv.LabelTool.Contracts.Models;
using Rcv.LabelTool.Contracts.Repositories;
using RCV.FileContainer.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rcv.LabelTool.Services.Repositories.File
{
    public class ImageFileRepository : IImageFileRepository
    {
        #region members

        /// <summary>
        /// Repository of files.
        /// </summary>
        private IFileContainer FileContainer { get; }

        #endregion

        #region constructor

        public ImageFileRepository(IFileContainer fileContainer)
        {
            FileContainer = fileContainer ?? throw new ArgumentNullException(nameof(fileContainer));
        }

        #endregion

        #region IImageRepository        

        public void DeleteImageFile(Topic topic, string imageFileName)
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

            // remove file from repository
            FileContainer.DeleteFile(imageFileName, imageFolderPath);
        }

        public System.IO.Stream GetImageFile(Topic topic, string imageFileName)
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

        public string[] AddImageFile(Topic topic, string fileName, System.IO.Stream imageStream)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (imageStream == null)
            {
                throw new ArgumentNullException(nameof(imageStream));
            }

            #endregion

            string[] imageFolder = GetImageFolder(topic);

            fileName = fileName.Replace(' ', '_');

            // check for filenames which already exists. 
            // check should ignore file extension, filenames are equal even if extensions are inequal (003.jpg and 003.png)
            IEnumerable<string> fileNames = FileContainer.GetFiles(imageFolder).Select(o => FileNameUtil.GetFileNameWithoutExtension(o).ToLower());
            // generate file name if file name already exists
            while (fileNames.Contains(FileNameUtil.GetFileNameWithoutExtension(fileName.ToLower())))
            {
                string fileExtension = FileNameUtil.GetFileExtension(fileName);
                fileName = $"{Guid.NewGuid().ToString()}{fileExtension}";
            }

            return FileContainer.CreateFile(fileName, imageStream, imageFolder);
        }

        #endregion

        #region private helper

        private string[] GetImageFolder(Topic topic)
        {
            return FileContainer.CreateDirectory("images", new[] { topic.FolderPath });
        }

        #endregion
    }
}
