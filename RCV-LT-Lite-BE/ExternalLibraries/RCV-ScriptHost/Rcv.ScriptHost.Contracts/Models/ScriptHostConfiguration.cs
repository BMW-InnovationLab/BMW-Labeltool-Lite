namespace Rcv.ScriptHost.Contracts.Models
{
    /// <summary>
    ///  AppSettings of PythonHost.
    /// </summary>
    public class ScriptHostConfiguration
    {
        /// <summary>
        /// Lifetime of session im milliseconds.
        /// </summary>
        public long SessionLifetime { get; set; }

        /// <summary>
        /// Path where client will transfers files to modify.
        /// Path where host will receive files to modify.
        /// </summary>
        public string ParametersDirectoryPath { get; set; }

        /// <summary>
        /// Path where host will put modified files.
        /// Path where client will receive modified files.
        /// </summary>
        public string ResultDirectoryPath { get; set; }

        /// <summary>
        /// Path where scripts are located.
        /// </summary>
        public string ScriptDirectoryPath { get; set; }

        /// <summary>
        /// Executable to run scripts.
        /// </summary>
        public string HostExecutable { get; set; }

        /// <summary>
        /// Timeout of process.
        /// </summary>
        public int? ProcessTimeout { get; set; }
    }
}
