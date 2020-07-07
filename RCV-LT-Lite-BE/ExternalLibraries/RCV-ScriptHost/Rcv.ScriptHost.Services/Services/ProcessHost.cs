using Rcv.ScriptHost.Contracts.Models;
using Rcv.ScriptHost.Contracts.Services;
using Serilog;
using System;
using System.Diagnostics;

namespace Rcv.ScriptHost.Services.Services
{
    public class ProcessHost : IProcessHost
    {
        #region member

        private ISettingsProvider SettingsProvider { get; set; }

        #endregion

        #region constructor

        public ProcessHost(ISettingsProvider settingsProvider)
        {
            SettingsProvider = settingsProvider ?? throw new ArgumentNullException(nameof(settingsProvider));
        }

        #endregion

        #region IProcessHost

        public ProcessResult RunProcess(string scriptCommand)
        {
            Log.Debug($"Run script '{SettingsProvider.HostExecutable}' at '{SettingsProvider.ResultDirectoryPath}' with command '{scriptCommand}'");

            string errorOutput = string.Empty;
            string standardOutput = string.Empty;

            Process process = new Process();
            try
            {
                process.StartInfo.WorkingDirectory = SettingsProvider.ResultDirectoryPath;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = SettingsProvider.HostExecutable;
                process.StartInfo.Arguments = scriptCommand;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.Start();

                standardOutput = process.StandardOutput.ReadToEnd();
                errorOutput = process.StandardError.ReadToEnd();

                if (SettingsProvider.ProcessTimeout.HasValue)
                {
                    Log.Information($"Wait {SettingsProvider.ProcessTimeout.Value}ms for process to end");
                    process.WaitForExit(SettingsProvider.ProcessTimeout.Value);
                }
                else
                {
                    Log.Information($"Wait for process to end");
                    process.WaitForExit();
                }

                Log.Debug($"Process successfully ended!");
                Log.Debug($"Information log of process: {standardOutput}");

                if (!string.IsNullOrEmpty(errorOutput))
                {
                    Log.Error($"Process fails with message: {errorOutput}");
                }
            }
            catch (Exception exc)
            {
                Log.Fatal(exc, $"Process execution has ended undefined. Script '{scriptCommand}' has not been executed!");
            }

            ProcessResult processResult = new ProcessResult()
            {
                ReturnCode = process.ExitCode,
                ProcessInformation = standardOutput,
                ProcessError = errorOutput,
                ProcessTime = (int)(process.ExitTime - process.StartTime).TotalMilliseconds
            };

            return processResult;
        }

        #endregion
    }
}
