using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SoundInTheory.DynamicImage.Util;

namespace SoundInTheory.DynamicImage.Layers
{
	/// <summary>
	/// Represents a rectangle shape layer.
	/// </summary>
	public class RectangleShapeLayer : ClosedShapeLayer
	{
        protected override sealed void CreateImage()
		{
			base.CreateImage();

			Rect bounds = new Rect(StrokeWidth / 2, StrokeWidth / 2,
				CalculatedWidth - StrokeWidth,
				CalculatedHeight - StrokeWidth);
			Brush brush = Fill.GetBrush();

			Pen pen = GetPen();

			DrawingVisual dv = new DrawingVisual();
			DrawingContext dc = dv.RenderOpen();

			if (Roundness == 0)
				dc.DrawRectangle(brush, pen, bounds);
			else
				dc.DrawRoundedRectangle(brush, pen, bounds, Roundness, Roundness);

			dc.Close();

			RenderTargetBitmap rtb = RenderTargetBitmapUtility.CreateRenderTargetBitmap(CalculatedWidth, CalculatedHeight);
			rtb.Render(dv);
			Bitmap = new FastBitmap(rtb);
		}
	}
}