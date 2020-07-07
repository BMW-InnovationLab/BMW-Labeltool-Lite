using Rcv.ScriptHost.Contracts.Models;

namespace Rcv.ScriptHost.Contracts.Services
{
    /// <summary>
    /// Host definition for run command on command line.
    /// </summary>
    public interface IProcessHost
    {
        /// <summary>
        /// Run process and return output to result.
        /// </summary>
        /// <param name="scriptCommand">Command to run (only arguments)</param>
        /// <returns>Result of scirpt</returns>
        ProcessResult RunProcess(string scriptCommand);
    }
}
