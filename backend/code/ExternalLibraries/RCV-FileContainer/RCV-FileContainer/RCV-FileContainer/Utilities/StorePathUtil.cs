using System.Collections.Generic;

namespace RCV.FileContainer.Utilities
{
    /// <summary>
    /// Path util for managing storage paths
    /// which are definied in configuration.
    /// </summary>
    public static class StorePathUtil
    {
        /// <summary>
        /// Get storage path representation as array.
        /// </summary>
        /// <param name="storagePath">Storage path string OR null</param>
        /// <returns>Representation of storage path as array</returns>
        public static string[] GetStoragePath(string storagePath)
        {
            if (!string.IsNullOrEmpty(storagePath))
            {
                return storagePath.Replace('\\', '/').Split('/', System.StringSplitOptions.RemoveEmptyEntries);
            }

            return null;
        }

        /// <summary>
        /// Joins path prefix and path togehter to a single array.
        /// </summary>
        /// <param name="pathPrefix">Prefix for path</param>
        /// <param name="path">Path</param>
        /// <returns>Joined path</returns>
        public static string[] JoinPaths(string[] pathPrefix, string[] path)
        {
            if (pathPrefix == null || pathPrefix.Length == 0)
            {
                return path;
            }

            List<string> joinedPath = new List<string>(pathPrefix);
            if (path != null)
            {
                joinedPath.AddRange(path);
            }
            return joinedPath.ToArray();
        }
    }
}
