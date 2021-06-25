using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using RCV.FileContainer.Contracts;
using RCV.FileContainer.Enumerations;
using RCV.FileContainer.Extensions;
using RCV.FileContainer.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RCV.FileContainer.Container
{
    /// <inheritdoc />
    /// <summary>
    /// Container definition for access of an azure blob container.
    /// </summary>
    public class AzureBlobContainer : IFileContainer
    {
        #region members

        private BlobContainerClient BlobContainerClient { get; }

        private static Encoding FileEncoding => Encoding.UTF8;

        private const char DIRECTORY_SEPARATOR_CHAR = '/';

        private string[] CloudBlobContainerPrefix { get; }

        #endregion

        #region constructor

        /// <summary>
        /// Creates a blob container utility wrapper for azure infrastructure.
        /// </summary>
        /// <param name="cloudStorageConnectionString">The connection string for azure (valid format needed), e.g. DefaultEndpointsProtocol=https;AccountName=bmwrcvdev;AccountKey=APUfaYGYAOk7owSH2P2Uy/2BWDYxFLJsKgGcG0DyhdYdWS6TRfdEYmKvlk7VewwdaorfetwB2MSfLckQm7a9YA==;EndpointSuffix=core.windows.net</param>
        /// <param name="blobContainerName">The name of configured azure blob container.</param>
        /// <param name="blobContainerPrefix">Container Prefix. Path of folders inside the bucket itself where root directory is located.</param>
        public AzureBlobContainer(string cloudStorageConnectionString, string blobContainerName, string blobContainerPrefix = null)
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

            BlobContainerClient = new BlobServiceClient(cloudStorageConnectionString).GetBlobContainerClient(blobContainerName);
            CloudBlobContainerPrefix = StorePathUtil.GetStoragePath(blobContainerPrefix);
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

            #endregion

            if (sourcePath.IsEqualTo(targetPath))
            {
                // return if source and target are the same directory
                // nothing to do here
                return;
            }

            // Identify sub virtual directories through evaluation of path segments for each blob reference
            foreach (var listBlobItem in BlobContainerClient.GetBlobs(prefix: BlobUtilities.GetPath(directoryName, GetContainerPath(sourcePath))))
            {
                // Get the new blob reference as BlockBlob so we can use the upload text method
                BlockBlobClient newCloudBlockBlob = GetCloudBlockBlobReference(listBlobItem.Name[(listBlobItem.Name.LastIndexOf(DIRECTORY_SEPARATOR_CHAR + directoryName + DIRECTORY_SEPARATOR_CHAR) + directoryName.Length + 2)..], directoryName, targetPath);

                // Create the blob
                using (var memoryStream = new MemoryStream(FileEncoding.GetBytes(string.Empty)))
                {
                    newCloudBlockBlob.Upload(memoryStream);
                }

                // Copy the blob content
                newCloudBlockBlob.StartCopyFromUri(new Uri(newCloudBlockBlob.Uri.AbsoluteUri));
            }
        }

        /// <inheritdoc />
        public void CopyFile(string sourceFileName, string[] sourceParentFolders, string targetFileName, string[] targetParentFolders, bool overwriteIfExists = false)
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

            if (sourceParentFolders.IsEqualTo(targetParentFolders) && sourceFileName.Equals(targetFileName))
            {
                // source and target file are the same so no copy action has to be performed
                return;
            }

            BlockBlobClient targetCloudBlockBlob = GetCloudBlockBlobReference(targetFileName, targetParentFolders);

            if (overwriteIfExists)
            {
                targetCloudBlockBlob.DeleteIfExists();
            }

            if (!targetCloudBlockBlob.Exists())
            {
                targetCloudBlockBlob.StartCopyFromUri(GetCloudBlockBlobReference(sourceFileName, sourceParentFolders).Uri);
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

            return GetListBlobItemRelativePath(BlobContainerClient.GetBlockBlobClient(directoryName));
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

            BlockBlobClient cloudBlockBlob = GetCloudBlockBlobReference(fileName, path);

            using (var memoryStream = new MemoryStream(fileContent))
            {
                cloudBlockBlob.Upload(memoryStream);
            }

            return GetListBlobItemRelativePath(cloudBlockBlob);
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

            BlockBlobClient cloudBlockBlob = GetCloudBlockBlobReference(fileName, path);

            cloudBlockBlob.Upload(fileContent);

            return GetListBlobItemRelativePath(cloudBlockBlob);
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

            foreach (var file in GetFiles(BlobUtilities.GetPath(directoryName, path).Split(DIRECTORY_SEPARATOR_CHAR), true))
            {
                BlobContainerClient.GetBlockBlobClient(file[(file.IndexOf(DIRECTORY_SEPARATOR_CHAR + BlobContainerClient.Name + DIRECTORY_SEPARATOR_CHAR) + BlobContainerClient.Name.Length + 2)..]).Delete();
            }
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

            GetCloudBlockBlobReference(fileName, path).DeleteIfExists();
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

            return BlobContainerClient.GetBlobs(prefix: BlobUtilities.GetPath(directoryName, GetContainerPath(path))).Any();
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

            return GetCloudBlockBlobReference(fileName, path).Exists();
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

            return BlobContainerClient.GetBlobs(prefix: BlobUtilities.GetPath(GetContainerPath(path))).Any();
        }

        /// <inheritdoc />
        public IEnumerable<string> GetDirectories(string[] path = null)
        {
            return BlobContainerClient.EnumerateDirectory(EBlobType.Directories, "*", string.Join(DIRECTORY_SEPARATOR_CHAR, GetContainerPath(path)), false).ToList();
        }

        /// <inheritdoc />
        public IEnumerable<string> GetDirectoryNames(string searchPattern, string[] path = null)
        {
            return BlobContainerClient.EnumerateDirectory(EBlobType.Directories, searchPattern, string.Join(DIRECTORY_SEPARATOR_CHAR, GetContainerPath(path)), false).Select(fullDirectoryPath => fullDirectoryPath.Split(DIRECTORY_SEPARATOR_CHAR).Last());
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

            string text = string.Empty;
            using (var memoryStream = new MemoryStream())
            {
                GetCloudBlockBlobReference(fileName, path).DownloadTo(memoryStream);
                text = FileEncoding.GetString(memoryStream.ToArray());
            }

            return text;
        }

        /// <inheritdoc />
        public IEnumerable<string> GetFileNames(string searchPattern, string[] path = null)
        {
            IEnumerable<string> fullFilePaths = BlobContainerClient.EnumerateDirectory(EBlobType.Blobs, searchPattern, string.Join(DIRECTORY_SEPARATOR_CHAR, GetContainerPath(path)), false);

            return fullFilePaths.Select(fullFilePath => fullFilePath.Split(DIRECTORY_SEPARATOR_CHAR).Last());
        }

        /// <inheritdoc />
        public string GetFilePath(string fileName, string[] path = null)
        {
            #region validation

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            #endregion

            return BlobContainerClient.Uri.AbsoluteUri + DIRECTORY_SEPARATOR_CHAR + GetCloudBlockBlobReference(fileName, path).Name;
        }

        /// <inheritdoc />
        public IEnumerable<string> GetFiles(string[] path = null, bool recursive = false)
        {
            return BlobContainerClient.EnumerateDirectory(EBlobType.Blobs, "*", string.Join(DIRECTORY_SEPARATOR_CHAR, GetContainerPath(path)), recursive).ToList();
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

            // download whole file to memory
            var memoryStream = new MemoryStream();
            GetCloudBlockBlobReference(fileName, path).DownloadTo(memoryStream);

            memoryStream.Rewind();
            return memoryStream;
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

            return GetCloudBlockBlobReference(fileName, path).OpenWrite(true);
        }

        /// <inheritdoc />
        public void MoveDirectory(string directoryName, string[] sourcePath, string[] targetPath)
        {
            #region validation

            if (string.IsNullOrEmpty(directoryName))
            {
                throw new ArgumentNullException(nameof(directoryName));
            }

            #endregion

            if (sourcePath.IsEqualTo(targetPath))
            {
                // return if source and target are the same directory
                // nothing to do here
                return;
            }

            CopyDirectory(directoryName, sourcePath, targetPath);

            foreach (var listBlobItem in BlobContainerClient.GetBlobs(prefix: BlobUtilities.GetPath(directoryName, GetContainerPath(sourcePath))))
            {
                GetCloudBlockBlobReference(listBlobItem.Name[(listBlobItem.Name.LastIndexOf(DIRECTORY_SEPARATOR_CHAR + directoryName + DIRECTORY_SEPARATOR_CHAR) + directoryName.Length + 2)..], directoryName, sourcePath).Delete();
            }
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

            using var memoryStream = new MemoryStream();
            GetCloudBlockBlobReference(fileName, path).DownloadTo(memoryStream);

            return memoryStream.ToArray();
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

            using var memoryStream = new MemoryStream(FileEncoding.GetBytes(fileContent));
            GetCloudBlockBlobReference(fileName, path).Upload(memoryStream);
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

            GetCloudBlockBlobReference(fileName, path).Upload(streamContent);
        }

        #endregion

        #region private utilities

        private BlockBlobClient GetCloudBlockBlobReference(string blobName, string[] blobPath = null)
        {
            return GetCloudBlockBlobReference(blobName, null, blobPath);
        }

        private BlockBlobClient GetCloudBlockBlobReference(string blobName, string blobDirectoryName = null, string[] blobPath = null)
        {
            return BlobContainerClient.GetCloudBlockBlobReference(blobName, blobDirectoryName, GetContainerPath(blobPath));
        }

        private string[] GetContainerPath(string[] itemPath)
        {
            return StorePathUtil.JoinPaths(CloudBlobContainerPrefix, itemPath);
        }


        private string[] GetListBlobItemRelativePath(BlockBlobClient blobItem)
        {
            #region validation

            if (blobItem == null)
            {
                throw new ArgumentNullException(nameof(blobItem));
            }

            #endregion

            string pathPrefix = $"{DIRECTORY_SEPARATOR_CHAR}{BlobContainerClient.Name}{DIRECTORY_SEPARATOR_CHAR}";
            if (CloudBlobContainerPrefix != null)
            {
                pathPrefix += string.Join(DIRECTORY_SEPARATOR_CHAR, CloudBlobContainerPrefix) + DIRECTORY_SEPARATOR_CHAR;
            }

            string relativeItemPath = $"{DIRECTORY_SEPARATOR_CHAR}{BlobContainerClient.Name}{DIRECTORY_SEPARATOR_CHAR}{blobItem.Name}";
            if (relativeItemPath.StartsWith(pathPrefix))
            {
                // remove path prefix only at the beginning of the path.
                // do not touch path in the middle or at the end
                relativeItemPath = relativeItemPath[pathPrefix.Length..];
            }

            return relativeItemPath.Split(DIRECTORY_SEPARATOR_CHAR, StringSplitOptions.RemoveEmptyEntries);
        }

        #endregion
    }
}