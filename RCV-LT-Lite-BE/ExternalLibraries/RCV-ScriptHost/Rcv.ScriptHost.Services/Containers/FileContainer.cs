using Rcv.ScriptHost.Contracts.Container;
using Serilog;
using System;
using System.IO;

namespace Rcv.ScriptHost.Services.Containers
{
    /// <summary>
    /// Implementation of file container based on file system.
    /// </summary>
    public class FileContainer : IFileContainer
    {
        #region member

        /// <summary>
        /// Root path where file container is mounted
        /// </summary>
        private string RootPath { get; set; }

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new instance of file container.
        /// Rootpath will be source for all operations of this container.
        /// </summary>
        /// <param name="fileContainerPath">Source path of container</param>
        public FileContainer(string fileContainerPath)
        {
            #region validation

            if (string.IsNullOrEmpty(fileContainerPath))
            {
                throw new ArgumentNullException(nameof(fileContainerPath));
            }

            #endregion

            if (!Directory.Exists(fileContainerPath))
            {
                Log.Information($"Create file container at path '{fileContainerPath}'");
                // Create directory if it does not exist
                Directory.CreateDirectory(fileContainerPath);
            }

            RootPath = fileContainerPath;
        }

        #endregion

        #region IFileContainer

        /// <summary>
        /// <see cref="IFileContainer.CreatePath(string[])"/>
        /// </summary>
        public string CreatePath(params string[] path)
        {
            string pathToCreate = Path.Combine(RootPath, Path.Combine(path));
            DirectoryInfo directoryInfo = Directory.CreateDirectory(pathToCreate);
            return directoryInfo.FullName;
        }

        public void CleanUp(params string[] path)
        {
            string pathToCleanUp = Path.Combine(RootPath, Path.Combine(path));
            Log.Information($"Cleanup path '{pathToCleanUp}'");

            DirectoryInfo directoryInfo = new DirectoryInfo(pathToCleanUp);

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

    }
}
