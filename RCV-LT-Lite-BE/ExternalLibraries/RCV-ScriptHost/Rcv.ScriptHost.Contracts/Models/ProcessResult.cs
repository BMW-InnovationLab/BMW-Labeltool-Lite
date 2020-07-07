namespace Rcv.ScriptHost.Contracts.Models
{
    /// <summary>
    /// Result of process which was runned on commandline.
    /// </summary>
    public class ProcessResult
    {
        /// <summary>
        /// Return code of process.
        /// </summary>
        public int ReturnCode { get; set; }

        /// <summary>
        /// Information which have been written to standard output during
        /// process execution.
        /// </summary>
        public string ProcessInformation { get; set; }

        /// <summary>
        /// Error information which have been written to standard error during
        /// process execution.
        /// </summary>
        public string ProcessError { get; set; }

        /// <summary>
        /// Timespan of process run in milliseconds.
        /// </summary>
        public int ProcessTime { get; set; }
    }
}
