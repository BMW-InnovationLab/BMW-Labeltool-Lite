using Rcv.ScriptHost.Contracts.Models;
using Rcv.ScriptHost.Contracts.Services;
using System;

namespace Rcv.ScriptHost.Services.Services
{
    /// <summary>
    /// Implementation of SettingsProvider.
    /// <see cref="ISettingsProvider"/>
    /// </summary>
    public class AppSettingsProvider : ISettingsProvider
    {
        #region private member

        private ScriptHostConfiguration AppSettings { get; set; }

        #endregion

        #region ISettingProvider

        /// <summary>
        /// <see cref="ISettingsProvider.SessionLifetime"/>
        /// </summary>
        public long SessionLifetime
        {
            get
            {
                return AppSettings.SessionLifetime;
            }
        }

        /// <summary>
        /// <see cref="ISettingsProvider.ParametersDirectoryPath"/>
        /// </summary>
        public string ParametersDirectoryPath
        {
            get
            {
                return GetDirectoryPath(AppSettings.ParametersDirectoryPath);
            }
        }

        /// <summary>
        /// <see cref="ISettingsProvider.ResultDirectoryPath"/>
        /// </summary>
        public string ResultDirectoryPath
        {
            get
            {
                return GetDirectoryPath(AppSettings.ResultDirectoryPath);
            }
        }

        /// <summary>
        /// <see cref="ISettingsProvider.ScriptDirectoryPath"/>
        /// </summary>
        public string ScriptDirectoryPath
        {
            get
            {
                return GetDirectoryPath(AppSettings.ScriptDirectoryPath);
            }
        }

        /// <summary>        
        /// <see cref="ISettingsProvider.HostExecutable"/>
        /// </summary>
        public string HostExecutable
        {
            get
            {
                return AppSettings.HostExecutable;
            }
        }

        /// <summary>
        /// <see cref="=ISettingsProvider.ProcessTimeout" />
        /// </summary>
        public int? ProcessTimeout
        {
            get
            {
                return AppSettings.ProcessTimeout;
            }
        }

        #endregion

        #region constructor

        /// <summary>
        /// Create a new instance with given appsettings.
        /// </summary>
        /// <param name="appSettings">Appsettings for provider</param>
        public AppSettingsProvider(ScriptHostConfiguration appSettings)
        {
            AppSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }

        #endregion

        #region helper

        /// <summary>
        /// Get directory path with trailing directory seperator.
        /// </summary>
        /// <param name="directoryPath">Path to directory</param>
        /// <returns>Path to directory with trailing directory seperator</returns>
        private string GetDirectoryPath(string directoryPath)
        {
            string resultPath = directoryPath;
            if (!directoryPath.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
            {
                resultPath = $"{directoryPath}{System.IO.Path.DirectorySeparatorChar}";
            }
            return resultPath;
        }

        #endregion
    }
}
