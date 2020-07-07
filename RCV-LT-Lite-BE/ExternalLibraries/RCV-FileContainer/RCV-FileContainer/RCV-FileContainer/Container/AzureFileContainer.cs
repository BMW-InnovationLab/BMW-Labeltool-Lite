using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using RCV.FileContainer.Contracts;
using RCV.FileContainer.Enumerations;
using RCV.FileContainer.Extensions;
using RCV.FileContainer.Utilities;

namespace RCV.FileContainer.Container
{
    /// <inheritdoc />
    /// <summary>
    /// Container definition for access of an azure file share.
    /// </summary>
    public class AzureFileContainer : IFileContainer
    {
        #region member

        private CloudStorageAccount CloudStorageAccount { get; }

        private CloudFileShare CloudFileShare { get; }

        private const string DIRECTORY_SEPARATOR_CHAR = "/";

        #endregion

        #region constructor

        /// <summary>
        /// Creates a file share utility wrapper for azure infrastructure.
        /// </summary>
        /// <param name="cloudStorageConnectionString">The connection string for azure (valid format needed), e.g. DefaultEndpointsProtocol=https;AccountName=bmwrcvdev;AccountKey=APUfaYGYAOk7owSH2P2Uy/2BWDYxFLJsKgGcG0DyhdYdWS6TRfdEYmKvlk7VewwdaorfetwB2MSfLckQm7a9YA==;EndpointSuffix=core.windows.net</param>
        /// <param name="shareName">The name of configured azure file share.</param>
        public AzureFileContainer(string cloudStorageConnectionString, string shareName)
        {
            #region validation

            if (string.IsNullOrEmpty(cloudStorageConnectionString))
            {
                throw new ArgumentNullException(nameof(cloudStorageConnectionString));
            }

            if (string.IsNullOrEmpty(shareName))
            {
                throw new ArgumentNullException(nameof(shareName));
            }

            #endregion

            CloudStorageAccount = CloudStorageAccount.Parse(cloudStorageConnectionString);

            CloudFileShare = CloudStorageAccount.GetShareReference(shareName);
        }

        #endregion

        #region methods

        /// <inheritdoc />
        /// Copies only first tier sub directory and files.
        public void CopyDirectory(string directoryName, string[] sourcePath, string[] targetPath)
        {
            #region validation

            if (string.IsNullOrEmpty(directoryName))
            {
                throw new ArgumentNullException(nameof(directoryName));
            }

            if (sourcePath == targetPath)
            {
                throw new InvalidOperationException("source can´t be equal target");
            }

            #endregion

            string[] source = sourcePath == null ?
                new[] { directoryName } :
                sourcePath.Append(directoryName);

            // get all files from source directory
            IEnumerable<string> sourceDirectoryFileNames = GetFiles(source)
                .Select(fullFilePath => fullFilePath.Substring(fullFilePath.LastIndexOf(DIRECTORY_SEPARATOR_CHAR, StringComparison.Ordinal)).Remove(0, 1));

            // create target directory
            CreateDirectory(directoryName, targetPath);

            foreach (string sourceDirectoryFileName in sourceDirectoryFileNames)
            {
                using (Stream sourceStream = GetFileStream(sourceDirectoryFileName, sourcePath))
                {
                    string[] target = targetPath == null ? 
                        new []{directoryName} : 
                        targetPath.Append(directoryName);

                    SetFileStream(sourceDirectoryFileName, sourceStream, target);
                }
            }

            CloudFileDirectory sourceCloudFileDirectory = CloudFileShare.GetDirectoryReference(directoryName, sourcePath);

            foreach (IListFileItem listFileItem in TaskUtilities.ExecuteSync(sourceCloudFileDirectory.ListFilesAndDirectoriesSegmentedAsync(new FileContinuationToken())).Results)
            {
                if (!(listFileItem is CloudFileDirectory cloudFileDirectory))
                {
                    continue;
                }

                // Get target directory
                CloudFileDirectory targetCloudFileDirectory = CloudFileShare.GetDirectoryReference(directoryName, targetPath);

                // Get new sub directory to be created
                CloudFileDirectory subCloudFileDirectory = targetCloudFileDirectory.GetDirectoryReference(cloudFileDirectory.Name);

                // Create new sub directory
                TaskUtilities.ExecuteSync(subCloudFileDirectory.CreateIfNotExistsAsync());
            }
        }

        /// <inheritdoc />
        public void CopyFile(string sourceFileName, string[] sourceParentDirectories, string targetFileName, string[] targetParentDirectories, bool overwriteIfExists = false)
        {
            #region validation

            if (string.IsNullOrEmpty(sourceFileName))
            {
                throw new ArgumentNullException(nameof(sourceFileName));
            }

            if (string.IsNullOrEmpty(targetFileName))
            {
                throw new ArgumentNullException(nameof(targetFileName));
            }

            #endregion

            CloudFileDirectory sourceDirectory = CloudFileShare.GetDirectoryReference(path: sourceParentDirectories);
                
            CloudFileDirectory targetDirectory = CloudFileShare.GetDirectoryReference(path: targetParentDirectories);

            CloudFile sourceFile = sourceDirectory.GetFileReference(sourceFileName);

            CloudFile targetFile = targetDirectory.GetFileReference(targetFileName);

            if (overwriteIfExists)
            {
                TaskUtilities.ExecuteSync(targetFile.DeleteIfExistsAsync());
            }

            if (!TaskUtilities.ExecuteSync(targetFile.ExistsAsync()))
            {
                TaskUtilities.ExecuteSync(targetFile.StartCopyAsync(sourceFile));
            }
        }
        
        /// <inheritdoc />
        public string[] CreateDirectory(string directoryName, string[] path = null)
        {
            #region validation

            if (string.IsNullOrEmpty(directoryName))
            {
                throw new ArgumentNullException(nameof(directoryName));
            }

            #endregion

            if (path != null)
            {
                path = path.Append(directoryName);
            }
            else
            {
                path = new[]
                {
                    directoryName
                };
            }

            CloudFileDirectory cloudFileDirectory = CloudFileShare.GetDirectoryReference(path: path);

            string qualifiedUri = GetQualifiedUri(cloudFileDirectory);

            return qualifiedUri.Split(DIRECTORY_SEPARATOR_CHAR, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <inheritdoc />
        public string[] CreateFile(string fileName, byte[] fileContent, string[] path = null)
        {
            #region validation

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (fileContent == null)
            {
                throw new ArgumentNullException(nameof(fileContent));
            }

            #endregion

            CloudFileDirectory currentDir = CloudFileShare.GetDirectoryReference(path: path);

            CloudFile newFile = currentDir.GetFileReference(fileName);

            TaskUtilities.ExecuteSync(newFile.UploadFromByteArrayAsync(fileContent, 0, fileContent.Length));

            string qualifiedUri = GetQualifiedUri(newFile);

            return qualifiedUri.Split(DIRECTORY_SEPARATOR_CHAR);
        }

        /// <inheritdoc />
        public string[] CreateFile(string fileName, Stream fileContent, string[] path = null)
        {
            #region validation

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (fileContent == null)
            {
                throw new ArgumentNullException(nameof(fileContent));
            }

            #endregion
 
            CloudFileDirectory currentDir = CloudFileShare.GetDirectoryReference(path: path);

            CloudFile newFile = currentDir.GetFileReference(fileName);

            TaskUtilities.ExecuteSync(newFile.UploadFromStreamAsync(fileContent));

            string qualifiedUri = GetQualifiedUri(newFile);

            return qualifiedUri.Split(DIRECTORY_SEPARATOR_CHAR);
        }

        /// <inheritdoc />
        public void DeleteDirectory(string directoryName, string[] path)
        {
            #region validation

            if (string.IsNullOrEmpty(directoryName))
            {
                throw new ArgumentNullException(nameof(directoryName));
            }

            #endregion

            CloudFileDirectory currentDir = CloudFileShare.GetDirectoryReference(directoryName, path);

            currentDir.DeleteRecursive();
        }

        /// <inheritdoc />
        public void DeleteFile(string fileName, string[] path)
        {
            #region validation

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            #endregion

            CloudFileDirectory currentDir = CloudFileShare.GetDirectoryReference(null, path);

            CloudFile file = currentDir.GetFileReference(fileName);

            TaskUtilities.ExecuteSync(file.DeleteIfExistsAsync());
        }

        /// <inheritdoc />
        public bool ExistsDirectory(string directoryName, string[] path)
        {
            #region validation

            if (string.IsNullOrEmpty(directoryName))
            {
                throw new ArgumentNullException(nameof(directoryName));
            }

            #endregion

            CloudFileDirectory cloudFileDirectory = CloudFileShare.GetDirectoryReference(directoryName, path);

            return TaskUtilities.ExecuteSync(cloudFileDirectory.ExistsAsync());
        }

        /// <inheritdoc />
        public bool ExistsFile(string fileName, string[] path)
        {
            #region validation

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            #endregion

            CloudFileDirectory currentDir = CloudFileShare.GetDirectoryReference(path: path);

            if (!TaskUtilities.ExecuteSync(currentDir.ExistsAsync()))
            {
                return false;

            }

            CloudFile file = currentDir.GetFileReference(fileName);

            return file != null && TaskUtilities.ExecuteSync(file.ExistsAsync());
        }

        /// <inheritdoc />
        public bool ExistsPath(string[] path)
        {
            #region validation

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            #endregion

            bool isExistingFile = ExistsFile(path.Last(), path.Take(path.Length > 0 ? path.Length - 1 : 0).ToArray());
            bool isExistingDirectory = ExistsDirectory(path.Last(), path.Take(path.Length > 0 ? path.Length - 1 : 0).ToArray());

            return isExistingFile || isExistingDirectory;
        }
                
        /// <inheritdoc />
        public IEnumerable<string> GetDirectories(string[] path)
        {
            CloudFileDirectory cloudFileDirectory = CloudFileShare.GetDirectoryReference(path: path);

            return cloudFileDirectory.EnumerateDirectory(EFileType.Directories, "*");
        }

        /// <inheritdoc />
        public IEnumerable<string> GetDirectoryNames(string searchPattern, string[] path)
        {
            #region validation

            if (string.IsNullOrEmpty(searchPattern))
            {
                throw new ArgumentNullException(nameof(searchPattern));
            }

            #endregion

            CloudFileDirectory cloudFileDirectory = CloudFileShare.GetDirectoryReference(path: path);

            IEnumerable<string> fullDirectoryPaths = cloudFileDirectory.EnumerateDirectory(EFileType.Directories, searchPattern);

            return fullDirectoryPaths.Select(fullDirectoryPath => fullDirectoryPath.Split(DIRECTORY_SEPARATOR_CHAR).Last());
        }

        /// <inheritdoc />
        public string GetFileContent(string fileName, string[] path = null)
        {
            #region validation

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            #endregion

            CloudFileDirectory cloudFileDirectory = CloudFileShare.GetDirectoryReference(path: path);

            CloudFile cloudFile = cloudFileDirectory.GetFileReference(fileName);

            return TaskUtilities.ExecuteSync(cloudFile.DownloadTextAsync());
        }

        /// <inheritdoc />
        public IEnumerable<string> GetFileNames(string searchPattern, string[] path = null)
        {
            #region validation

            if (string.IsNullOrEmpty(searchPattern))
            {
                throw new ArgumentNullException(nameof(searchPattern));
            }

            #endregion

            CloudFileDirectory cloudFileDirectory = CloudFileShare.GetDirectoryReference(path: path);

            IEnumerable<string> fullFilePaths = cloudFileDirectory.EnumerateDirectory(EFileType.Files, searchPattern);

            return fullFilePaths.Select(fullFilePath => fullFilePath.Split(DIRECTORY_SEPARATOR_CHAR).Last());
        }
        
        /// <inheritdoc />
        public IEnumerable<string> GetFiles(string[] path = null)
        {
            CloudFileDirectory cloudFileDirectory = CloudFileShare.GetDirectoryReference(path: path);

            return cloudFileDirectory.EnumerateDirectory(EFileType.Files, "*");
        }

        /// <inheritdoc />
        public Stream GetFileStream(string fileName, string[] path = null)
        {
            #region validation

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            #endregion

            CloudFile cloudFile = GetCloudFileReference(fileName, path);

            return TaskUtilities.ExecuteSync(cloudFile.OpenReadAsync());
        }

        /// <inheritdoc />
        public Stream GetWriteStream(string fileName, string[] path)
        {
            #region validation

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            #endregion

            CloudFileDirectory currentDir = CloudFileShare.GetDirectoryReference(path: path);

            CloudFile cloudFile = currentDir.GetFileReference(fileName);

            return TaskUtilities.ExecuteSync(cloudFile.OpenWriteAsync(1024 * 1024 * 5));
        }

        /// <inheritdoc />
        /// Moves only first tier sub directory and files.
        public void MoveDirectory(string directoryName, string[] sourcePath, string[] targetPath)
        {
            #region validation

            if (string.IsNullOrEmpty(directoryName))
            {
                throw new ArgumentNullException(nameof(directoryName));
            }

            if (sourcePath == targetPath)
            {
                throw new InvalidOperationException("source can´t be equal target");
            }

            #endregion

            CopyDirectory(directoryName, sourcePath, targetPath);

            // delete source directory, because it is move operation
            DeleteDirectory(directoryName, sourcePath);
        }

        /// <inheritdoc />
        public byte[] ReadAllBytes(string fileName, string[] path)
        {
            #region validation

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            #endregion

            CloudFileDirectory cloudFileDirectory = CloudFileShare.GetDirectoryReference(path: path);

            CloudFile cloudFile = cloudFileDirectory.GetFileReference(fileName);

            TaskUtilities.ExecuteSync(cloudFile.FetchAttributesAsync());

            var cloudFileBytes = new byte[cloudFile.Properties.Length];

            TaskUtilities.ExecuteSync(cloudFile.DownloadToByteArrayAsync(cloudFileBytes, 0));

            return cloudFileBytes;

        }

        /// <inheritdoc />
        public void SetFileContent(string fileName, string fileContent, string[] path)
        {
            #region validation

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (string.IsNullOrEmpty(fileContent))
            {
                throw new ArgumentNullException(nameof(fileContent));
            }

            #endregion

            CloudFileDirectory currentDir = CloudFileShare.GetDirectoryReference(path: path);

            CloudFile cloudFile = currentDir.GetFileReference(fileName);

            TaskUtilities.ExecuteSync(cloudFile.UploadTextAsync(fileContent));

        }
        
        /// <inheritdoc />
        public void SetFileStream(string fileName, Stream streamContent, string[] path = null)
        {
            #region validation

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (streamContent == null)
            {
                throw new ArgumentNullException(nameof(streamContent));
            }

            #endregion

            streamContent.Rewind();

            CloudFile cloudFile = GetCloudFileReference(fileName, path);

            TaskUtilities.ExecuteSync(cloudFile.UploadFromStreamAsync(streamContent));

        }

        #endregion

        #region private utilities

        private CloudFile GetCloudFileReference(string fileName, string[] path = null)
        {
            return CloudFileShare
                .GetDirectoryReference(path: path)
                .GetFileReference(fileName);
        }

        private string GetQualifiedUri(IListFileItem listFileItem)
        {
            switch (listFileItem)
            {
                    // absolute path
                    case CloudFile cloudFile:
                        return cloudFile.StorageUri.PrimaryUri.ToString();

                    // relative path
                    case CloudFileDirectory cloudFileDirectory:
                        return $"{cloudFileDirectory.StorageUri.PrimaryUri.AbsolutePath.Replace(CloudFileShare.Name, string.Empty)}";

                    default:
                        throw new ArgumentOutOfRangeException(nameof(listFileItem), "Type not supported");
            }

        }
       
        #endregion
    }
}