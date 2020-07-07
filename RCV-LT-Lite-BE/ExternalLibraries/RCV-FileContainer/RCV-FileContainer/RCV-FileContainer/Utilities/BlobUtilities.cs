using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Blob;

namespace RCV.FileContainer.Utilities
{
    /// <summary>
    /// Utility class for azure blob storage.
    /// </summary>
    public static class BlobUtilities
    {
        /// <summary>
        /// Builds up a relative path based on specified segment parts.
        /// </summary>
        /// <param name="path">Path segments</param>
        /// <returns>Returns a relative path.</returns>
        public static string GetPath(string[] path)
        {
            return path == null
                ? string.Empty
                : $"{path.Aggregate(string.Empty, (current, subPath) => $"{current}{subPath}/")}";
        }

        /// <summary>
        /// Builds up a relative path based on specified segment parts.
        /// </summary>
        /// <param name="name">Name of blob or directory</param>
        /// <param name="path">Path segments</param>
        /// <returns>Returns a relative path.</returns>
        public static string GetPath(string name, string[] path = null)
        {
            #region validation

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            #endregion

            return path == null
                ? $"{name}"
                : $"{GetPath(path)}{name}";
        }

        
        /// <summary>
        /// Builds up a relative path based on specified segment parts.
        /// </summary>
        /// <param name="name">Name of blob or directory</param>
        /// <param name="path">Path as string</param>
        /// <returns>Returns a relative path.</returns>
        public static string GetPath(string name, string path = null)
        {
            #region validation

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            #endregion

            return !string.IsNullOrEmpty(path) ? $@"{path}/{name}" : name;
        }
    }
}