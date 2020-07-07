using Microsoft.Extensions.Configuration;
using Rcv.LabelTool.Backend.Configuration;

namespace Rcv.LabelTool.Backend
{
    /// <summary>
    /// Extension for appsettings.json to get application configuration of labeltool backend.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Get configuration for onPremise-Mode of labeltool backend. Defines where data is stored.
        /// </summary>
        /// <param name="configuration">Appsettings configuration</param>
        /// <returns>OnPremise-mode configuration</returns>
        public static OnPremiseConfiguration GetOnPremiseConfiguration(this IConfiguration configuration)
        {
            return configuration.GetConfiguration<OnPremiseConfiguration>("OnPremise");
        }

        #region private helper

        /// <summary>
        /// Get appsettings configuration section from configuration.
        /// </summary>
        /// <param name="configuration">Configuration to get appsettings from</param>
        /// <returns>AppSettings configuration section</returns>
        private static IConfigurationSection GetAppSettings(this IConfiguration configuration)
        {
            return configuration.GetSection("AppSettings");
        }

        /// <summary>
        /// Get configuration from appsettings of type.
        /// </summary>
        /// <typeparam name="T">Type to get</typeparam>
        /// <param name="configuration">Appsettings configuration</param>
        /// <param name="configurationName">Name of configuration section</param>
        /// <returns>Typed configuration section</returns>
        private static T GetConfiguration<T>(this IConfiguration configuration, string configurationName)
        {
            return configuration.GetAppSettings().GetSection(configurationName).Get<T>();
        }

        #endregion
    }
}
