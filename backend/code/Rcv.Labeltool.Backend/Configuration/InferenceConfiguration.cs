using Rcv.LabelTool.Contracts.Configurations;

namespace Rcv.LabelTool.Backend.Configuration
{
    /// <summary>
    /// Configuration of interence functionality of labeltool backend.
    /// </summary>
    public class InferenceConfiguration : IInferenceConfiguration
    {
        /// <summary>
        /// Uri of inference container.
        /// </summary>
        public string InferenceContainerUri { get; set; }
    }
}
