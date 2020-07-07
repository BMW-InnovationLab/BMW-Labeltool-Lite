using System;

namespace Rcv.ScriptHost.Contracts.Models
{
    /// <summary>
    /// PythonSession which describes session which is created
    /// at server level for client only.
    /// </summary>
    public class ScriptSession
    {
        /// <summary>
        /// ID of this session. Used to identify session 
        /// in communcation.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Expire date of session. When this Date is reached,
        /// all artifacts of session will be deleted automatically.
        /// Only for sessions which are not disposed.
        /// </summary>
        public DateTime ExpireDate { get; set; }

        /// <summary>
        /// Directory where host can transfers files to
        /// modify by session. Directory should be writable
        /// from client.
        /// </summary>
        public string ParametersDirectory { get; set; }

        /// <summary>
        /// Directory where python execution can drop
        /// result files. Client can transfers files from 
        /// her. Directory should be writable from pyhton
        /// host and readable from client.
        /// </summary>
        public string ResultDirectory { get; set; }
    }
}
