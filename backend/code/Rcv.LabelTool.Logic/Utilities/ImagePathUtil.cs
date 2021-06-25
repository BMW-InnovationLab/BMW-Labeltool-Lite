using Rcv.FileUtils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rcv.LabelTool.Logic.Utilities
{
    public static class ImagePathUtil
    {
        /// <summary>
        /// List of valid image extensions.
        /// </summary>
        public static IEnumerable<string> ValidImageExtensions { get; set; }

        #region methods

        /// <summary>
        /// Get valid image paths from list of given file paths.
        /// </summary>
        /// <param name="filePaths">List of file paths</param>
        /// <returns>List of image paths</returns>
        public static IEnumerable<string> GetValidImagePaths(IEnumerable<string> filePaths)
        {
            #region validation

            if (filePaths == null)
            {
                throw new ArgumentNullException(nameof(filePaths));
            }

            #endregion

            List<string> imageFileList = new List<string>();
            if (ValidImageExtensions != null && ValidImageExtensions.Count() > 0)
            {
                foreach (string validImageExtension in ValidImageExtensions)
                {
                    imageFileList.AddRange(filePaths.Where(o => FileNameUtil.GetFileExtension(o).Equals(validImageExtension, StringComparison.InvariantCultureIgnoreCase)));
                }
            }
            else
            {
                // return all files if no filter is set.
                return filePaths;
            }
            return imageFileList;
        }

        #endregion
    }
}
