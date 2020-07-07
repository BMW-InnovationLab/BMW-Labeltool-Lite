using Newtonsoft.Json;
using Rcv.ScriptHost.Contracts.Models;
using System;
using System.Collections.Generic;

namespace Rcv.ScriptClient.Result
{
    /// <summary>
    /// Static class for parsing result from script host.
    /// </summary>
    static class ScriptHostResult
    {
        /// <summary>
        /// Parse result from new session request.
        /// </summary>
        /// <param name="response">Response of request</param>
        /// <returns>Newly created script session</returns>
        public static ScriptSession ParseSessionResult(string response)
        {
            #region validation

            if (string.IsNullOrEmpty(response))
            {
                throw new ArgumentNullException(nameof(response));
            }

            #endregion

            return JsonConvert.DeserializeObject<ScriptSession>(response);
        }

        /// <summary>
        /// Parse result from execute scripts request.
        /// </summary>
        /// <param name="response">Response of request</param>
        /// <returns>List of results of script execution</returns>
        public static IEnumerable<ScriptExecutionResult> ParseScriptExecutionResult(string response)
        {
            #region validation

            if (string.IsNullOrEmpty(response))
            {
                throw new ArgumentNullException(nameof(response));
            }

            #endregion

            return JsonConvert.DeserializeObject<IEnumerable<ScriptExecutionResult>>(response);
        }

        /// <summary>
        /// Parse end session result. This method will never called, because
        /// result is empty and exception should be cachtched!
        /// </summary>
        /// <param name="response">Response from end session request</param>
        /// <returns>Nonse bool TRUE</returns>
        public static bool ParseEndSessionResult(string response)
        {
            return true;
        }
    }
}
