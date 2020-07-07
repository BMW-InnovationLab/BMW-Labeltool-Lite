using System.Collections.Generic;

namespace Rcv.ScriptHost.Contracts.Models
{
    /// <summary>
    /// Script which is reachble from pyhton host. 
    /// Only metadata will be shared with this class.
    /// </summary>
    public class Script
    {
        /// <summary>
        /// Name of script. Equal to filename in filesystem.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Path of subfolders in filesystem.
        /// </summary>
        public string SubFolder { get; set; }

        /// <summary>
        /// Does this script support named parameters?
        /// TRUE: Named parameters are supported. All parameters
        /// will be used by name
        /// FALSE: Named parameters are not supported. Parameters
        /// will be used by position
        /// </summary>
        public bool NamedParameters { get; set; }

        /// <summary>
        /// List of parameters to run this script.
        /// </summary>
        public IEnumerable<ScriptParameter> Parameters { get; set; }
    }
}
