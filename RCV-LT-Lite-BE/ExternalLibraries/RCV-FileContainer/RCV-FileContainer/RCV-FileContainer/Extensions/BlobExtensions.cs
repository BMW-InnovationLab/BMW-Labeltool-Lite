using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.WindowsAzure.Storage.Blob;
using RCV.FileContainer.Enumerations;
using RCV.FileContainer.Utilities;

namespace RCV.FileContainer.Extensions
{
    /// <summary>
    /// Static extension class used to provide extension methods for blob specific classes used in <see cref="RCV.FileContainer.Container.AzureBlobContainer"/>
    /// </summary>
    public static class BlobExtensions
    {
        /// <summary>
        /// Builds the full qualified uri for a new blob
        /// </summary>
        /// <param name="cloudBlob">The blob reference to build the uri for</param>
        /// <param name="targetName">The name of the target directory</param>
        /// <param name="targetPath">The target path of the new blob</param>
        /// <returns>Full qualified uri for a new blob</returns>
        public static string GetTargetUri(this CloudBlob cloudBlob, string targetName, string[] targetPath)
        {
            string basePath = $"https://{cloudBlob.Uri.Authority}/{cloudBlob.Container.Name}/{BlobUtilities.GetPath(targetName, targetPath)}/{cloudBlob.Uri.Segments.Last()}";

            return basePath;
        }
        
        /// <summary>
        /// Get the reference of a cloud block blob.
        /// </summary>
        /// <param name="cloudBlobContainer">The container to get the blob reference from</param>
        /// <param name="blobName">Name of the desired blob</param>
        /// <param name="directoryName">Name of the virtual directory</param>
        /// <param name="path">Path of the virtual directory / blob</param>
        /// <returns></returns>
        public static CloudBlockBlob GetCloudBlockBlobReference(this CloudBlobContainer cloudBlobContainer, string blobName, string directoryName = null, string[] path = null)
        {
            #region validation

            if (cloudBlobContainer == null)
            {
                throw new ArgumentNullException(nameof(cloudBlobContainer));
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

            return cloudBlobContainer.GetBlockBlobReference(fullBlobPath);
        }

        /// <summary>
        /// Returns a cloud blob directory reference at a specified location
        /// </summary>
        /// <param name="cloudBlobContainer">Container reference to get the directory reference from</param>
        /// <param name="directoryName">Name of the cloud blob directory</param>
        /// <param name="path">Path to the cloud blob directory</param>
        /// <returns>Cloud blob directory reference at the desired location</returns>
        public static CloudBlobDirectory GetCloudBlobDirectoryReference(this CloudBlobContainer cloudBlobContainer, string directoryName, string[] path = null)
        {
            #region validation

            if (cloudBlobContainer == null)
            {
                throw new ArgumentNullException(nameof(cloudBlobContainer));
            }

            if (string.IsNullOrEmpty(directoryName))
            {
                throw new ArgumentNullException(directoryName);
            }
            
            #endregion

            string fullPath = BlobUtilities.GetPath(directoryName, path);

            return cloudBlobContainer.GetDirectoryReference(fullPath);
        }

        /// <summary>
        /// Returns a cloud blob directory reference at a specified location
        /// </summary>
        /// <param name="cloudBlobContainer">Container reference to get the directory reference from</param>
        /// <param name="path">Path to the cloud blob directory</param>
        /// <returns>Cloud blob directory reference at the desired location</returns>
        public static CloudBlobDirectory GetCloudBlobDirectoryReference(this CloudBlobContainer cloudBlobContainer, string[] path = null)
        {
            #region validation

            if (cloudBlobContainer == null)
            {
                throw new ArgumentNullException(nameof(cloudBlobContainer));
            }            
            #endregion

            string fullPath = BlobUtilities.GetPath(path);

            return cloudBlobContainer.GetDirectoryReference(fullPath);
        }

        /// <summary>
        /// Enumerates the given directory and filters based on specified searchPattern
        /// </summary>
        /// <param name="cloudBlobDirectory">Directory to enumerate on</param>
        /// <param name="searchPattern">Search pattern to apply</param>
        /// <returns>Enumerable of strings which contains all found blobs</returns>
        public static IEnumerable<string> EnumerateDirectory(this CloudBlobDirectory cloudBlobDirectory, string searchPattern)
        {
            #region validation

            if (cloudBlobDirectory == null)
            {
                throw new ArgumentNullException(nameof(cloudBlobDirectory));
            }

            if (string.IsNullOrEmpty(searchPattern))
            {
                throw new ArgumentNullException(nameof(searchPattern));
            }

            #endregion

            var result = new List<string>();

            var blobContinuationToken = new BlobContinuationToken();
            
            do
            {
                BlobResultSegment blobResultSegment = TaskUtilities.ExecuteSync(cloudBlobDirectory.ListBlobsSegmentedAsync(blobContinuationToken));

                foreach (IListBlobItem listBlobItem in blobResultSegment.Results)
                {
                    // replace wildcard for 'multiple letters' with regex-wildcard
                    string regexeableSearchPattern = searchPattern.Replace("*", ".*");

                    // replace wildcard for 'single letter' with regex-wildcard
                    regexeableSearchPattern = regexeableSearchPattern.Replace("?", ".?");

                    // set search pattern in 'begin-to-end'-symbols
                    regexeableSearchPattern = $"^{regexeableSearchPattern}$";

                    var rgxBlobName = new Regex(regexeableSearchPattern);

                    string itemName = listBlobItem.Uri.AbsolutePath;

                    if (!string.IsNullOrEmpty(itemName) && rgxBlobName.IsMatch(itemName))
                    {
                        result.Add(listBlobItem.StorageUri.PrimaryUri.ToString());
                    }
                }

                blobContinuationToken = blobResultSegment.ContinuationToken;

            } while (blobContinuationToken != null);

            return result;
        }

        /// <summary>
        /// Enumerates the given directory and filters based on specified searchPattern
        /// </summary>
        /// <param name="cloudBlobDirectory">Directory to enumerate on</param>
        /// <param name="blobType">Item type to look for</param>
        /// <param name="searchPattern">Search pattern to apply</param>
        /// <returns>Enumerable of strings which contains all found blobs</returns>
        public static IEnumerable<string> EnumerateDirectory(this CloudBlobDirectory cloudBlobDirectory, EBlobType blobType, string searchPattern)
        {
            #region validation

            if (cloudBlobDirectory == null)
            {
                throw new ArgumentNullException(nameof(cloudBlobDirectory));
            }

            if (string.IsNullOrEmpty(searchPattern))
            {
                throw new ArgumentNullException(nameof(searchPattern));
            }

            #endregion

            var result = new List<string>();

            var blobContinuationToken = new BlobContinuationToken();
            
            do
            {
                BlobResultSegment blobResultSegment = TaskUtilities.ExecuteSync(cloudBlobDirectory.ListBlobsSegmentedAsync(blobContinuationToken));

                foreach (IListBlobItem listBlobItem in blobResultSegment.Results)
                {
                    // replace wildcard for 'multiple letters' with regex-wildcard
                    string regexeableSearchPattern = searchPattern.Replace("*", ".*");

                    // replace wildcard for 'single letter' with regex-wildcard
                    regexeableSearchPattern = regexeableSearchPattern.Replace("?", ".?");

                    // set search pattern in 'begin-to-end'-symbols
                    regexeableSearchPattern = $"^{regexeableSearchPattern}$";

                    var rgxBlobName = new Regex(regexeableSearchPattern);

                    string itemName = listBlobItem.Uri.AbsolutePath;

                    switch (blobType)
                    {
                        case EBlobType.Blobs:
                            if (listBlobItem is CloudBlob blob)
                            {
                                if (!string.IsNullOrEmpty(itemName) && rgxBlobName.IsMatch(itemName))
                                {
                                    result.Add(blob.StorageUri.PrimaryUri.ToString());
                                }
                            }

                            break;
                        case EBlobType.Directories:
                            if (listBlobItem is CloudBlobDirectory subCloudBlobDirectory)
                            {
                                if (!string.IsNullOrEmpty(itemName) && rgxBlobName.IsMatch(itemName))
                                {
                                    string fullListItemPath = subCloudBlobDirectory.StorageUri.PrimaryUri.ToString();

                                    if (fullListItemPath.EndsWith('/'))
                                    {
                                        fullListItemPath = fullListItemPath.Remove(fullListItemPath.LastIndexOf('/'));
                                    }

                                    result.Add(fullListItemPath);
                                }
                            }

                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(blobType), blobType, null);
                    }
                }

                blobContinuationToken = blobResultSegment.ContinuationToken;

            } while (blobContinuationToken != null);

            return result;
        }

        /// <summary>
        /// Deletes recursively based on given directory.
        /// </summary>
        /// <param name="cloudBlobDirectory">Instance of the blob directory to delete from</param>
        public static void DeleteRecursive(this CloudBlobDirectory cloudBlobDirectory)
        {
            #region validation

            if (cloudBlobDirectory == null)
            {
                throw new ArgumentNullException(nameof(cloudBlobDirectory));
            }

            #endregion

            var blobContinuationToken = new BlobContinuationToken();

            do
            {
                // Try to get all existing blobs at once
                BlobResultSegment blobResultSegment = TaskUtilities.ExecuteSync(
                    cloudBlobDirectory.ListBlobsSegmentedAsync(blobContinuationToken)
                );

                // Delete each blob
                foreach (IListBlobItem listBlobItem in blobResultSegment.Results)
                {
                    switch (listBlobItem)
                    {
                        case CloudBlob cloudBlob:
                            TaskUtilities.ExecuteSync(cloudBlob.DeleteAsync());
                            break;

                        case CloudBlobDirectory subCloudBlobDirectory:
                            subCloudBlobDirectory.DeleteRecursive();
                            break;
                    }
                }

                blobContinuationToken = blobResultSegment.ContinuationToken;

            } while (blobContinuationToken != null);
        }
    }
}