using Rcv.ScriptHost.Contracts.Container;
using Rcv.ScriptHost.Contracts.Services;
using System;

namespace Rcv.ScriptHost.Services.Containers
{
    /// <summary>
    /// Factory of filecontainer.
    /// </summary>
    public class FileContainerFactory : IFileContainerFactory
    {
        #region member

        /// <summary>
        /// Settings provider for connection strings.
        /// </summary>
        private ISettingsProvider SettingsProvider { get; set; }

        #endregion

        #region IFileContainerFactory

        private static IFileContainer parametersFileContainer;
        public IFileContainer ParametersFileContainer
        {
            get
            {
                if (parametersFileContainer == null)
                {
                    parametersFileContainer = new FileContainer(SettingsProvider.ParametersDirectoryPath);
                    parametersFileContainer.CleanUp();
                }
                return parametersFileContainer;
            }
        }

        private static IFileContainer resultsFileContainer;
        public IFileContainer ResultsFileContainer
        {
            get
            {
                if (resultsFileContainer == null)
                {
                    resultsFileContainer = new FileContainer(SettingsProvider.ResultDirectoryPath);
                    resultsFileContainer.CleanUp();
                }
                return resultsFileContainer;
            }
        }

        private static IFileContainer scriptsFileContainer;
        public IFileContainer ScriptsFileContainer
        {
            get
            {
                if (scriptsFileContainer == null)
                {
                    scriptsFileContainer = new FileContainer(SettingsProvider.ScriptDirectoryPath);
                }
                return scriptsFileContainer;
            }
        }

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="settingsProvider">Provider for settings of filecontainer</param>
        public FileContainerFactory(ISettingsProvider settingsProvider)
        {
            SettingsProvider = settingsProvider ?? throw new ArgumentNullException(nameof(settingsProvider));
        }

        #endregion
    }
}
