using System.Collections.Generic;

namespace Robotron.Imagine.Transformation
{
    /// <summary>
    /// Tranformation parameter for pipelined transformation process.
    /// Parameter will be stored in lists in order of execution.
    /// </summary>
    public class TransformationParameters
    {
        /// <summary>
        /// List of scale parameters.
        /// </summary>
        public IList<Scale> ScaleParameters { get; set; } = new List<Scale>();

        /// <summary>
        /// List of fill parameters.
        /// </summary>
        public IList<Fill> FillParameters { get; set; } = new List<Fill>();

    }
}
