namespace Robotron.Imagine.Model
{
    /// <summary>
    /// Strucutre of cuboid object.
    /// </summary>
    public class Cuboid : Rect
    {
        /// <summary>
        /// X-Coordinate of starting point of background rectangle.
        /// </summary>
        public int XBackground { get; set; }

        /// <summary>
        /// Y-Coordinate of starting point of background rectangle.
        /// </summary>
        public int YBackground { get; set; }

        /// <summary>
        /// Width of background rectangle.
        /// </summary>
        public int WidthBackground { get; set; }

        /// <summary>
        /// Height of background rectangle.
        /// </summary>
        public int HeightBackground { get; set; }
    }
}
