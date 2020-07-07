namespace Rcv.ScriptHost.Contracts.Models
{
    /// <summary>
    /// Script Parameter structure. Only key-value-pair.
    /// </summary>
    public class ScriptParameter
    {
        /// <summary>
        /// Name of parameter. When using parameters by name
        /// this name is rendered to command line. Otherwise
        /// it's only for diversation of parameters.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Vaule of parameter. Parameter is string based
        /// because it's rendered as it is onto command line.
        /// </summary>
        public string Value { get; set; }
    }
}
