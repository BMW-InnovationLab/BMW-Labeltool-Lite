using System;
using System.Collections.Generic;
using System.IO;

namespace RCV.FileContainer.Utilities
{
    /// <summary>
    /// Utility class for file based storage.
    /// </summary>
    public class FileUtilities
    {
        #region properties

        private string RootPath { get; }

        #endregion

        #region constructor

        /// <summary>
        /// Constructor for the file based utility class.
        /// </summary>
        /// <param name="rootPath">Root path of the directory which the class will be wrapped around.</param>
        public FileUtilities(string rootPath)
        {
            #region validation

            if (string.IsNullOrEmpty(rootPath))
            {
                throw new ArgumentNullException(nameof(rootPath));
            }

            #endregion

            RootPath = rootPath;
        }

        #endregion

        #region methods

        /// <summary>
        /// Builds up a relative path based on specified segment parts.
        /// </summary>
        /// <param name="path">Path segments</param>
        /// <returns>Returns a relative path.</returns>
        public string GetPath(string[] path)
        {
            #region validation

            if (path == null)
            {
                return RootPath;
            }

            #endregion

            // create result path
            string resultPath = Path.Combine(path);

            // prepend root path with corrected separators
            string cleansedRoot = RootPath.Trim('/', '\\').Replace('/', '\\');
            if (!resultPath.StartsWith(cleansedRoot))
            {
                resultPath = Path.Join(cleansedRoot, resultPath);
            }

            // if root path is relative, add relative separator
            if (RootPath.StartsWith('/'))
            {
                resultPath = '/' + resultPath;
            }

            return resultPath;
        }

        /// <summary>
        /// Builds up a relative path based on specified segment parts.
        /// </summary>
        /// <param name="name">Name of blob or directory</param>
        /// <param name="path">Path segments</param>
        /// <returns>Returns a relative path.</returns>
        public string GetPath(string name, string[] path = null)
        {
            #region validation

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            #endregion

            var pathList = new List<string>();

            if (path != null)
            {
                pathList.AddRange(path);
            }

            pathList.Add(name);

            return GetPath(pathList.ToArray());
        }

        /// <summary>
        /// Builds up a relative path based on specified segment parts.
        /// </summary>
        /// <param name="name">Name of blob or directory</param>
        /// <param name="path">Path as string</param>
        /// <returns>Returns a relative path.</returns>
        public string GetPath(string name, string path = null)
        {
            #region validation

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            #endregion

            return !string.IsNullOrEmpty(path) ?
                Path.Combine(path, name) :
                name;
        }

        #endregion
    }
}