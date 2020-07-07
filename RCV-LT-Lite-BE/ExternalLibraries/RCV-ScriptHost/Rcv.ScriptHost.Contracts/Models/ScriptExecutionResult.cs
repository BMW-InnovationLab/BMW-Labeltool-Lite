namespace Rcv.ScriptHost.Contracts.Models
{
    /// <summary>
    /// Result of script execution.
    /// </summary>
    public class ScriptExecutionResult
    {
        /// <summary>
        /// Script was failed during run.
        /// </summary>
        public bool IsFailed { get; set; }

        /// <summary>
        /// Executed script.
        /// </summary>
        public Script Script { get; set; }
    }
}
