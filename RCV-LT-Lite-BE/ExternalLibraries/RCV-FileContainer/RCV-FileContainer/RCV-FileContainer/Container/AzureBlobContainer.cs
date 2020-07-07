using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using RCV.FileContainer.Contracts;
using RCV.FileContainer.Enumerations;
using RCV.FileContainer.Extensions;
using RCV.FileContainer.Utilities;

namespace RCV.FileContainer.Container
{
    /// <inheritdoc />
    /// <summary>
    /// Container definition for access of an azure blob container.
    /// </summary>
    public class AzureBlobContainer : IFileContainer
    {
        #region members

        private CloudStorageAccount CloudStorageAccount { get; }

        private CloudBlobContainer CloudBlobContainer { get; }

        private static Encoding FileEncoding => Encoding.UTF8;

        private const char DIRECTORY_SEPARATOR_CHAR = '/';

        #endregion

        #region constructor

        /// <summary>
        /// Creates a blob container utility wrapper for azure infrastructure.
        /// </summary>
        /// <param name="cloudStorageConnectionString">The connection string for azure (valid format needed), e.g. DefaultEndpointsProtocol=https;AccountName=bmwrcvdev;AccountKey=APUfaYGYAOk7owSH2P2Uy/2BWDYxFLJsKgGcG0DyhdYdWS6TRfdEYmKvlk7VewwdaorfetwB2MSfLckQm7a9YA==;EndpointSuffix=core.windows.net</param>
        /// <param name="blobContainerName">The name of configured azure blob container.</param>
        public AzureBlobContainer(string cloudStorageConnectionString, string blobContainerName)
        {
            #region validation

            if (string.IsNullOrWhiteSpace(cloudStorageConnectionString))
            {
                throw new ArgumentNullException(nameof(cloudStorageConnectionString));
            }

            if (string.IsNullOrWhiteSpace(blobContainerName))
            {
                throw new ArgumentNullException(nameof(blobContainerName));
            }

            #endregion

            CloudStorageAccount = CloudStorageAccount.Parse(cloudStorageConnectionString);

            CloudBlobClient cloudBlobClient = CloudStorageAccount.CreateCloudBlobClient();

            CloudBlobContainer = cloudBlobClient.GetContainerReference(blobContainerName);
        }

        #endregion

        #region methods

        /// <inheritdoc />
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

            // Source directory
            CloudBlobDirectory sourceCloudBlobDirectory = CloudBlobContainer.GetCloudBlobDirectoryReference(directoryName, sourcePath);

            // Get blobs and directories on current level
            IEnumerable<IListBlobItem> sourceListBlobItems = sourceCloudBlobDirectory
                .EnumerateDirectory("*")
                .Select<string, IListBlobItem>(
                    listBlobItemName =>
                    {
                        if (listBlobItemName.EndsWith(DIRECTORY_SEPARATOR_CHAR))
                        {
                            return CloudBlobContainer.GetDirectoryReference(listBlobItemName);
                        }

                        return CloudBlobContainer.GetBlobReference(listBlobItemName);
                    });


            // Identify sub virtual directories through evaluation of path segments for each blob reference
            foreach (IListBlobItem listBlobItem in sourceListBlobItems)
            {
                string[] subSource = BlobUtilities.GetPath(directoryName, sourcePath).Split(DIRECTORY_SEPARATOR_CHAR);

                string[] subTarget = BlobUtilities.GetPath(directoryName, targetPath).Split(DIRECTORY_SEPARATOR_CHAR);

                switch (listBlobItem)
                {
                    case CloudBlob cloudBlob:
                        // Get the new blob reference as BlockBlob so we can use the upload text method
                        CloudBlockBlob newCloudBlockBlob = CloudBlobContainer.GetCloudBlockBlobReference(cloudBlob.Uri.Segments.Last(), directoryName, targetPath);

                        // Create the blob
                        TaskUtilities.ExecuteSync(newCloudBlockBlob.UploadTextAsync(string.Empty));
                
                        // Get the new uri
                        var source = new Uri(cloudBlob.GetTargetUri(directoryName, sourcePath));

                        // Copy the blob content
                        TaskUtilities.ExecuteSync(newCloudBlockBlob.StartCopyAsync(source));
                        
                        break;

                    case CloudBlobDirectory cloudBlobDirectory:
                        string lastSegment = cloudBlobDirectory.Uri.Segments.Last();

                        string subDirectoryName = lastSegment.Remove(lastSegment.LastIndexOf(DIRECTORY_SEPARATOR_CHAR));

                        CopyDirectory(subDirectoryName, subSource, subTarget);

                        break;
                }                
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

            if (sourceParentDirectories == targetParentDirectories)
            {
                throw new InvalidOperationException("source can´t be equal target");
            }

            #endregion

            CloudBlockBlob sourceCloudBlockBlob = CloudBlobContainer.GetCloudBlockBlobReference(sourceFileName, path: sourceParentDirectories);

            CloudBlockBlob targetCloudBlockBlob = CloudBlobContainer.GetCloudBlockBlobReference(targetFileName, path: targetParentDirectories);

            if (overwriteIfExists)
            {
                TaskUtilities.ExecuteSync(targetCloudBlockBlob.DeleteIfExistsAsync());
            }

            if (!TaskUtilities.ExecuteSync(targetCloudBlockBlob.ExistsAsync()))
            {
                TaskUtilities.ExecuteSync(targetCloudBlockBlob.StartCopyAsync(sourceCloudBlockBlob));
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Does not actually create a directory since in an azure blob storage based platform there are no real directories, only virtual directories
        /// </summary>
        /// <param name="directoryName">Name of the directory to be created</param>
        /// <param name="path">Path of the directory to be created</param>
        /// <returns>Relative directory path segments</returns>
        public string[] CreateDirectory(string directoryName, string[] path = null)
        {
            #region validation

            if (string.IsNullOrEmpty(directoryName))
            {
                throw new ArgumentNullException(nameof(directoryName));
            }

            #endregion

            CloudBlobDirectory cloudBlobDirectoryReference = CloudBlobContainer.GetCloudBlobDirectoryReference(directoryName, path);

            return GetListBlobItemRelativePath(cloudBlobDirectoryReference);
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

            CloudBlockBlob cloudBlockBlob = CloudBlobContainer.GetCloudBlockBlobReference(fileName, path: path);

            TaskUtilities.ExecuteSync(cloudBlockBlob.UploadFromByteArrayAsync(fileContent, 0, fileContent.Length));

            return GetListItemPathSegments(cloudBlockBlob);
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

            CloudBlockBlob cloudBlockBlob = CloudBlobContainer.GetCloudBlockBlobReference(fileName, path: path);

            TaskUtilities.ExecuteSync(cloudBlockBlob.UploadFromStreamAsync(fileContent));

            return GetListItemPathSegments(cloudBlockBlob);
        }

        /// <inheritdoc />
        public void DeleteDirectory(string directoryName, string[] path = null)
        {
            #region validation

            if (string.IsNullOrEmpty(directoryName))
            {
                throw new ArgumentNullException(nameof(directoryName));
            }

            #endregion

            CloudBlobDirectory cloudBlobDirectory = CloudBlobContainer.GetCloudBlobDirectoryReference(directoryName, path);

            cloudBlobDirectory.DeleteRecursive();
        }
        
        /// <inheritdoc />
        public void DeleteFile(string fileName, string[] path = null)
        {
            #region validation

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            #endregion

            CloudBlockBlob cloudBlockBlob = CloudBlobContainer.GetCloudBlockBlobReference(fileName, path: path);

            TaskUtilities.ExecuteSync(cloudBlockBlob.DeleteIfExistsAsync());
        }

        /// <inheritdoc />
        public bool ExistsDirectory(string directoryName, string[] path = null)
        {
            #region validation

            if (string.IsNullOrEmpty(directoryName))
            {
                throw new ArgumentNullException(nameof(directoryName));
            }

            #endregion

            CloudBlobDirectory cloudBlobDirectory = CloudBlobContainer.GetCloudBlobDirectoryReference(directoryName, path);

            return TaskUtilities.ExecuteSync(cloudBlobDirectory.ListBlobsSegmentedAsync(new BlobContinuationToken())).Results.Any();
        }

        /// <inheritdoc />
        public bool ExistsFile(string fileName, string[] path = null)
        {
            #region validation

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            #endregion

            CloudBlob cloudBlob = CloudBlobContainer.GetCloudBlockBlobReference(fileName, path: path);

            return TaskUtilities.ExecuteSync(cloudBlob.ExistsAsync());
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

            CloudBlobDirectory cloudBlobDirectory = CloudBlobContainer.GetCloudBlobDirectoryReference(path);

            return TaskUtilities.ExecuteSync(cloudBlobDirectory.ListBlobsSegmentedAsync(new BlobContinuationToken())).Results.Any();
        }
        
        /// <inheritdoc />
        public IEnumerable<string> GetDirectories(string[] path = null)
        {
            CloudBlobDirectory cloudBlobDirectory = CloudBlobContainer.GetCloudBlobDirectoryReference(path);

            List<string> fullDirectoryPaths = cloudBlobDirectory.EnumerateDirectory(EBlobType.Directories, "*").ToList();

            return fullDirectoryPaths;
        }
        
        /// <inheritdoc />
        public IEnumerable<string> GetDirectoryNames(string searchPattern, string[] path = null)
        {
            CloudBlobDirectory cloudBlobDirectory = CloudBlobContainer.GetCloudBlobDirectoryReference(path);

            IEnumerable<string> fullDirectoryPaths = cloudBlobDirectory.EnumerateDirectory(EBlobType.Directories, searchPattern);

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

            CloudBlob cloudBlob = CloudBlobContainer.GetCloudBlockBlobReference(fileName, path: path);

            TaskUtilities.ExecuteSync(cloudBlob.FetchAttributesAsync());
            
            var bytes = new byte[cloudBlob.Properties.Length];

            TaskUtilities.ExecuteSync(cloudBlob.DownloadToByteArrayAsync(bytes, 0));

            return FileEncoding.GetString(bytes);
        }

        /// <inheritdoc />
        public IEnumerable<string> GetFileNames(string searchPattern, string[] path = null)
        {
            CloudBlobDirectory cloudBlobDirectory = CloudBlobContainer.GetCloudBlobDirectoryReference(path);

            IEnumerable<string> fullFilePaths = cloudBlobDirectory.EnumerateDirectory(EBlobType.Blobs, searchPattern);

            return fullFilePaths.Select(fullFilePath => fullFilePath.Split(DIRECTORY_SEPARATOR_CHAR).Last());
        }
        
        /// <inheritdoc />
        public IEnumerable<string> GetFiles(string[] path = null)
        {
            CloudBlobDirectory cloudBlobDirectory = CloudBlobContainer.GetCloudBlobDirectoryReference(path);
            
            List<string> fullBlobPaths = cloudBlobDirectory.EnumerateDirectory(EBlobType.Blobs, "*").ToList();

            return fullBlobPaths;
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

            CloudBlob cloudBlob = CloudBlobContainer.GetCloudBlockBlobReference(fileName, path: path);

            return TaskUtilities.ExecuteSync(cloudBlob.OpenReadAsync());
        }

        /// <inheritdoc />
        /// <summary>
        /// Downloads the blob from azure blob storage. Usage of this methods requires a separate call of UploadFromStreamAsync() of a blob;
        /// </summary>
        /// <param name="fileName">Name of the file to get the write stream from</param>
        /// <param name="path">Path of the file to get the write stream from</param>
        /// <returns>A local memory stream.</returns>
        public Stream GetWriteStream(string fileName, string[] path = null)
        {
            #region validation

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            #endregion
            
            CloudBlockBlob cloudBlockBlob = CloudBlobContainer.GetCloudBlockBlobReference(fileName, path: path);

            return TaskUtilities.ExecuteSync(cloudBlockBlob.OpenWriteAsync());
        }
        
        /// <inheritdoc />
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

            CloudBlobDirectory cloudBlobDirectory = CloudBlobContainer.GetCloudBlobDirectoryReference(directoryName, sourcePath);
            
            cloudBlobDirectory.DeleteRecursive();
        }

        /// <inheritdoc />
        public byte[] ReadAllBytes(string fileName, string[] path = null)
        {
            #region validation

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            #endregion
            
            CloudBlockBlob blockBlob = CloudBlobContainer.GetBlockBlobReference(fileName);

            using (var memoryStream = new MemoryStream())
            {
                TaskUtilities.ExecuteSync(blockBlob.DownloadToStreamAsync(memoryStream));

                return memoryStream.ToArray();
            }
        }

        /// <inheritdoc />
        public void SetFileContent(string fileName, string fileContent, string[] path = null)
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

            CloudBlockBlob cloudBlockBlob = CloudBlobContainer.GetCloudBlockBlobReference(fileName, path: path);

            TaskUtilities.ExecuteSync(cloudBlockBlob.UploadTextAsync(fileContent));
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

            CloudBlockBlob cloudBlockBlob = CloudBlobContainer.GetCloudBlockBlobReference(fileName, path: path);

            TaskUtilities.ExecuteSync(cloudBlockBlob.UploadFromStreamAsync(streamContent));
        }
        
        #endregion

        #region private utilities

        private string[] GetListBlobItemRelativePath(IListBlobItem listBlobItem)
        {
            #region validation

            if (listBlobItem == null)
            {
                throw new ArgumentNullException(nameof(listBlobItem));
            }

            #endregion

            return listBlobItem.StorageUri.PrimaryUri.AbsolutePath
                .Replace(CloudBlobContainer.Name, string.Empty)
                .Split(DIRECTORY_SEPARATOR_CHAR, StringSplitOptions.RemoveEmptyEntries);
        }

        private string[] GetListItemPathSegments(IListBlobItem listBlobItem)
        {
            return listBlobItem.StorageUri.PrimaryUri
                .ToString()
                .Split(DIRECTORY_SEPARATOR_CHAR);
        }

        #endregion
    }
}