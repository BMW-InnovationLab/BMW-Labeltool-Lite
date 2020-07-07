using System.ComponentModel.DataAnnotations;

namespace Rcv.LabelTool.Contracts.Models
{
    /// <summary>
    /// Datacontract for topic.
    /// </summary>
    public class Topic
    {
        /// <summary>
        /// Id of topic.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Name of topic. Equals folder name in file system.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Path to folder in data structure.
        /// </summary>
        public string FolderPath { get; set; }

        /// <summary>
        /// Virtual path of topic used in frontend.
        /// </summary>
        public string Path { get; set; }
    }
}
