using Rcv.ScriptHost.Contracts.Models;

namespace Rcv.ScriptHost.Logic.Extensions
{
    /// <summary>
    /// Extensions of script class.
    /// </summary>
    public static class ScriptExtensions
    {
        /// <summary>
        /// Get parameters defined in script as command line args.
        /// </summary>
        /// <param name="script">Script to generate command line args from</param>
        /// <returns>Parameters as command line args</returns>
        public static string GetParametersAsArgs(this Script script)
        {
            string args = string.Empty;
            if (script.Parameters != null)
            {
                foreach (ScriptParameter item in script.Parameters)
                {
                    // append script parameter to command line
                    args += script.GetParameterAsArg(item.Name, item.Value);
                }
            }

            return args;
        }

        /// <summary>
        /// Get parameter as command line parameter depending on information out of script.
        /// </summary>
        /// <param name="script">Script to get parameter as command line parameter</param>
        /// <param name="paramName">Name of parameter</param>
        /// <param name="paramValue">Value of parameter</param>
        /// <returns>Parameter as command line parameter</returns>
        public static string GetParameterAsArg(this Script script, string paramName, string paramValue)
        {
            string arg = " ";
            if (script.NamedParameters)
            {
                // if named parameters are supported by this script
                // add parametername with trailing --
                arg += $"--{paramName}=";
            }

            // add param value
            arg += $"{paramValue}";

            return arg;
        }
    }
}
