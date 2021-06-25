using Serilog;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Rcv.FileUtils.AccessControl
{
    /// <summary>
    /// Class for represenatation of single file access. This class encapsulates the functionality
    /// for file access without concurrent reads/writes.
    /// </summary>
    public static class FileSession
    {
        #region member

        /// <summary>
        /// Dictionary of Mutexs for specific files.
        /// </summary>
        private static ConcurrentDictionary<string, Mutex> MutexDictionary = new ConcurrentDictionary<string, Mutex>();

        #endregion

        #region delegate functions

        /// <summary>
        /// Delegate function for file action.
        /// </summary>
        /// <typeparam name="T">Result of file access action</typeparam>
        /// <param name="fileName">Name of file</param>
        /// <param name="filePath">Relative path to file</param>
        /// <returns>Result of file operation</returns>
        public delegate T FileSessionAction<T>(string fileName, string[] filePath);

        #endregion

        #region FileSession

        /// <summary>
        /// Perform session safe file access to specified file.
        /// </summary>
        /// <typeparam name="T">Type of file result</typeparam>
        /// <param name="action">Action to perform with file</param>
        /// <param name="fileName">Name of file</param>
        /// <param name="filePath">Path to file</param>
        /// <returns>Result of file action</returns>
        public static T Execute<T>(FileSessionAction<T> action, string fileName, string[] filePath = null)
        {
            #region validation

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            #endregion

            // determine session name from filename and filepath
            string sessionName = GetSessionName(fileName, filePath);

            Log.Debug($"Session '{sessionName}' starts. Gain access to '{fileName}'.");

            T result = default(T);
            try
            {
                // request mutex for file access
                BeginSession(sessionName);

                // access file
                result = action.Invoke(fileName, filePath);
            }
            finally
            {
                // release mutex of file access
                EndSession(sessionName);

                Log.Debug($"Session '{sessionName}' ends");
            }

            return result;
        }

        #endregion

        #region private helper

        /// <summary>
        /// Begin sessing with given name. Waits for the mutex.
        /// </summary>
        /// <param name="sessionName">Name of session to begin</param>
        private static void BeginSession(string sessionName)
        {
            if (!MutexDictionary.ContainsKey(sessionName))
            {
                MutexDictionary.AddOrUpdate(sessionName, new Mutex(), (session, mutex) =>
                {
                    return mutex;
                });
            }

            MutexDictionary[sessionName].WaitOne();
        }

        /// <summary>
        /// End session with given name. Releases the mutex for the
        /// given file.
        /// </summary>
        /// <param name="sessionName">Name of session to end</param>
        private static void EndSession(string sessionName)
        {
            if (MutexDictionary.ContainsKey(sessionName))
            {
                MutexDictionary[sessionName].ReleaseMutex();
            }
        }

        /// <summary>
        /// Determine session name from filename and path. The session name should
        /// be unique for each file.
        /// </summary>
        /// <example>A valid session name is path_file.json</example>
        /// <param name="fileName">Name of file</param>
        /// <param name="filePath">Relative path of file</param>
        /// <returns>Unique identifier for session</returns>
        private static string GetSessionName(string fileName, string[] filePath)
        {
            string sessionName = fileName;
            if (filePath != null)
            {
                foreach (string subPath in filePath)
                {
                    sessionName = $"{subPath}_{sessionName}";
                }
            }
            return sessionName;
        }

        #endregion
    }
}
