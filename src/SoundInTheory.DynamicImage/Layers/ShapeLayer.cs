using System;
using System.Windows.Media;
using SoundInTheory.DynamicImage.Util;

namespace SoundInTheory.DynamicImage.Layers
{
	public abstract class ShapeLayer : Layer
	{
		#region Properties

		/// <summary>
		/// Width of the shape layer.
		/// </summary>
		public int? Width
		{
			get { return this["Width"] as int?; }
			set { this["Width"] = value; }
		}

		/// <summary>
		/// Height of the shape layer.
		/// </summary>
		public int? Height
		{
			get { return this["Height"] as int?; }
			set { this["Height"] = value; }
		}

		public DashStyle StrokeDashStyle
		{
			get { return (DashStyle)(this["StrokeDashStyle"] ?? DashStyle.Solid); }
			set { this["StrokeDashStyle"] = value; }
		}

		public Fill StrokeFill
		{
			get { return (Fill)(this["StrokeFill"] ?? (this["StrokeFill"] = new Fill())); }
		}

		public float StrokeWidth
		{
			get { return (float)(this["StrokeWidth"] ?? 0.0f); }
			set { this["StrokeWidth"] = value; }
		}

		public int Roundness
		{
			get { return (int)(this["Roundness"] ?? 0); }
			set { this["Roundness"] = value; }
		}

		public override bool HasFixedSize
		{
			get { return (Width != null && Height != null); }
		}

		#endregion

        protected override void CreateImage()
		{
			if (this.Width != null)
				this.CalculatedWidth = this.Width.Value;

			if (this.Height != null)
				this.CalculatedHeight = this.Height.Value;
		}

		protected Pen GetPen()
		{
			Pen pen = null;
			if (StrokeWidth > 0)
				pen = new Pen(StrokeFill.GetBrush(), StrokeWidth)
				{
					DashStyle = GetDashStyle()
				};
			return pen;
		}

		private System.Windows.Media.DashStyle GetDashStyle()
		{
			switch (StrokeDashStyle)
			{
				case DashStyle.Solid :
					return DashStyles.Solid;
				case DashStyle.Dash :
					return DashStyles.Dash;
				case DashStyle.Dot :
					return DashStyles.Dot;
				case DashStyle.DashDot :
					return DashStyles.DashDot;
				case DashStyle.DashDotDot :
					return DashStyles.DashDotDot;
				default :
					throw new NotSupportedException();
			}
		}
	}
}
