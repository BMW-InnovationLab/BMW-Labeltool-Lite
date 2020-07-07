using System;

namespace Rcv.LabelTool.Web.Utilities
{
    /// <summary>
    /// Utility for filename operations.
    /// </summary>
    public class FileNameUtil
    {
        /// <summary>
        /// Extracts filename from given path. Filename is returned with extension!
        /// </summary>
        /// <param name="filePath">Path to file</param>
        /// <returns>Filename with extension</returns>
        public static string GetFileNameFromPath(string filePath)
        {
            #region validation

            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            #endregion

            if (IsWebFilePath(filePath))
            {
                filePath = ExtractWebFileName(filePath);
            }

            return System.IO.Path.GetFileName(filePath);
        }

        /// <summary>
        /// Extracts filename without extension from given path.
        /// </summary>
        /// <param name="filePath">Path to file</param>
        /// <returns>Filename without extension</returns>
        public static string GetFileNameWithoutExtension(string filePath)
        {
            #region validation

            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            #endregion

            if (IsWebFilePath(filePath))
            {
                filePath = ExtractWebFileName(filePath);
            }

            return System.IO.Path.GetFileNameWithoutExtension(filePath);
        }

        /// <summary>
        /// Extracts only fileextension (with leading dot) from given path.
        /// </summary>
        /// <param name="filePath">Path to file</param>
        /// <returns>Fileextension</returns>
        public static string GetFileExtension(string filePath)
        {
            #region validation

            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            #endregion

            if (IsWebFilePath(filePath))
            {
                filePath = ExtractWebFileName(filePath);
            }

            return System.IO.Path.GetExtension(filePath);
        }

        #region privat helper - web files

        /// <summary>
        /// Checks if filePath is URL. 
        /// </summary>
        /// <param name="filePath">Filepath to Check</param>
        /// <returns>TRUE if url, otherwise FALSE</returns>
        private static bool IsWebFilePath(string filePath)
        {
            return filePath.StartsWith("http://") || filePath.StartsWith("https://");
        }

        /// <summary>
        /// Extract filename from url.
        /// </summary>
        /// <param name="filePath">URL to extract filename from</param>
        /// <returns>Filename with extension</returns>
        private static string ExtractWebFileName(string filePath)
        {
            string fileName = filePath;
            if (fileName.Contains('?'))
            {
                // remove following parameters
                fileName = fileName.Substring(0, fileName.IndexOf('?'));
            }

            if (fileName.Contains('/'))
            {
                // remove leading uri
                fileName = fileName.Substring(fileName.LastIndexOf('/') + 1);
            }

            return fileName;
        }

        #endregion
    }
}
