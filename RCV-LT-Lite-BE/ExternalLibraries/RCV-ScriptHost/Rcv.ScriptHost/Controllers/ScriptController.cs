using Microsoft.AspNetCore.Mvc;
using Rcv.Base.WebApi.Controller;
using Rcv.ScriptHost.Contracts.Models;
using Rcv.ScriptHost.Contracts.Services;
using Rcv.ScriptHost.Logic;
using Rcv.ScriptHost.Logic.Extensions;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Robotron.PythonHost.Controllers
{
    /// <summary>
    /// Controller for handling script execution.
    /// </summary>
    [Route("api/[controller]")]
    public class ScriptController : AbstractController
    {
        #region member

        private ISettingsProvider SettingsProvider { get; set; }

        private IProcessHost ProcessHost { get; set; }

        private ISessionService SessionService { get; set; }

        #endregion

        #region static member

        private static Dictionary<string, ExpireSessionTimer> Sessions { get; set; } = new Dictionary<string, ExpireSessionTimer>();

        #endregion

        #region constructor

        public ScriptController(ISettingsProvider settingsProvider, ISessionService sessionService, IProcessHost processHost)
        {
            SettingsProvider = settingsProvider ?? throw new ArgumentNullException(nameof(settingsProvider));
            ProcessHost = processHost ?? throw new ArgumentNullException(nameof(processHost));
            SessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        }

        #endregion

        #region ScriptController

        /// <summary>
        /// Creates a new session for run scripts.
        /// </summary>
        /// <returns>Resultobject of contoller</returns>
        /// <response code="200">Newly created python session</response>
        /// <response code="500">Unexpected error occures</response>
        [HttpGet]
        public ActionResult<ScriptSession> Get()
        {
            return Execute(() =>
            {
                ScriptSession pythonSession = SessionService.CreateSession();

                ExpireSessionTimer sessionTimer = new ExpireSessionTimer(pythonSession, SettingsProvider.SessionLifetime);
                Sessions.Add(pythonSession.Id, sessionTimer);
                sessionTimer.Start();

                Log.Information($"Request new session DONE. Created session with ID '{pythonSession.Id}' and expire date '{pythonSession.ExpireDate.ToLongTimeString()}'");

                return Ok(pythonSession);
            });
        }

        /// <summary>
        /// Run scripts in definied session.
        /// </summary>
        /// <param name="sessionId">ID of session where to run scripts</param>
        /// <param name="scripts">Scripts to run</param>
        /// <param name="combined">Scripts should be runned combined</param>
        /// <returns>Resultobject of controller</returns>
        /// <response code="200">Scripts successfully runned</response>
        /// <response code="400">Session id and scripts must be provided</response>
        /// <response code="404">No session with given id was found</response>
        /// <response code="500">Unexpected error occures</response>
        [HttpPut]
        [Route("{sessionId}")]
        public ActionResult<IEnumerable<ScriptExecutionResult>> Put(string sessionId, [FromBody] IEnumerable<Script> scripts, [FromQuery] bool combined = false)
        {
            return Execute(() =>
            {
                #region validation

                CheckMandatoryParameter(nameof(sessionId), sessionId);
                CheckMandatoryParameter(nameof(scripts), scripts);

                if (!Sessions.ContainsKey(sessionId))
                {
                    Log.Error($"Session with id '{sessionId}' is unknown!");
                    return NotFound();
                }

                #endregion

                List<ScriptExecutionResult> scriptExecutionResults = new List<ScriptExecutionResult>();

                Log.Debug($"Execute scripts in session '{sessionId}'");
                // run scripts 
                foreach (Script script in scripts)
                {
                    Log.Debug($"Execute script '{script.Name}' (folder: '{script.SubFolder}') in session '{sessionId}'");

                    string parameterPath = GetDefaultParametersPath(sessionId);
                    string resultPath = GetDefaultResultsPath(sessionId);
                    if (combined)
                    {
                        parameterPath = GetParameterStagePath(script, scripts, sessionId);
                        resultPath = GetResultStagePath(script, scripts, sessionId);
                    }

                    Log.Debug($"Process files from '{parameterPath}' into '{resultPath}'");

                    ProcessResult processResult = RunScript(script, parameterPath, resultPath);
                    ScriptExecutionResult scriptExecutionResult = new ScriptExecutionResult()
                    {
                        IsFailed = processResult.ReturnCode < 0,
                        Script = script
                    };
                    scriptExecutionResults.Add(scriptExecutionResult);
                }
                Log.Debug($"All scripts executed in session '{sessionId}'");

                return Ok(scriptExecutionResults);
            });
        }

        /// <summary>
        /// End up session with given session-id.
        /// </summary>
        /// <param name="sessionId">Id of session to end</param>
        /// <returns>Resultobject of controller</returns>
        /// <response code="200">Session has ended successfully</response>
        /// <response code="400">Session id must be provided</response>
        /// <response code="404">No session with given id was found</response>
        /// <response code="500">Unexpected error occures</response>
        [HttpDelete]
        [Route("{sessionId}")]
        public ActionResult Delete(string sessionId)
        {
            return Execute(() =>
            {
                #region validation

                CheckMandatoryParameter(nameof(sessionId), sessionId);

                if (!Sessions.ContainsKey(sessionId))
                {
                    Log.Error($"Session with id '{sessionId}' is unknown!");
                    return NotFound();
                }

                #endregion

                // Stop session timer
                Sessions[sessionId].Stop();

                // Remove session from session list
                Sessions.Remove(sessionId);

                return Ok();
            });
        }

        #endregion

        #region private helper

        private ProcessResult RunScript(Script script, string parameterPath, string resultPath)
        {
            string args = $"{SettingsProvider.ScriptDirectoryPath}{script.Name}";
            args += " " + parameterPath;
            args += " " + resultPath;

            args += script.GetParametersAsArgs();

            return ProcessHost.RunProcess(args);
        }

        private string GetDefaultParametersPath(string sessionId)
        {
            return $"{SettingsProvider.ParametersDirectoryPath}{sessionId}";
        }


        private string GetDefaultResultsPath(string sessionId)
        {
            return $"{SettingsProvider.ResultDirectoryPath}{sessionId}";
        }

        private string GetParameterStagePath(Script script, IEnumerable<Script> scripts, string sessionId)
        {
            int currentScriptIndex = scripts.ToList().IndexOf(script);

            if (currentScriptIndex == 0)
            {
                return GetDefaultParametersPath(sessionId);
            }

            return $"{SettingsProvider.ParametersDirectoryPath}{sessionId}{System.IO.Path.DirectorySeparatorChar}s{currentScriptIndex - 1}";
        }

        private string GetResultStagePath(Script script, IEnumerable<Script> scripts, string sessionId)
        {
            int currentScriptIndex = scripts.ToList().IndexOf(script);

            if (currentScriptIndex == scripts.Count() - 1)
            {
                return GetDefaultResultsPath(sessionId);
            }

            return $"{SettingsProvider.ParametersDirectoryPath}{sessionId}{System.IO.Path.DirectorySeparatorChar}s{currentScriptIndex}";
        }

        #endregion
    }
}
