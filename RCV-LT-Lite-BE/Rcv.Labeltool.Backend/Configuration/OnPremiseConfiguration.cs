namespace Rcv.LabelTool.Backend.Configuration
{
    /// <summary>
    /// Configuration for on premise file system locations where data
    /// of labeltool is stored.
    /// </summary>
    public class OnPremiseConfiguration
    {
        /// <summary>
        /// File system path of training data.
        /// </summary>
        public string TrainingDataDirectoryPath { get; set; }

        /// <summary>
        /// File system path of export data.
        /// </summary>
        public string ExportDataDirectoryPath { get; set; }

        /// <summary>
        /// File system path of share data.
        /// </summary>
        public string ShareDataDirectoryPath { get; set; }

        /// <summary>
        /// File system path where workbench of script execution
        /// is hosted.
        /// </summary>
        public string ScriptWorkbenchDirectoryPath { get; set; }
    }
}
