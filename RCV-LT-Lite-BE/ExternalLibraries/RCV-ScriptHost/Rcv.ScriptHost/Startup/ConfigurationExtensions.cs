using Microsoft.Extensions.Configuration;
using Rcv.ScriptHost.Contracts.Models;

namespace Rcv.ScriptHost
{
    /// <summary>
    /// Extension methods for configuration.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Get configuration for script host.
        /// </summary>
        /// <param name="configuration">Appsettings configuration file</param>
        /// <returns>Configuration of script host</returns>
        public static ScriptHostConfiguration GetScriptHostConfiguration(this IConfiguration configuration)
        {
            return configuration.GetConfiguration<ScriptHostConfiguration>("ScriptHost");
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
