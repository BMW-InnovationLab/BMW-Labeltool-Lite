using Rcv.ScriptHost.Contracts.Container;
using Rcv.ScriptHost.Contracts.Models;
using Rcv.ScriptHost.Contracts.Services;
using System;

namespace Rcv.ScriptHost.Services.Services
{
    /// <summary>
    /// Implementation class of session service.
    /// </summary>
    public class SessionService : ISessionService
    {
        #region member

        private IFileContainer ParametersFileContainer { get; set; }

        private IFileContainer ResultFileContainer { get; set; }

        private ISettingsProvider SettingsProvider { get; set; }

        private IGuidService GuidService { get; set; }

        private IDateTimeService DateTimeService { get; set; }

        #endregion

        #region constructor

        public SessionService(IFileContainerFactory fileContainerFactory, ISettingsProvider settingsProvider, IGuidService guidService, IDateTimeService dateTimeService)
        {
            GuidService = guidService ?? throw new ArgumentNullException(nameof(guidService));
            SettingsProvider = settingsProvider ?? throw new ArgumentNullException(nameof(settingsProvider));
            DateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));

            if (fileContainerFactory == null)
            {
                throw new ArgumentNullException(nameof(fileContainerFactory));
            }

            ResultFileContainer = fileContainerFactory.ResultsFileContainer;
            ParametersFileContainer = fileContainerFactory.ParametersFileContainer;
        }

        #endregion

        #region ISessionService

        public ScriptSession CreateSession()
        {
            string sessionId = GuidService.GenerateGuid();

            string parametersDirectory = ParametersFileContainer.CreatePath(sessionId);
            string resultDirectory = ResultFileContainer.CreatePath(sessionId);

            ScriptSession pythonSession = new ScriptSession()
            {
                Id = sessionId,
                ExpireDate = DateTimeService.GetNow().AddMilliseconds(SettingsProvider.SessionLifetime).ToUniversalTime(),
                ParametersDirectory = parametersDirectory,
                ResultDirectory = resultDirectory
            };

            return pythonSession;
        }

        #endregion
    }
}
