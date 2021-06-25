using Azure.Storage.Files.Shares;
using RCV.FileContainer.Enumerations;
using RCV.FileContainer.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

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

            #endregion

            if (source == target)
            {
                return;
            }

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

            array[^1] = value;

            return array;
        }

        /// <summary>
        /// Recursive delete of folder and all included subfolders or files.
        /// </summary>
        /// <param name="cloudFileDirectory">Directory to delete</param>
        public static void DeleteRecursive(this ShareDirectoryClient cloudFileDirectory)
        {
            #region validation

            if (cloudFileDirectory == null)
            {
                throw new ArgumentNullException(nameof(cloudFileDirectory));
            }

            #endregion

            cloudFileDirectory.DeleteRecursive();
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

        private static IEnumerable<ShareDirectoryClient> ListSubDirectories(this ShareDirectoryClient cloudFileDirectory)
        {
            #region validation

            if (cloudFileDirectory == null)
            {
                throw new ArgumentNullException(nameof(cloudFileDirectory));
            }

            #endregion

            return cloudFileDirectory.ListSubDirectories();
        }

        /// <summary>
        /// Enumerates a specified cloud file directory and returns items based item type and searchPattern.
        /// </summary>
        /// <param name="cloudFileDirectory">Cloud file directory to enumerate on</param>
        /// <param name="fileType">Item type to filter with</param>
        /// <param name="searchPattern">Search pattern to filter with</param>
        /// <returns>Enumerable of strings which contains all found names based on file type</returns>
        public static IEnumerable<string> EnumerateDirectory(this ShareDirectoryClient cloudFileDirectory, EFileType fileType, string searchPattern)
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

            if (!cloudFileDirectory.Exists())
            {
                return new List<string>();
            }

            #endregion

            var directoryNames = new List<string>();

            var cancellationToken = new CancellationToken();

            do
            {
                var filesAndDirectories = cloudFileDirectory.GetFilesAndDirectories(cancellationToken: cancellationToken);

                Regex fileNameRegex = RegexFactory.CreateRegexFromSearchPattern(searchPattern);

                foreach (var shareFileItem in filesAndDirectories)
                {
                    string fullListItemPath = string.Empty;
                    string listFileItemName = string.Empty;

                    switch (fileType)
                    {
                        case EFileType.Files:
                            if (!shareFileItem.IsDirectory)
                            {
                                listFileItemName = shareFileItem.Name;
                                fullListItemPath = shareFileItem.Name;
                            }

                            break;
                        case EFileType.Directories:
                            if (shareFileItem.IsDirectory)
                            {
                                listFileItemName = shareFileItem.Name;
                                fullListItemPath = shareFileItem.Name;

                                if (fullListItemPath.EndsWith('/'))
                                {
                                    fullListItemPath = fullListItemPath.Remove(fullListItemPath.LastIndexOf('/'));
                                }
                            }

                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null);
                    }

                    if (!string.IsNullOrEmpty(listFileItemName) && fileNameRegex.IsMatch(listFileItemName))
                    {
                        directoryNames.Add(fullListItemPath);
                    }
                }
            } while (!cancellationToken.IsCancellationRequested);

            return directoryNames;
        }

        /// <summary>
        /// Gets a <see cref="ShareDirectoryClient"/> reference of a directory inside the specified container based on given directory name and path.
        /// </summary>
        /// <param name="cloudFileShare">Cloud File Share to get the file share reference from</param>
        /// <param name="directoryName">Directory name of the desired directory reference</param>
        /// <param name="path">Directory path of the desired directory reference</param>
        /// <param name="createIfNotExists">Flag to control the creation of the directory in case it does not exist. TRUE for creation, FALSE for cancel</param>
        /// <returns>A cloud file directory reference at the specified location</returns>
        public static ShareDirectoryClient GetDirectoryReference(this ShareClient cloudFileShare, string directoryName = null, string[] path = null, bool createIfNotExists = true)
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
                    ? cloudFileShare.GetRootDirectoryClient()
                    : cloudFileShare.GetRootDirectoryClient().GetSubdirectoryClient(directoryName);
            }

            if (!isDirectoryNameNullOrEmpty)
            {
                path = path.Append(directoryName);
            }

            ShareDirectoryClient currentDir = cloudFileShare.GetRootDirectoryClient();

            foreach (string folder in path)
            {
                currentDir = currentDir.GetSubdirectoryClient(folder);

                if (createIfNotExists)
                {
                    currentDir.CreateIfNotExists();
                }
            }

            return currentDir;
        }

        /// <summary>
        /// Gets a <see cref="ShareDirectoryClient"/> reference of a directory inside the specified container based on given directory name and path.
        /// </summary>
        /// <param name="cloudStorageAccount">Instance of the cloud storage account to get the share reference from</param>
        /// <param name="shareName">Name of the cloud file share</param>
        /// <returns>Cloud file share reference</returns>
        public static ShareDirectoryClient GetShareReference(this ShareClient cloudStorageAccount, string shareName)
        {
            #region validation

            if (cloudStorageAccount == null)
            {
                throw new ArgumentNullException(nameof(cloudStorageAccount));
            }

            #endregion

            return cloudStorageAccount.GetDirectoryReference(shareName);
        }
        #endregion
    }
}