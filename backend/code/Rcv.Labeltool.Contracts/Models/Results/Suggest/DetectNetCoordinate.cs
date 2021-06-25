using System.Text.Json.Serialization;

namespace Rcv.LabelTool.Contracts.Models.Results.Suggest
{
    /// <summary>
    /// Coordinates of DetectNet bounding box.
    /// </summary>
    public class DetectNetCoordinate
    {
        /// <summary>
        /// Left coordinate of bounding box.
        /// </summary>
        [JsonPropertyName("left")]
        public int Left { get; set; }

        /// <summary>
        /// Top coordinate of bounding box.
        /// </summary>
        [JsonPropertyName("top")]
        public int Top { get; set; }

        /// <summary>
        /// Right coordinate of bounding box.
        /// </summary>
        [JsonPropertyName("right")]
        public int Right { get; set; }

        /// <summary>
        /// Bottom coordinate of bounding box.
        /// </summary>
        [JsonPropertyName("bottom")]
        public int Bottom { get; set; }
    }
}