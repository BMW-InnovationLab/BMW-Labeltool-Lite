using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using RCV.FileContainer.Enumerations;
using RCV.FileContainer.Utilities;

namespace RCV.FileContainer.Extensions
{
    /// <summary>
    /// Static extension class used to provide extension methods for file specific classes used in <see cref="Container.SystemFileContainer"/> and <see cref="Container.SystemFileContainer"/>.
    /// </summary>
    public static class FileExtensions
    {        
        #region methods system files

        /// <summary>
        /// Copies folder from source to target recursively.
        /// </summary>
        /// <param name="source">Source directory info</param>
        /// <param name="target">Target direct info</param>
        public static void CopyAllTo(this DirectoryInfo source, DirectoryInfo target)
        {
            #region validation

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (source == target)
            {
                throw new InvalidOperationException("source can´t be equal target");
            }

            #endregion

            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                string destFileName = Path.Combine(target.FullName, fi.Name);

                fi.CopyTo(destFileName, true);
            }

            // Copy each sub directory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);

                diSourceSubDir.CopyAllTo(nextTargetSubDir);
            }
        }

        /// <summary>
        /// Deletes a directory and its contents recursively.
        /// </summary>
        /// <param name="directoryInfo">Directory info of directory which is supposed to be deleted.</param>
        public static void DeleteRecursive(this DirectoryInfo directoryInfo)
        {
            #region validation

            if (directoryInfo == null)
            {
                throw new ArgumentNullException(nameof(directoryInfo));
            }

            #endregion

            // delete all child folder
            foreach (DirectoryInfo subDirectoryInfo in directoryInfo.EnumerateDirectories())
            {
                subDirectoryInfo.Delete(true);
            }

            // delete all child files
            foreach (FileInfo subFileInfo in directoryInfo.EnumerateFiles())
            {
                subFileInfo.Delete();
            }
        }

        #endregion

        #region methods azure files

        /// <summary>
        /// Appends a given string to the specified array.
        /// </summary>
        /// <param name="array">Array to append to</param>
        /// <param name="value">Value to append to array</param>
        /// <returns>A new array with the appended string value.</returns>
        public static string[] Append(this string[] array, string value)
        {
            #region validation

            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            #endregion

            Array.Resize(ref array, array.Length + 1);

            array[array.Length - 1] = value;

            return array;
        }

        /// <summary>
        /// Recursive delete of folder and all included subfolders or files.
        /// </summary>
        /// <param name="cloudFileDirectory">Directory to delete</param>
        public static void DeleteRecursive(this CloudFileDirectory cloudFileDirectory)
        {
            #region validation

            if (cloudFileDirectory == null)
            {
                throw new ArgumentNullException(nameof(cloudFileDirectory));
            }

            #endregion

            if (!TaskUtilities.ExecuteSync(cloudFileDirectory.ExistsAsync()))
            {
                return;
            }

            var directoriesToBeDeleted = new List<CloudFileDirectory>();

            var continuationToken = new FileContinuationToken();

            // get first segment
            FileResultSegment fileResultSegment;

            do
            {
                fileResultSegment = TaskUtilities.ExecuteSync(cloudFileDirectory.ListFilesAndDirectoriesSegmentedAsync(continuationToken));

                // iterate through items
                foreach (IListFileItem fileListItem in fileResultSegment.Results)
                {
                    switch (fileListItem)
                    {
                        case CloudFile file:

                            // Delete file directly
                            TaskUtilities.ExecuteSync(file.DeleteAsync());

                            break;

                        case CloudFileDirectory directory:

                            // Add the current directory
                            directoriesToBeDeleted.Add(directory);

                            // List all sub directories recursively
                            directoriesToBeDeleted.AddRange(directory.ListSubDirectories());

                            break;
                    }
                }

            } while (fileResultSegment.ContinuationToken != null);

            // Sort directories bottom to top so the most deepest nested directories will be deleted first
            directoriesToBeDeleted.Sort((aDirectory, bDirectory) => bDirectory.Uri.LocalPath.Length.CompareTo(aDirectory.Uri.LocalPath.Length));

            // Delete all found directories
            foreach (CloudFileDirectory cloudFileDirectoryToBeDeleted in directoriesToBeDeleted)
            {
                TaskUtilities.ExecuteSync(cloudFileDirectoryToBeDeleted.DeleteAsync());
            }

            // Delete the parent directory itself in case it´s not the root directory (which is the file share)
            if (cloudFileDirectory.SnapshotQualifiedUri != cloudFileDirectory.Share.GetRootDirectoryReference().SnapshotQualifiedUri)
            {
                TaskUtilities.ExecuteSync(cloudFileDirectory.DeleteAsync());
            }
        }

        private static IEnumerable<CloudFileDirectory> ListSubDirectories(this CloudFileDirectory cloudFileDirectory)
        {
            #region validation

            if (cloudFileDirectory == null)
            {
                throw new ArgumentNullException(nameof(cloudFileDirectory));
            }

            #endregion

            var subDirectories = new List<CloudFileDirectory>();

            FileResultSegment fileResultSegment;

            do
            {
                fileResultSegment = TaskUtilities.ExecuteSync(cloudFileDirectory.ListFilesAndDirectoriesSegmentedAsync(new FileContinuationToken()));

                foreach (IListFileItem listFileItem in fileResultSegment.Results)
                {
                    switch (listFileItem)
                    {
                        case CloudFile cloudFile:

                            TaskUtilities.ExecuteSync(cloudFile.DeleteAsync());

                            break;

                        case CloudFileDirectory subDirectory:

                            subDirectories.Add(subDirectory);

                            subDirectories.AddRange(subDirectory.ListSubDirectories());

                            break;
                    }
                }

            } while (fileResultSegment.ContinuationToken != null);

            return subDirectories;
        }

        /// <summary>
        /// Enumerates a specified cloud file directory and returns items based item type and searchPattern.
        /// </summary>
        /// <param name="cloudFileDirectory">Cloud file directory to enumerate on</param>
        /// <param name="fileType">Item type to filter with</param>
        /// <param name="searchPattern">Search pattern to filter with</param>
        /// <returns>Enumerable of strings which contains all found names based on file type</returns>
        public static IEnumerable<string> EnumerateDirectory(this CloudFileDirectory cloudFileDirectory, EFileType fileType, string searchPattern)
        {
            #region validation

            if (cloudFileDirectory == null)
            {
                throw new ArgumentNullException(nameof(cloudFileDirectory));
            }

            if (string.IsNullOrEmpty(searchPattern))
            {
                throw new ArgumentNullException(nameof(searchPattern));
            }

            if (!TaskUtilities.ExecuteSync(cloudFileDirectory.ExistsAsync()))
            {
               return new List<string>();
            }

            #endregion

            var directoryNames = new List<string>();

            var fileContinuationToken = new FileContinuationToken();

            do
            {
                FileResultSegment fileResultSegment = TaskUtilities.ExecuteSync(cloudFileDirectory.ListFilesAndDirectoriesSegmentedAsync(fileContinuationToken));

                foreach (IListFileItem listFileItem in fileResultSegment.Results)
                {
                    string fullListItemPath = string.Empty;

                    switch (fileType)
                    {
                        case EFileType.Files:
                            if (listFileItem is CloudFile cloudFile)
                            {
                                fullListItemPath = cloudFile.StorageUri.PrimaryUri.ToString();
                            }

                            break;
                        case EFileType.Directories:
                            if (listFileItem is CloudFileDirectory pendingCloudFileDirectory)
                            {
                                fullListItemPath = pendingCloudFileDirectory.StorageUri.PrimaryUri.ToString();

                                if (fullListItemPath.EndsWith('/'))
                                {
                                    fullListItemPath = fullListItemPath.Remove(fullListItemPath.LastIndexOf('/'));
                                }
                            }

                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null);
                    }

                    // replace wildcard for 'multiple letters' with regex-wildcard
                    string regexableSearchPattern = searchPattern.Replace("*", ".*");

                    // replace wildcard for 'single letter' with regex-wildcard
                    regexableSearchPattern = regexableSearchPattern.Replace("?", ".?");

                    // set search pattern in 'begin-to-end'-symbols
                    regexableSearchPattern = $"^{regexableSearchPattern}$";

                    var rgxFileName = new Regex(regexableSearchPattern);

                    if (!string.IsNullOrEmpty(fullListItemPath) && rgxFileName.IsMatch(fullListItemPath))
                    {
                        directoryNames.Add(fullListItemPath);
                    }
                }

                fileContinuationToken = fileResultSegment.ContinuationToken;

            } while (fileContinuationToken != null);

            return directoryNames;
        }
        
        /// <summary>
        /// Gets a <see cref="CloudFileDirectory"/> reference of a directory inside the specified container based on given directory name and path.
        /// </summary>
        /// <param name="cloudFileShare">Cloud File Share to get the file share reference from</param>
        /// <param name="directoryName">Directory name of the desired directory reference</param>
        /// <param name="path">Directory path of the desired directory reference</param>
        /// <param name="createIfNotExists">Flag to control the creation of the directory in case it does not exist. TRUE for creation, FALSE for cancel</param>
        /// <returns>A cloud file directory reference at the specified location</returns>
        public static CloudFileDirectory GetDirectoryReference(this CloudFileShare cloudFileShare, string directoryName = null, string[] path = null, bool createIfNotExists = true)
        {
            #region validation

            if (cloudFileShare == null)
            {
                throw new ArgumentNullException(nameof(cloudFileShare));
            }

            #endregion

            bool isDirectoryNameNullOrEmpty = string.IsNullOrEmpty(directoryName);

            if (path == null)
            {
                return isDirectoryNameNullOrEmpty
                    ? cloudFileShare.GetRootDirectoryReference()
                    : cloudFileShare.GetRootDirectoryReference().GetDirectoryReference(directoryName);
            }

            if (!isDirectoryNameNullOrEmpty)
            {
                path = path.Append(directoryName);
            }

            CloudFileDirectory currentDir = cloudFileShare.GetRootDirectoryReference();

            foreach (string folder in path)
            {
                currentDir = currentDir.GetDirectoryReference(folder);

                if (createIfNotExists)
                {
                    TaskUtilities.ExecuteSync(currentDir.CreateIfNotExistsAsync());
                }
            }

            return currentDir;
        }

        /// <summary>
        /// Gets a <see cref="CloudFileShare"/> reference of a directory inside the specified container based on given directory name and path.
        /// </summary>
        /// <param name="cloudStorageAccount">Instance of the cloud storage account to get the share reference from</param>
        /// <param name="shareName">Name of the cloud file share</param>
        /// <returns>Cloud file share reference</returns>
        public static CloudFileShare GetShareReference(this CloudStorageAccount cloudStorageAccount, string shareName)
        {
            #region validation

            if (cloudStorageAccount == null)
            {
                throw new ArgumentNullException(nameof(cloudStorageAccount));
            }

            #endregion

            CloudFileClient fileClient = cloudStorageAccount.CreateCloudFileClient();

            CloudFileShare share = fileClient.GetShareReference(shareName);

            return share;
        }
        #endregion
    }
}