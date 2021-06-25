using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rcv.LabelTool.Contracts.Models.Results.Suggest
{
    /// <summary>
    /// Response of DetectNet object detection request.
    /// </summary>
    public class DetectNetDetectionResponse
    {
        /// <summary>
        /// Listing of DetectNet bounding boxes.
        /// </summary>
        [JsonPropertyName("bounding-boxes")]
        public IEnumerable<DetectNetBoundingBox> BoundingBoxes { get; set; }
    }
}
