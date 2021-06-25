using System.Text.Json.Serialization;

namespace Rcv.LabelTool.Contracts.Models.Results.Suggest
{
    /// <summary>
    /// Bounding box of DetectNet object detection.
    /// </summary>
    public class DetectNetBoundingBox
    {
        /// <summary>
        /// Objectclass id of DetectNet bounding box.
        /// </summary>
        [JsonPropertyName("ObjectClassId")]
        public uint ObjectClassId { get; set; }

        /// <summary>
        /// Objectclass name of DetectNet bounding box.
        /// </summary>
        [JsonPropertyName("ObjectClassName")]
        public string ObjectClassName { get; set; }

        /// <summary>
        /// Confidence of DetectNet bounding box.
        /// </summary>
        [JsonPropertyName("confidence")]
        public double Confidence { get; set; }

        /// <summary>
        /// Coordinates of DetectNet bounding box.
        /// </summary>
        [JsonPropertyName("coordinates")]
        public DetectNetCoordinate Coordinates { get; set; }
    }
}
