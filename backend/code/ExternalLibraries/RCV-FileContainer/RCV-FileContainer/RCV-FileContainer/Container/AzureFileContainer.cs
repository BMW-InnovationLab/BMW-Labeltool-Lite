using Azure.Storage.Files.Shares;
using RCV.FileContainer.Contracts;
using RCV.FileContainer.Enumerations;
using RCV.FileContainer.Extensions;
using RCV.FileContainer.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace RCV.FileContainer.Container
{
    /// <inheritdoc />
    /// <summary>
    /// Container definition for access of an azure file share.
    /// </summary>
    public class AzureFileContainer : IFileContainer
    {
        #region member

        private ShareClient CloudFileShare { get; }

        private const string DIRECTORY_SEPARATOR_CHAR = "/";

        private string[] CloudFileSharePrefix { get; }

        #endregion

        #region constructor

        /// <summary>
        /// Creates a file share utility wrapper for azure infrastructure.
        /// </summary>
        /// <param name="cloudStorageConnectionString">The connection string for azure (valid format needed), e.g. DefaultEndpointsProtocol=https;AccountName=bmwrcvdev;AccountKey=APUfaYGYAOk7owSH2P2Uy/2BWDYxFLJsKgGcG0DyhdYdWS6TRfdEYmKvlk7VewwdaorfetwB2MSfLckQm7a9YA==;EndpointSuffix=core.windows.net</param>
        /// <param name="shareName">The name of configured azure file share.</param>
        /// <param name="sharePrefix">Prefix of share. Path of folders inside the bucket itself where root directory is located.</param>
        public AzureFileContainer(string cloudStorageConnectionString, string shareName, string sharePrefix = null)
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

            CloudFileShare = new ShareClient(cloudStorageConnectionString, shareName);
            CloudFileSharePrefix = StorePathUtil.GetStoragePath(sharePrefix);
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

            #endregion

            if (sourcePath.IsEqualTo(targetPath))
            {
                // return if source and target are the same directory
                // nothing to do here
                return;
            }

            string[] source = sourcePath == null ?
                new[] { directoryName } :
                sourcePath.Append(directoryName);

            // get all files from source directory
            IEnumerable<string> sourceDirectoryFileNames = GetFiles(source)
                .Select(fullFilePath => fullFilePath[fullFilePath.LastIndexOf(DIRECTORY_SEPARATOR_CHAR, StringComparison.Ordinal)..].Remove(0, 1));

            // create target directory
            CreateDirectory(directoryName, targetPath);

            foreach (string sourceDirectoryFileName in sourceDirectoryFileNames)
            {
                string[] target = targetPath == null ?
                        new[] { directoryName } :
                        targetPath.Append(directoryName);

                using Stream sourceStream = GetFileStream(sourceDirectoryFileName, source);
                SetFileStream(sourceDirectoryFileName, sourceStream, target);
            }

            foreach (var listFileItem in GetDirectoryReference(directoryName, sourcePath).GetFilesAndDirectories(null, new CancellationToken()))
            {
                if (!listFileItem.IsDirectory)
                {
                    continue;
                }

                // Get target directory
                ShareDirectoryClient targetCloudFileDirectory = GetDirectoryReference(directoryName, targetPath);

                // Get new sub directory to be created
                ShareDirectoryClient subCloudFileDirectory = targetCloudFileDirectory.GetSubdirectoryClient(listFileItem.Name);

                // Create new sub directory
                subCloudFileDirectory.CreateIfNotExists();
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

            ShareFileClient sourceFile = GetCloudFileReference(sourceFileName, sourceParentFolders);
            ShareFileClient targetFile = GetCloudFileReference(targetFileName, targetParentFolders);

            if (overwriteIfExists)
            {
                targetFile.DeleteIfExists();
            }

            if (!targetFile.Exists())
            {
                targetFile.StartCopy(sourceFile.Uri);
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

            return GetRelativePath(GetDirectoryReference(path).Uri.AbsolutePath);
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

            ShareFileClient newFile = GetDirectoryReference(path).GetFileClient(fileName);

            using var memoryStream = new MemoryStream(fileContent);
            newFile.Upload(memoryStream);

            return GetRelativePath(newFile.Uri.AbsolutePath);
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

            ShareFileClient newFile = GetDirectoryReference(path).GetFileClient(fileName);

            newFile.Upload(fileContent);

            return GetRelativePath(newFile.Uri.AbsolutePath);
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

            GetDirectoryReference(directoryName, path).DeleteRecursive();
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

            GetDirectoryReference(path).GetFileClient(fileName).DeleteIfExists();
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

            return GetDirectoryReference(directoryName, path, false).Exists();
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

            ShareDirectoryClient currentDir = GetDirectoryReference(path);

            if (!currentDir.Exists())
            {
                return false;

            }

            ShareFileClient file = currentDir.GetFileClient(fileName);

            return file != null && file.Exists();
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
        public IEnumerable<string> GetDirectories(string[] path = null)
        {
            return GetDirectoryReference(path).EnumerateDirectory(EFileType.Directories, "*");
        }

        /// <inheritdoc />
        public IEnumerable<string> GetDirectoryNames(string searchPattern, string[] path = null)
        {
            #region validation

            if (string.IsNullOrEmpty(searchPattern))
            {
                throw new ArgumentNullException(nameof(searchPattern));
            }

            #endregion

            return GetDirectoryReference(path).EnumerateDirectory(EFileType.Directories, searchPattern).Select(fullDirectoryPath => fullDirectoryPath.Split(DIRECTORY_SEPARATOR_CHAR).Last());
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

            return new StreamReader(GetDirectoryReference(path).GetFileClient(fileName).Download().GetRawResponse().ContentStream).ReadToEnd();
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

            return GetDirectoryReference(path).EnumerateDirectory(EFileType.Files, searchPattern).Select(fullFilePath => fullFilePath.Split(DIRECTORY_SEPARATOR_CHAR).Last());
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

            return GetCloudFileReference(fileName, path).Uri.AbsoluteUri;
        }

        /// <inheritdoc />
        public IEnumerable<string> GetFiles(string[] path = null, bool recursive = false)
        {
            return GetDirectoryReference(path).EnumerateDirectory(EFileType.Files, "*");
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

            return GetCloudFileReference(fileName, path).Download().GetRawResponse().ContentStream;
        }

        /// <inheritdoc />
        public Stream GetWriteStream(string fileName, string[] path = null)
        {
            #region validation

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            #endregion

            return GetDirectoryReference(path).GetFileClient(fileName).OpenWrite(true, 1024 * 1024 * 5);
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

            #endregion

            if (sourcePath.IsEqualTo(targetPath))
            {
                // return if source and target are the same directory
                // nothing to do here
                return;
            }

            CopyDirectory(directoryName, sourcePath, targetPath);

            // delete source directory, because it is move operation
            DeleteDirectory(directoryName, sourcePath);
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
            using var contentStream = GetDirectoryReference(path).GetFileClient(fileName).Download().GetRawResponse().ContentStream;
            contentStream.CopyTo(memoryStream);

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

            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream);
            streamWriter.Write(fileContent);
            streamWriter.Flush();
            memoryStream.Rewind();
            GetDirectoryReference(path).GetFileClient(fileName).Upload(memoryStream);

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

            GetCloudFileReference(fileName, path).Upload(streamContent);

        }

        #endregion

        #region private utilities

        private ShareDirectoryClient GetDirectoryReference(string[] directoryPath)
        {
            return GetDirectoryReference(null, directoryPath);
        }

        private ShareDirectoryClient GetDirectoryReference(string directoryName, string[] directoryPath, bool createIfNotExists = true)
        {
            return CloudFileShare.GetDirectoryReference(directoryName, GetSharePath(directoryPath), createIfNotExists);
        }

        private ShareFileClient GetCloudFileReference(string fileName, string[] path = null)
        {
            return GetDirectoryReference(path).GetFileClient(fileName);
        }

        private string[] GetSharePath(string[] itemPath)
        {
            return StorePathUtil.JoinPaths(CloudFileSharePrefix, itemPath);
        }

        private string[] GetRelativePath(string absolutePath)
        {
            string pathPrefix = $"{DIRECTORY_SEPARATOR_CHAR}{CloudFileShare.Name}{DIRECTORY_SEPARATOR_CHAR}";
            if (CloudFileSharePrefix != null)
            {
                pathPrefix += string.Join(DIRECTORY_SEPARATOR_CHAR, CloudFileSharePrefix) + DIRECTORY_SEPARATOR_CHAR;
            }

            string relativeItemPath = absolutePath;
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