using RCV.FileContainer.Contracts;
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
    /// Container definition for access of an on-premise file system.
    /// </summary>
    public class SystemFileContainer : IFileContainer
    {
        #region member

        /// <summary>
        /// Root path of system file container.
        /// </summary>
        private string RootPath { get; }

        private FileUtilities FileUtilities { get; }

        /// <summary>
        /// Encoding for file content.
        /// </summary>
        public static Encoding FileEncoding = Encoding.Unicode;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a file share utility wrapper for on premise infrastructure.
        /// </summary>
        /// <param name="rootPath">Path to the root directory to wrap the file-container around.</param>
        public SystemFileContainer(string rootPath)
        {
            #region validation

            if (string.IsNullOrEmpty(rootPath))
            {
                throw new ArgumentNullException(nameof(rootPath));
            }

            #endregion

            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }

            RootPath = rootPath;
            FileUtilities = new FileUtilities(rootPath);
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

            string sourceDirectoryPath = FileUtilities.GetPath(directoryName, sourcePath);

            string targetDirectoryPath = FileUtilities.GetPath(directoryName, targetPath);

            var sourceDirectoryInfo = new DirectoryInfo(sourceDirectoryPath);

            var targetDirectoryInfo = new DirectoryInfo(targetDirectoryPath);

            if (ExistsDirectory(directoryName, targetPath))
            {
                DeleteDirectory(directoryName, targetPath);
            }

            sourceDirectoryInfo.CopyAllTo(targetDirectoryInfo);
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

            string sourceFilePath = FileUtilities.GetPath(sourceFileName, sourceParentFolders);

            string targetFilePath = FileUtilities.GetPath(targetFileName, targetParentFolders);

            string targetDirectoryPath = FileUtilities.GetPath(targetParentFolders);

            var directoryInfo = new DirectoryInfo(targetDirectoryPath);

            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }

            File.Copy(sourceFilePath, targetFilePath, overwriteIfExists);
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

            string pathToCreate = FileUtilities.GetPath(directoryName, path);

            if (!Directory.Exists(pathToCreate))
            {
                Directory.CreateDirectory(pathToCreate);
            }

            return GetRelativePath(pathToCreate);
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

            CreatePathIfNotExists(path);

            string filePath = FileUtilities.GetPath(fileName, path);

            using (FileStream stream = File.Open(filePath, FileMode.Create))
            {
                stream.Write(fileContent, 0, fileContent.Length);
            }

            return GetRelativePath(filePath);
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

            CreatePathIfNotExists(path);

            string filePath = FileUtilities.GetPath(fileName, path);

            using (FileStream stream = File.Open(filePath, FileMode.Create))
            {
                fileContent.CopyTo(stream);
            }

            return GetRelativePath(filePath);
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

            string directoryPath = FileUtilities.GetPath(directoryName, path);

            if (Directory.Exists(directoryPath))
            {
                Directory.Delete(directoryPath, true);
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

            string filePath = FileUtilities.GetPath(fileName, path);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
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

            string directoryPath = FileUtilities.GetPath(directoryName, path);

            return Directory.Exists(directoryPath);
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

            string filePath = FileUtilities.GetPath(fileName, path);

            return File.Exists(filePath);
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
            string directoryPath = FileUtilities.GetPath(path);

            var directory = new DirectoryInfo(directoryPath);

            return !directory.Exists ? new List<string>() : directory.GetDirectories().Select(d => d.FullName);
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

            string directoryPath = FileUtilities.GetPath(path);

            if (path != null && !ExistsPath(path))
            {
                return new List<string>();
            }

            return Directory.EnumerateDirectories(directoryPath, searchPattern)
                .Select(Path.GetFileName)
                .ToList();
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

            string filePath = FileUtilities.GetPath(fileName, path);

            return File.ReadAllText(filePath, FileEncoding);
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

            string directoryPath = FileUtilities.GetPath(path);

            return Directory.EnumerateFiles(directoryPath, searchPattern).Select(o => Path.GetFileName(o));
        }

        /// <inheritdoc />
        public IEnumerable<string> GetFiles(string[] path = null, bool recursive = false)
        {
            string directoryPath = FileUtilities.GetPath(path);

            Directory.CreateDirectory(directoryPath);

            var directory = new DirectoryInfo(directoryPath);

            return directory.GetFiles().Select(f => f.FullName);
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

            // get directory path
            string directoryPath = FileUtilities.GetPath(path);

            // create file path
            string filePath = FileUtilities.GetPath(fileName, directoryPath);

            // get file and determine full name (e.g. full storage path)
            FileInfo fileInfo = new FileInfo(filePath);
            return fileInfo.FullName;
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

            string directoryPath = FileUtilities.GetPath(path: path);

            string filePath = FileUtilities.GetPath(fileName, directoryPath);

            return File.OpenRead(filePath);
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

            string filePath = FileUtilities.GetPath(fileName, path);

            CreatePathIfNotExists(path);

            return File.OpenWrite(filePath);
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

            string sourceDirectoryPath = FileUtilities.GetPath(directoryName, sourcePath);

            string targetDirectoryPath = FileUtilities.GetPath(directoryName, targetPath);

            Directory.Move(sourceDirectoryPath, targetDirectoryPath);
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

            string fileWithPath = FileUtilities.GetPath(fileName, path);

            return File.ReadAllBytes(fileWithPath);
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

            string filePath = FileUtilities.GetPath(fileName, path);

            CreatePathIfNotExists(path);

            File.WriteAllText(filePath, fileContent, FileEncoding);
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

            string filePath = FileUtilities.GetPath(fileName, path);

            CreatePathIfNotExists(path);

            using FileStream fileStream = File.OpenWrite(filePath);
            streamContent.Rewind();

            streamContent.CopyTo(fileStream);
            fileStream.SetLength(streamContent.Length);
        }

        #endregion

        #region private helper

        /// <summary>
        /// Get path which is relative to root path.
        /// </summary>
        /// <param name="path">Path to get relative path from</param>
        /// <returns>Relative path as array</returns>
        private string[] GetRelativePath(string path)
        {
            if (path.StartsWith(RootPath))
            {
                return path[RootPath.Length..].Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
            }

            return path.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Creates a path in filesystem if path does not exists.
        /// </summary>
        /// <param name="path">Path to create</param>
        private void CreatePathIfNotExists(string[] path)
        {
            // path is not defined, so it is created
            if (path == null)
            {
                return;
            }

            // check if path exists
            if (!ExistsPath(path))
            {
                if (path.Length == 1)
                {
                    // only 1 folder in path
                    CreateDirectory(path.First());
                }
                else
                {
                    // more than 1 folder in path
                    CreateDirectory(path.Last(), path.Take(path.Length - 1).ToArray());
                }
            }
        }

        #endregion
    }
}