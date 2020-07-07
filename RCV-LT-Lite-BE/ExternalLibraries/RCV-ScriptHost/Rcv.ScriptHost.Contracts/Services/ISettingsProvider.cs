namespace Rcv.ScriptHost.Contracts.Services
{
    /// <summary>
    /// Settings provider. Will return application settings
    /// in correct way. Settings are approved by logic inside settings 
    /// provider.
    /// </summary>
    public interface ISettingsProvider
    {
        /// <summary>
        /// Lifetime of session in milliseconds.
        /// </summary>
        long SessionLifetime { get; }

        /// <summary>
        /// Directory path for parameters directory.
        /// </summary>
        string ParametersDirectoryPath { get; }

        /// <summary>
        /// Directory path for result directory.
        /// </summary>
        string ResultDirectoryPath { get; }

        /// <summary>
        /// Directory path for script directory.
        /// </summary>
        string ScriptDirectoryPath { get; }

        /// <summary>
        /// Name/command of executable to run with.
        /// </summary>
        string HostExecutable { get; }

        /// <summary>
        /// Timeout for started process in milliseconds.
        /// When timeout is reached, process will be terminated.
        /// </summary>
        int? ProcessTimeout { get; }
    }
}
