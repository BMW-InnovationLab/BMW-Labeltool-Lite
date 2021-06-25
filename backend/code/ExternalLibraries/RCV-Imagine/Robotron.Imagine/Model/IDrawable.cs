using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Robotron.Imagine.Model
{
    /// <summary>
    /// Interface for drawable objects.
    /// </summary>
    public interface IDrawable
    {
        /// <summary>
        /// Draw current object to image.
        /// </summary>
        /// <param name="image">Image to draw current object on</param>
        void Draw<TPixel>(Image<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>;
    }
}
