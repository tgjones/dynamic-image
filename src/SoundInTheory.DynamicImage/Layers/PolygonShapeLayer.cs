using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SoundInTheory.DynamicImage.Util;

namespace SoundInTheory.DynamicImage.Layers
{
	public class PolygonShapeLayer : ClosedShapeLayer
	{
		public int Sides
		{
			get { return (int) (this["Sides"] ?? 3); }
			set { this["Sides"] = value; }
		}

        protected sealed override void CreateImage()
		{
			base.CreateImage();

			Rect bounds = new Rect(StrokeWidth / 2, StrokeWidth / 2,
				CalculatedWidth - StrokeWidth,
				CalculatedHeight - StrokeWidth);
			Brush brush = Fill.GetBrush();

			PointCollection points = GetPoints(bounds);
			PathGeometry geometry = CanonicalSplineHelper.CreateSpline(points, Roundness / 100.0, null, true, true, 0.25);

			Pen pen = GetPen();

			DrawingVisual dv = new DrawingVisual();
			DrawingContext dc = dv.RenderOpen();

			// Draw polygon.
			dc.DrawGeometry(brush, pen, geometry);

			dc.Close();

			RenderTargetBitmap rtb = RenderTargetBitmapUtility.CreateRenderTargetBitmap(CalculatedWidth, CalculatedHeight);
			rtb.Render(dv);
			Bitmap = new FastBitmap(rtb);
		}

		protected PointCollection GetPoints(Rect bounds)
		{
			// Inscribe the shape inside a circle
			double angle = -(Math.PI / 2.0f);
			double deltaAngle = (Math.PI * 2) / Sides;

			double halfWidth = bounds.Width / 2;
			double halfHeight = bounds.Height / 2;

			PointCollection points = new PointCollection(Sides);
			for (int i = 0; i < this.Sides; i++)
			{
				// Convert from polar to cartesian coordinates.
				double x = halfWidth * Math.Cos(angle);
				double y = halfHeight * Math.Sin(angle);

				points.Add(new Point(
					x + halfWidth + bounds.Left,
					y + halfHeight + bounds.Top));

				angle += deltaAngle;
			}

			return points;
		}
	}
}
