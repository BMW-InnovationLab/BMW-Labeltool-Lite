using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RCV.FileContainer.Contracts;

namespace RCV.FileContainer.Extensions
{
    /// <summary>
    /// Extension for file container
    /// </summary>
    public static class FileContainerExtensions
    {
        /// <summary>
        /// Copy all files and directories from source file container to target file container.
        /// </summary>
        /// <param name="sourceFileContainer">Source file container where files are stored</param>
        /// <param name="sourceDataPath">Path where source data can be found</param>
        /// <param name="targetFileContainer">Target file container where files should be stored</param>
        /// <param name="targetDataPath">Path where target data should be stored</param>
        public static void CopyTo(this IFileContainer sourceFileContainer, string[] sourceDataPath, IFileContainer targetFileContainer, string[] targetDataPath)
        {
            #region validation
            
            if (sourceFileContainer == null)
            {
                throw new ArgumentNullException(nameof(sourceFileContainer));
            }

            if (sourceDataPath == null)
            {
                throw new ArgumentNullException(nameof(sourceDataPath));
            }

            if (targetFileContainer == null)
            {
                throw new ArgumentNullException(nameof(targetFileContainer));
            }

            if (targetDataPath == null)
            {
                throw new ArgumentNullException(nameof(targetDataPath));
            }

            #endregion

            foreach (string directoryName in sourceFileContainer.GetDirectoryNames("*", sourceDataPath))
            {
                string[] subTargetPath = AddPath(targetDataPath, directoryName);
                string[] subSourcePath = AddPath(sourceDataPath, directoryName);
                targetFileContainer.CreateDirectory(directoryName, targetDataPath);

                sourceFileContainer.CopyTo(subSourcePath, targetFileContainer, subTargetPath);
            }
            foreach (string fileName in sourceFileContainer.GetFileNames("*", sourceDataPath))
            {
                using (Stream fileStream = sourceFileContainer.GetFileStream(fileName, sourceDataPath))
                {
                    targetFileContainer.SetFileStream(fileName, fileStream, targetDataPath);
                }
            }
        }

        /// <summary>
        /// Evaluates given path and creates directory
        /// </summary>
        /// <param name="fileContainer">FileContainer to extend</param>
        /// <param name="path">Directory Path</param>
        /// <returns>Path segments of created directory</returns>
        public static string[] CreateDirectoryPath(this IFileContainer fileContainer, string[] path)
        {
            #region validation

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            #endregion

            switch (path.Length)
            {
                case 0:
                    return new string[] { };
                case 1:
                {
                    string directoryName = path.First();

                    return fileContainer.CreateDirectory(directoryName);
                }
                default:
                {
                    string directoryName = path.Last();

                    string[] pathSegments = path.SkipLast(1).ToArray();

                    return fileContainer.CreateDirectory(directoryName, pathSegments);
                }
            }
        }

        /// <summary>
        /// Checks existence of a given directory path
        /// </summary>
        /// <param name="fileContainer">FileContainer to extend</param>
        /// <param name="path">Directory path</param>
        /// <returns>TRUE if directory exists; False if directory does not exist</returns>
        public static bool DirectoryPathExists(this IFileContainer fileContainer, string[] path)
        {
            #region validation

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            #endregion

            switch (path.Length)
            {
                case 0:
                    return fileContainer.ExistsDirectory(string.Empty);
                case 1:
                {
                    string directoryName = path.First();

                    return fileContainer.ExistsDirectory(directoryName);

                }
                default:
                {
                    string directoryName = path.Last();

                    string[] pathSegments = path.Take(path.Length - 1).ToArray();

                    return fileContainer.ExistsDirectory(directoryName, pathSegments);
                }
            }
        }

        /// <summary>
        /// Utilizes <see cref="IFileContainer"/> implementation of GetFileNames(...) and cleanses off the filename extensions.
        /// </summary>
        /// <param name="fileContainer">File container to extend</param>
        /// <param name="searchPattern">Search pattern to apply to the list of files</param>
        /// <param name="path">Path of the parent directory which contains the desired files</param>
        /// <returns>A list of file names at a specific location without filename extensions</returns>
        public static IEnumerable<string> GetFileNamesWithoutExtensions(this IFileContainer fileContainer, string searchPattern, string[] path = null)
        {
            #region validation

            if (fileContainer == null)
            {
                throw new ArgumentNullException(nameof(fileContainer));
            }

            if (string.IsNullOrEmpty(searchPattern))
            {
                throw new ArgumentNullException(nameof(searchPattern));
            }

            #endregion

            IEnumerable<string> fileNames = fileContainer.GetFileNames(searchPattern, path);

            return fileNames.Select(Path.GetFileNameWithoutExtension);
        }

        /// <summary>
        /// Extracts the filename from a given path
        /// </summary>
        /// <param name="path">Path to parse the filename from</param>
        /// <returns>Filename parsed from path</returns>
        public static string GetFileNameFromPath(string path)
        {
            #region validation

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            #endregion

            char separator = path.Contains("\\") ? 
                Path.DirectorySeparatorChar : 
                '/';

            int pos = path.LastIndexOf(separator) + 1;

            string fileName = path.Substring(pos, path.Length - pos);

            return fileName.Contains('?') ? 
                fileName.Substring(0, fileName.IndexOf('?')) : 
                fileName;
        }

        #region private helper

        /// <summary>
        /// Add sub-directory path to current path array.
        /// </summary>
        /// <param name="array">Array to extend</param>
        /// <param name="element">Element to add</param>
        /// <returns>Extended path</returns>
        private static string[] AddPath(this IEnumerable<string> array, string element)
        {
            #region validation

            if (string.IsNullOrEmpty(element))
            {
                throw new ArgumentNullException(nameof(element));
            }

            #endregion

            var list = new List<string>(array)
            {
                element
            };

            return list.ToArray();
        }

        #endregion
    }
}