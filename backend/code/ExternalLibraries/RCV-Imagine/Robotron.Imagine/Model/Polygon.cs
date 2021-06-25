using System.Collections.Generic;

namespace Robotron.Imagine.Model
{
    /// <summary>
    /// Defines the outline of the polygon.
    /// </summary>
    public class Polygon
    {
        /// <summary>
        /// List of points which define polygon outline.
        /// </summary>
        public IEnumerable<Point> Points { get; set; }
    }
}
