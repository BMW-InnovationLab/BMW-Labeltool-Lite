namespace Rcv.ScriptHost.Contracts.Container
{
    /// <summary>
    /// Interface definition for file container factory.
    /// Factory will provide several file containers depending on
    /// their configuration.
    /// </summary>
    public interface IFileContainerFactory
    {
        /// <summary>
        /// File container where parameter files are stored.
        /// </summary>
        IFileContainer ParametersFileContainer { get; }

        /// <summary>
        /// File container where result files are stored.
        /// </summary>
        IFileContainer ResultsFileContainer { get; }

        /// <summary>
        /// File container where script files are stored.
        /// </summary>
        IFileContainer ScriptsFileContainer { get; }
    }
}
