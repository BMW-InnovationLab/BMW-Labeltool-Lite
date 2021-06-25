namespace Robotron.Imagine.Model
{
    /// <summary>
    /// Cuboid which can be drawn to any image.
    /// </summary>
    public class DrawableCuboid : Cuboid
    {
        /// <summary>
        /// Color of border of cuboid.
        /// </summary>
        public string BorderColorCode { get; set; }

        /// <summary>
        /// Thickness of border in pixels.
        /// Middle of border will be at given coordinates!
        /// Border is painted in every direction.
        /// </summary>
        public ushort BorderThickness { get; set; } = 5;
    }
}
