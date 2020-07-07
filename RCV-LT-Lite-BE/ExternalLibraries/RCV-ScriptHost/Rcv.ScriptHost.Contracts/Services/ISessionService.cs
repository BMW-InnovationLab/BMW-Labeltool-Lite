using Rcv.ScriptHost.Contracts.Models;

namespace Rcv.ScriptHost.Contracts.Services
{
    /// <summary>
    /// Service definition for services for managing sessions.
    /// </summary>
    public interface ISessionService
    {
        /// <summary>
        /// Creates a new session and returns it.
        /// </summary>
        /// <returns>Newly created session</returns>
        ScriptSession CreateSession();
    }
}
