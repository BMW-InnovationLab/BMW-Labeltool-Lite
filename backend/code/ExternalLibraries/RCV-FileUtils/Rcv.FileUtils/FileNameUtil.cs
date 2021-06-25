using System;
using System.IO;

namespace Rcv.FileUtils
{
    /// <summary>
    /// Utility for filename operations.
    /// </summary>
    public static class FileNameUtil
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

            if (IsURL(filePath))
            {
                filePath = ExtractFileNameFromURL(filePath);
            }

            return Path.GetFileName(filePath);
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

            if (IsURL(filePath))
            {
                filePath = ExtractFileNameFromURL(filePath);
            }

            return Path.GetFileNameWithoutExtension(filePath);
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

            if (IsURL(filePath))
            {
                filePath = ExtractFileNameFromURL(filePath);
            }

            return Path.GetExtension(filePath);
        }

        /// <summary>
        /// Extracts only fileextension (in lower case letters with leading dot) from given path.
        /// </summary>
        /// <param name="filePath">Path to file</param>
        /// <returns>Fileextension</returns>
        public static string GetLowercaseFileExtension(string filePath)
        {
            #region validation

            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            #endregion

            string fileExtension = GetFileExtension(filePath);
            if (!string.IsNullOrEmpty(fileExtension))
            {
                fileExtension = fileExtension.ToLower();
            }
            return fileExtension;
        }

        #region privat helper - web files

        /// <summary>
        /// Checks if filePath is URL. 
        /// </summary>
        /// <param name="filePath">Filepath to Check</param>
        /// <returns>TRUE if url, otherwise FALSE</returns>
        private static bool IsURL(string filePath)
        {
            return filePath.StartsWith("http://") || filePath.StartsWith("https://") || filePath.StartsWith("ftp://");
        }

        /// <summary>
        /// Extract filename from url.
        /// </summary>
        /// <param name="filePath">URL to extract filename from</param>
        /// <returns>Filename with extension</returns>
        private static string ExtractFileNameFromURL(string filePath)
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
