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
    }
}
