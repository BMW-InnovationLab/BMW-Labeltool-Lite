using System.Collections.Generic;
using System.IO;

namespace RCV.FileContainer.Contracts
{
    /// <summary>
    /// Container definition for access of file-container
    /// </summary>
    public interface IFileContainer
    {
        /// <summary>
        /// Copies directory with given name from source to target. Fully recursive only on premise and blob storage. 
        /// </summary>
        /// <param name="directoryName">Name of directory to copy from source to target</param>
        /// <param name="sourcePath">Relative source path to folder</param>
        /// <param name="targetPath">Relative target path to folder</param>
        void CopyDirectory(string directoryName, string[] sourcePath, string[] targetPath);

        /// <summary>
        /// Copies directory with given name from source to target recursively.
        /// Current limitation applies only to azure file storage (only first tier folders and files will be touched).
        /// </summary>
        /// <param name="sourceFileName">Name of source file</param>
        /// <param name="sourceParentFolders">Relative source path to file</param>
        /// <param name="targetFileName">Name of target file</param>
        /// <param name="targetParentFolders">Relative target path to file</param>
        /// <param name="overwriteIfExists">Flag to control overwrite mechanism if file already exists</param>
        void CopyFile(string sourceFileName, string[] sourceParentFolders, string targetFileName, string[] targetParentFolders, bool overwriteIfExists = false);

        /// <summary>
        /// Create directory with specified name at specified location.
        /// </summary>
        /// <param name="directoryName">Directory name to create</param>
        /// <param name="path">Relative path to create</param>
        /// <returns>The full path of the new directory</returns>
        string[] CreateDirectory(string directoryName, string[] path = null);

        /// <summary>
        /// Create file with specified name and content at specified location. Creates needed sub directories itself.
        /// </summary>
        /// <param name="fileName">File name to create</param>
        /// <param name="fileContent">File content to create</param>
        /// <param name="path">Path to create</param>
        /// <returns>The full qualified path of the new file</returns>
        string[] CreateFile(string fileName, byte[] fileContent, string[] path = null);

        /// <summary>
        /// Create file with specified name and content at specified location. Creates needed sub directories itself.
        /// </summary>
        /// <param name="fileName">File name to create</param>
        /// <param name="fileContent">File content to create</param>
        /// <param name="path">Path to create</param>
        /// <returns>The full qualified path of the new file</returns>
        string[] CreateFile(string fileName, Stream fileContent, string[] path = null);

        /// <summary>
        /// Delete directory (and content) with given name.
        /// If path does not exists, nothing will happen.
        /// </summary>
        /// <param name="directoryName">Name of directory</param>
        /// <param name="path">Path to directory</param>
        void DeleteDirectory(string directoryName, string[] path = null);

        /// <summary>
        /// Delete file with given name.
        /// If path does not exists, nothing will happen.
        /// </summary>
        /// <param name="fileName">Name of file</param>
        /// <param name="path">Path to file</param>
        void DeleteFile(string fileName, string[] path = null);

        /// <summary>
        /// Checks if directory with given name exists at given path.
        /// </summary>
        /// <param name="directoryName">Name of directory</param>
        /// <param name="path">Path to directory</param>
        /// <returns>TRUE if directory exists, otherwise FALSE</returns>
        bool ExistsDirectory(string directoryName, string[] path = null);

        /// <summary>
        /// Checks if file with given name exists at given path.
        /// </summary>
        /// <param name="fileName">Name of file</param>
        /// <param name="path">Path to file</param>
        /// <returns>TRUE if file exists, otherwise FALSE</returns>
        bool ExistsFile(string fileName, string[] path = null);

        /// <summary>
        /// Checks if the given path exists.
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <returns>TRUE if path exists, otherwise FALSE</returns>
        bool ExistsPath(string[] path);

        /// <summary>
        /// Get directories from given folder.
        /// If path does not exists, an empty list will be returned.
        /// </summary>
        /// <param name="path">Empty means root otherwise 2nd level folder</param>
        /// <returns>Enumerable of string which contains the full directory paths</returns>
        IEnumerable<string> GetDirectories(string[] path = null);

        /// <summary>
        /// Get names of directory at given path which are matching the given search pattern.
        /// If path does not exists, an empty list will be returned.
        /// </summary>
        /// <param name="searchPattern">Pattern to search for</param>
        /// <param name="path">Path to directory</param>
        /// <returns>Enumerable of strings which contains the directory names</returns>
        IEnumerable<string> GetDirectoryNames(string searchPattern, string[] path = null);

        /// <summary>
        /// Get content of file as string.
        /// </summary>
        /// <param name="fileName">Name of file</param>
        /// <param name="path">Path to file</param>
        /// <returns>Content of file as string</returns>
        string GetFileContent(string fileName, string[] path = null);

        /// <summary>
        /// Get names of files at given directory and path which are matching the given search pattern.
        /// If path does not exists, an empty list will be returned.
        /// </summary>
        /// <param name="searchPattern">Pattern to search for</param>
        /// <param name="path">Path to directory</param>
        /// <returns>List of file-names</returns>
        IEnumerable<string> GetFileNames(string searchPattern, string[] path = null);

        /// <summary>
        /// Get files from given folder.
        /// If directory does not exist, it will be created by the method.
        /// </summary>
        /// <param name="path">Empty means root otherwise 2nd level folder.</param>
        /// <param name="recursive">Recursive.</param>
        /// <returns>Enumerable of string which contains the full file paths</returns>
        IEnumerable<string> GetFiles(string[] path = null, bool recursive = false);

        /// <summary>
        /// Get full storage file path of file which is stored with given
        /// filename and path.
        /// </summary>
        /// <param name="fileName">Name of file ot get full filepath of</param>
        /// <param name="path">Relative path where file is stored in container</param>
        /// <returns>Full and absolute file path including the filename</returns>
        string GetFilePath(string fileName, string[] path = null);

        /// <summary>
        /// Get file content as stream.
        /// </summary>
        /// <param name="fileName">Name of file</param>
        /// <param name="path">Path to file</param>
        /// <returns>Content of file as stream</returns>
        Stream GetFileStream(string fileName, string[] path = null);

        /// <summary>
        /// Get stream to write content.
        /// If path does not exists, it will be created.
        /// </summary>
        /// <param name="fileName">Name of file</param>
        /// <param name="path">Path to file</param>
        /// <returns>Stream to write content to</returns>
        Stream GetWriteStream(string fileName, string[] path = null);

        /// <summary>
        /// Moves directory with given name from source to target recursively.
        /// Current limitation applies only to azure file storage (only first tier folders and files will be touched).
        /// </summary>
        /// <param name="directoryName">Name of directoryName to move from source to target</param>
        /// <param name="sourcePath">Relative source path to folder</param>
        /// <param name="targetPath">Relative target path to folder</param>
        void MoveDirectory(string directoryName, string[] sourcePath, string[] targetPath);

        /// <summary>
        /// Reads out all content as byte array of a file with specified name and location.
        /// </summary>
        /// <param name="fileName">File name of desired file</param>
        /// <param name="path">Location of desired file</param>
        /// <returns>Content of the file as byte array</returns>
        byte[] ReadAllBytes(string fileName, string[] path = null);

        /// <summary>
        /// Set content of file with given string-content.
        /// If path does not exists, it will be created.
        /// </summary>
        /// <param name="fileName">Name of file</param>
        /// <param name="fileContent">Content of file as string</param>
        /// <param name="path">Path to file</param>
        void SetFileContent(string fileName, string fileContent, string[] path = null);

        /// <summary>
        /// Set file content of file (specified by filename, parent directory and directory path) with stream-content.
        /// If path does not exists, it will be created.
        /// </summary>
        /// <param name="fileName">Name of file</param>
        /// <param name="streamContent">Content of file as stream</param>
        /// <param name="path">Path to the directory</param>
        void SetFileStream(string fileName, Stream streamContent, string[] path = null);
    }
}