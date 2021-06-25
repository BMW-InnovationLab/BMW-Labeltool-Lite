using Robotron.Imagine.Util;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;

namespace Robotron.Imagine.Model
{
    /// <summary>
    /// Drawable circle.
    /// </summary>
    public class DrawableCircle : Circle, IDrawable
    {
        /// <summary>
        /// Color of border of circle.
        /// </summary>
        public string BorderColorCode { get; set; }

        /// <summary>
        /// Thickness of border in pixels.
        /// Middle of border will be at given coordinates!
        /// Border is painted in every direction.
        /// </summary>
        public ushort BorderThickness { get; set; } = 5;

        /// <summary>
        /// Color of filling of circle.
        /// </summary>
        public string FillColorCode { get; set; } = string.Empty;

        /// <inheritdoc />
        public void Draw<TPixel>(Image<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
        {
            #region validation

            if (image is null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            #endregion

            EllipsePolygon ellipsePolygon = new EllipsePolygon(new PointF(X, Y), Radius);

            if (!string.IsNullOrEmpty(BorderColorCode))
            {
                Rgba32 borderColor = Rgba32Util.InitFromHex(BorderColorCode);
                SolidBrush borderBrush = new SolidBrush(borderColor);
                Pen pen = new Pen(borderColor, BorderThickness);

                image.Mutate(x => x.Draw(pen, ellipsePolygon));
            }
            if (!string.IsNullOrEmpty(FillColorCode))
            {
                Rgba32 fillColor = Rgba32Util.InitFromHex(FillColorCode);
                SolidBrush fillBrush = new SolidBrush(fillColor);

                image.Mutate(x => x.Fill(fillBrush, ellipsePolygon));
            }
        }
    }
}
