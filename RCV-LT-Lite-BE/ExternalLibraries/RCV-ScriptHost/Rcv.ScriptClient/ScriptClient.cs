using Newtonsoft.Json;
using Rcv.HttpUtils;
using Rcv.HttpUtils.Exceptions;
using Rcv.ScriptClient.Result;
using Rcv.ScriptHost.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Rcv.ScriptClient
{
    /// <summary>
    /// Client class for python host.
    /// </summary>
    public class ScriptClient
    {
        public delegate void CopyFiles(ScriptSession session);

        #region member

        /// <summary>
        /// Endpoint path on Python host where scripts are located.
        /// </summary>
        public string ScriptPath { get; set; } = "api/script";

        /// <summary>
        /// List with script execution results.
        /// </summary>
        private IEnumerable<ScriptExecutionResult> ScriptExecutionResults { get; set; }

        #endregion

        #region statics

        /// <summary>
        /// Media type of request to python-host.
        /// </summary>
        private static string RequestMediaType { get; set; } = "application/json";

        /// <summary>
        /// Encodig setting of request to python-host.
        /// </summary>
        private static Encoding RequestEncoding { get; set; } = Encoding.UTF8;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new instance of python host.
        /// </summary>
        public ScriptClient()
        {
        }

        #endregion

        #region PythonClient

        /// <summary>
        /// Run scripts which are choosed.
        /// </summary>
        /// <param name="httpClient">HttpClient to use for requests</param>
        /// <param name="scripts">List of scripts to run</param>
        /// <param name="copyParameterFiles">Function for copy parameter files</param>
        /// <param name="copyResultFiles">Function for copy result files</param>
        /// <returns>TRUE if scripts executed successfully, otherwise FALSE</returns>
        public bool RunScripts(HttpClient httpClient, IEnumerable<Script> scripts, CopyFiles copyParameterFiles = null, CopyFiles copyResultFiles = null)
        {
            return RunScriptsInternal(httpClient, scripts, false, copyParameterFiles, copyResultFiles);
        }

        /// <summary>
        /// Run scripts in sequence. Scripts will be executed with result of previous script exection.
        /// With this method an combination of scripts is possible.
        /// </summary>
        /// <param name="httpClient">HttpClient to use for requests</param>
        /// <param name="scripts">List of scripts to run</param>
        /// <param name="copyParameterFiles">Function for copy parameter files</param>
        /// <param name="copyResultFiles">Function for copy result files</param>
        /// <returns>TRUE is scripts executed successfully, otherwise FALSE</returns>
        public bool RunScriptsInSequence(HttpClient httpClient, IEnumerable<Script> scripts, CopyFiles copyParameterFiles = null, CopyFiles copyResultFiles = null)
        {
            return RunScriptsInternal(httpClient, scripts, true, copyParameterFiles, copyResultFiles);
        }

        #endregion

        #region private methods

        private bool RunScriptsInternal(HttpClient httpClient, IEnumerable<Script> scripts, bool runInSequence = false, CopyFiles copyParameterFiles = null, CopyFiles copyResultFiles = null)
        {
            #region validation

            if (httpClient == null)
            {
                throw new ArgumentNullException(nameof(httpClient));
            }

            if (scripts == null)
            {
                throw new ArgumentNullException(nameof(scripts));
            }

            #endregion

            // No scripts avaiable for running
            // No python session has to be established
            if (scripts.Count() == 0)
            {
                return false;
            }

            ScriptSession pythonSession = GetPythonSession(httpClient);
            if (pythonSession == null)
            {
                throw new HostUnreachableException($"Script execution host is unreachable at the moment!");
            }

            copyParameterFiles?.Invoke(pythonSession);

            ScriptExecutionResults = ExecuteScripts(httpClient, pythonSession, scripts, runInSequence);

            copyResultFiles?.Invoke(pythonSession);

            EndPythonSession(httpClient, pythonSession);

            return ScriptExecutionResults != null && ScriptExecutionResults.All(o => !o.IsFailed);
        }

        #endregion

        #region requests to api

        /// <summary>
        /// Request new session from server.
        /// </summary>
        /// <returns>Newly created session or NULL</returns>
        private ScriptSession GetPythonSession(HttpClient httpClient)
        {
            // uri
            Uri requestUri = new Uri($"{httpClient.BaseAddress}{ScriptPath}");

            // request
            return httpClient.Get(requestUri, ScriptHostResult.ParseSessionResult);
        }

        /// <summary>
        /// Delete session on server.
        /// </summary>
        /// <param name="pythonSession">Session to delete</param>
        private void EndPythonSession(HttpClient httpClient, ScriptSession pythonSession)
        {
            // uri
            Uri requestUri = new Uri($"{httpClient.BaseAddress}{ScriptPath}/{pythonSession.Id}");

            // request
            try
            {
                httpClient.Delete(requestUri, ScriptHostResult.ParseEndSessionResult);
            }
            catch (EmptyHttpResponseException)
            {
                // response of delete should be empty, so catch this
            }
        }

        /// <summary>
        /// Execute scripts which are definied for this session.
        /// </summary>
        /// <param name="pythonSession">Session to execute</param>
        /// <param name="scripts">Scripts to execute</param>
        /// <returns>List of execution results</returns>
        private IEnumerable<ScriptExecutionResult> ExecuteScripts(HttpClient httpClient, ScriptSession pythonSession, IEnumerable<Script> scripts, bool inSequence)
        {
            // content
            StringContent content = new StringContent(JsonConvert.SerializeObject(scripts), RequestEncoding, RequestMediaType);

            // uri
            Uri requestUri = new Uri($"{httpClient.BaseAddress}{ScriptPath}/{pythonSession.Id}?combined={inSequence.ToString().ToLower()}");

            // request
            return httpClient.Put(requestUri, content, ScriptHostResult.ParseScriptExecutionResult);
        }

        #endregion
    }
}
