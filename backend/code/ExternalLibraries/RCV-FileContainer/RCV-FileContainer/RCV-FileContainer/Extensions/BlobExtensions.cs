using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using RCV.FileContainer.Enumerations;
using RCV.FileContainer.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RCV.FileContainer.Extensions
{
    /// <summary>
    /// Static extension class used to provide extension methods for blob specific classes used in <see cref="RCV.FileContainer.Container.AzureBlobContainer"/>
    /// </summary>
    public static class BlobExtensions
    {
        private const char DIRECTORY_SEPARATOR_CHAR = '/';

        /// <summary>
        /// Builds the full qualified uri for a new blob
        /// </summary>
        /// <param name="cloudBlob">The blob reference to build the uri for</param>
        /// <param name="targetName">The name of the target directory</param>
        /// <param name="targetPath">The target path of the new blob</param>
        /// <returns>Full qualified uri for a new blob</returns>
        public static string GetTargetUri(this BlobClient cloudBlob, string targetName, string[] targetPath)
        {
            string basePath = $"https://{cloudBlob.Uri.Authority}/{cloudBlob.BlobContainerName}/{BlobUtilities.GetPath(targetName, targetPath)}/{cloudBlob.Uri.Segments.Last()}";

            return basePath;
        }

        /// <summary>
        /// Get the reference of a cloud block blob.
        /// </summary>
        /// <param name="blobContainerClient">The container to get the blob reference from</param>
        /// <param name="blobName">Name of the desired blob</param>
        /// <param name="directoryName">Name of the virtual directory</param>
        /// <param name="path">Path of the virtual directory / blob</param>
        /// <returns></returns>
        public static BlockBlobClient GetCloudBlockBlobReference(this BlobContainerClient blobContainerClient, string blobName, string directoryName = null, string[] path = null)
        {
            #region validation

            if (blobContainerClient == null)
            {
                throw new ArgumentNullException(nameof(blobContainerClient));
            }

            if (string.IsNullOrEmpty(blobName))
            {
                throw new ArgumentNullException(blobName);
            }

            #endregion

            string fullBlobPath;

            if (string.IsNullOrEmpty(directoryName))
            {
                fullBlobPath = BlobUtilities.GetPath(blobName, path);
            }
            else
            {
                string fullDirectoryPath = BlobUtilities.GetPath(directoryName, path);

                fullBlobPath = BlobUtilities.GetPath(blobName, fullDirectoryPath);
            }

            return blobContainerClient.GetBlockBlobClient(fullBlobPath);
        }

        /// <summary>
        /// Enumerates the given directory and filters based on specified searchPattern
        /// </summary>
        /// <param name="blobContainerClient">Directory to enumerate on</param>
        /// <param name="searchPattern">Search pattern to apply</param>
        /// <param name="prefix">Prefix to apply</param>
        /// <returns>Enumerable of strings which contains all found blobs</returns>
        public static IEnumerable<string> EnumerateDirectory(this BlobContainerClient blobContainerClient, string searchPattern, string prefix)
        {
            #region validation

            if (blobContainerClient == null)
            {
                throw new ArgumentNullException(nameof(blobContainerClient));
            }

            if (string.IsNullOrEmpty(searchPattern))
            {
                throw new ArgumentNullException(nameof(searchPattern));
            }

            #endregion

            var result = new List<string>();

            Regex blobNameRegex = RegexFactory.CreateRegexFromSearchPattern(searchPattern);

            foreach (var listBlobItem in blobContainerClient.GetBlobs(prefix: prefix))
            {
                string itemName = listBlobItem.Name;

                if (!string.IsNullOrEmpty(itemName) && blobNameRegex.IsMatch(itemName[(itemName.LastIndexOf(DIRECTORY_SEPARATOR_CHAR + prefix + DIRECTORY_SEPARATOR_CHAR) + prefix.Length + 2)..]))
                {
                    result.Add(blobContainerClient.Uri.AbsoluteUri + DIRECTORY_SEPARATOR_CHAR + itemName);
                }
            }

            return result;
        }

        /// <summary>
        /// Enumerates the given directory and filters based on specified searchPattern
        /// </summary>
        /// <param name="blobContainerClient">BlobContainerClient to enumerate on</param>
        /// <param name="blobType">Item type to look for</param>
        /// <param name="searchPattern">Search pattern to apply</param>
        /// <param name="prefix">Prefix to apply</param>
        /// <param name="recursive">Recursive</param>
        /// <returns>Enumerable of strings which contains all found blobs</returns>
        public static IEnumerable<string> EnumerateDirectory(this BlobContainerClient blobContainerClient, EBlobType blobType, string searchPattern, string prefix, bool recursive)
        {
            #region validation

            if (blobContainerClient == null)
            {
                throw new ArgumentNullException(nameof(blobContainerClient));
            }

            if (string.IsNullOrEmpty(searchPattern))
            {
                throw new ArgumentNullException(nameof(searchPattern));
            }

            #endregion

            var result = new List<string>();

            Regex blobNameRegex = RegexFactory.CreateRegexFromSearchPattern(searchPattern);

            foreach (var blob in blobContainerClient.GetBlobsByHierarchy(delimiter: DIRECTORY_SEPARATOR_CHAR.ToString(), prefix: prefix + DIRECTORY_SEPARATOR_CHAR))
            {
                switch (blobType)
                {
                    case EBlobType.Blobs:
                        if (blob.IsBlob)
                        {
                            if (blobNameRegex.IsMatch(blob.Blob.Name[(blob.Blob.Name.LastIndexOf(DIRECTORY_SEPARATOR_CHAR + prefix + DIRECTORY_SEPARATOR_CHAR) + prefix.Length + 2)..]))
                            {
                                result.Add(blobContainerClient.Uri.AbsoluteUri + DIRECTORY_SEPARATOR_CHAR + blob.Blob.Name);
                            }
                        }
                        else if (recursive)
                        {
                            result.AddRange(EnumerateDirectory(blobContainerClient, EBlobType.Blobs, "*", blob.Prefix[0..^1], recursive));
                        }

                        continue;
                    case EBlobType.Directories:
                        if (blob.IsPrefix)
                        {
                            string fullListItemPath = blob.Prefix;
                            if (fullListItemPath.EndsWith('/'))
                            {
                                fullListItemPath = fullListItemPath.Remove(fullListItemPath.LastIndexOf('/'));
                            }
                            if (blobNameRegex.IsMatch(fullListItemPath[(fullListItemPath.LastIndexOf(DIRECTORY_SEPARATOR_CHAR + prefix + DIRECTORY_SEPARATOR_CHAR) + prefix.Length + 2)..]))
                            {
                                result.Add(blobContainerClient.Uri.AbsoluteUri + DIRECTORY_SEPARATOR_CHAR + fullListItemPath);
                            }
                        }

                        continue;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(blobType), blobType, null);
                }
            }

            return result;
        }
    }
}