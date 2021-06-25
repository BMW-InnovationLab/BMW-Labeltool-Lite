namespace Rcv.LabelTool.Contracts.Configurations
{
    /// <summary>
    /// Data contract definition of segmentation configuration.
    /// </summary>
    public interface IInferenceConfiguration
    {
        /// <summary>
        /// Uri of inference container.
        /// </summary>
        public string InferenceContainerUri { get; set; }
    }
}
