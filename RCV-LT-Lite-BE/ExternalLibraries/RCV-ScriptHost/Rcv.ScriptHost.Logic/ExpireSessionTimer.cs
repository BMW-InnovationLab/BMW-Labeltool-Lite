using Rcv.ScriptHost.Contracts.Models;
using System;
using System.IO;

namespace Rcv.ScriptHost.Logic
{
    /// <summary>
    /// Timer for websession of rcv scripthost.
    /// </summary>
    public class ExpireSessionTimer : System.Timers.Timer
    {
        #region member

        /// <summary>
        /// Path to parameters directory.
        /// </summary>
        private string ParametersDirectory { get; set; }

        /// <summary>
        /// Path to result directory.
        /// </summary>
        private string ResultsDirectory { get; set; }

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new instance of session timer.
        /// </summary>
        /// <param name="pythonSession">Python session to terminate</param>
        /// <param name="interval">Interval when session should be end</param>
        public ExpireSessionTimer(ScriptSession pythonSession, double interval) : base(interval)
        {
            #region validation

            if (pythonSession == null)
            {
                throw new ArgumentNullException(nameof(pythonSession));
            }

            #endregion

            this.ParametersDirectory = pythonSession.ParametersDirectory;
            this.ResultsDirectory = pythonSession.ResultDirectory;

            this.Elapsed += ExpireSessionTimerElapsed;
        }

        #endregion

        #region Timer

        public new void Stop()
        {
            base.Stop();
            this.CleanUp();
        }

        #endregion

        #region private methods

        private void ExpireSessionTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.CleanUp();
        }

        private void CleanUp()
        {
            DirectoryInfo direcoryInfoParameters = new DirectoryInfo(ParametersDirectory);
            if (direcoryInfoParameters.Exists)
            {
                direcoryInfoParameters.Delete(true);
            }

            DirectoryInfo directoryInfoResults = new DirectoryInfo(ResultsDirectory);
            if (directoryInfoResults.Exists)
            {
                directoryInfoResults.Delete(true);
            }
        }

        #endregion
    }
}
