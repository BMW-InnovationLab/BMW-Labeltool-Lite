using Robotron.Imagine.Util;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Numerics;

namespace Robotron.Imagine.Model
{
    /// <summary>
    /// Drawable line on image.
    /// </summary>
    public class DrawableLine : Line, IDrawable
    {
        /// <summary>
        /// Color of the line.
        /// </summary>
        public string ColorCode { get; set; }

        /// <summary>
        /// Thickness of border in pixels.
        /// Middle of border will be at given coordinates!
        /// Border is painted in every direction.
        /// </summary>
        public ushort Thickness { get; set; } = 5;

        /// <inheritdoc />
        public void Draw<TPixel>(Image<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
        {
            #region validation

            if (image is null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            #endregion

            Rgba32 rectColor = Rgba32Util.InitFromHex(ColorCode);
            SolidBrush brush = new SolidBrush(rectColor);

            image.Mutate(x => x.DrawLines(brush, Thickness, new PointF[] {
                        new Vector2(X1, Y1),
                        new Vector2(X2, Y2)
                    }));
        }
    }
}
