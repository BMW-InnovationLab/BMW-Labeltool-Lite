using System.ComponentModel.DataAnnotations;

namespace Rcv.LabelTool.Contracts.Models.View
{
    /// <summary>
    /// Datacontract for topic.
    /// </summary>
    public class TopicView
    {
        /// <summary>
        /// Id of topic.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Name of topic. Equals folder name in file system.
        /// </summary>
        [Required]
        [RegularExpression(Topic.REGEX_NAME)]
        public string Name { get; set; }

        /// <summary>
        /// Virtual path of topic used in frontend.
        /// </summary>
        public string Path { get; set; }
    }
}
