using RCV.FileContainer.Contracts;
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

        private IPathUtil PathUtil { get; }

        #endregion

        #region constructor

        /// <summary>
        /// Constructor for the file based utility class.
        /// </summary>
        /// <param name="rootPath">Root path of the directory which the class will be wrapped around.</param>
        /// <param name="pathUtil">Util for determine path information from underlying file system</param>
        public FileUtilities(string rootPath, IPathUtil pathUtil = null)
        {
            #region validation

            if (string.IsNullOrEmpty(rootPath))
            {
                throw new ArgumentNullException(nameof(rootPath));
            }

            #endregion

            RootPath = rootPath;

            // set default path util (determine from unterlying file system)
            PathUtil = pathUtil ?? new PathUtil();
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
            string[] childPath = path ?? new string[] { };

            // create result path
            string resultPath = PathUtil.Combine(childPath);
            resultPath = resultPath.Trim('/', '\\');

            // prepend root path with corrected separators
            // remove all leading and trailing separator chars
            string cleansedRoot = RootPath.Trim('/', '\\');

            // replace all slashes with directory separator
            cleansedRoot = cleansedRoot.Replace('/', PathUtil.DirectorySeparatorChar);
            // replace all backslashes with directory separator
            cleansedRoot = cleansedRoot.Replace('\\', PathUtil.DirectorySeparatorChar);

            if (!resultPath.StartsWith(cleansedRoot))
            {
                resultPath = Path.Join(cleansedRoot, resultPath);
            }

            // if root path is relative, add relative separator
            if (RootPath.StartsWith(PathUtil.DirectorySeparatorChar) || RootPath.StartsWith(PathUtil.AltDirectorySeparatorChar))
            {
                resultPath = PathUtil.DirectorySeparatorChar + resultPath;
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