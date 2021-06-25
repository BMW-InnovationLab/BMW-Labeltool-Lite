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
    /// Rect which can be drawed to any image.
    /// </summary>
    public class DrawableRect : Rect, IDrawable
    {
        /// <summary>
        /// Color of border of rect.
        /// </summary>
        public string BorderColorCode { get; set; }

        /// <summary>
        /// Thickness of border in pixels.
        /// Middle of border will be at given coordinates!
        /// Border is painted in every direction.
        /// </summary>
        public ushort BorderThickness { get; set; } = 5;

        /// <summary>
        /// Color of filling of rect.
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

            var rectCoordinates = new PointF[] {
                        new Vector2(X, Y),
                        new Vector2(X + Width, Y),
                        new Vector2(X + Width, Y + Height),
                        new Vector2(X, Y + Height)
                    };

            // draw filled rectangle
            if (!string.IsNullOrEmpty(FillColorCode))
            {
                Rgba32 fillColor = Rgba32Util.InitFromHex(FillColorCode);
                SolidBrush fillBrush = new SolidBrush(fillColor);

                image.Mutate(x => x.FillPolygon(fillBrush, rectCoordinates));
            }

            // draw border of rectangle
            if (!string.IsNullOrEmpty(BorderColorCode) && BorderThickness > 0)
            {
                Rgba32 borderColor = Rgba32Util.InitFromHex(BorderColorCode);
                SolidBrush borderBrush = new SolidBrush(borderColor);

                image.Mutate(x => x.DrawPolygon(borderBrush, BorderThickness, rectCoordinates));
            }
        }
    }
}
