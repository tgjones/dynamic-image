using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SoundInTheory.DynamicImage.Util;

namespace SoundInTheory.DynamicImage.Layers
{
	public class TextLayer : Layer
	{
		#region Properties

		/// <summary>
		/// Width of the text layer; if omitted, size will be calculated automatically 
		/// and all text will be rendered on a single line.
		/// </summary>
		public int? Width
		{
			get { return (int?) this["Width"]; }
			set { this["Width"] = value; }
		}

		/// <summary>
		/// Height of the text layer; if both Width and Height are omitted,
		/// size will be calculated automatically and all text will be rendered
		/// on a single line; if just Height is omitted then text will be 
		/// wrapped based on the Width.
		/// </summary>
		public int? Height
		{
			get { return (int?) this["Height"]; }
			set { this["Height"] = value; }
		}

		public bool Multiline
		{
			get { return (bool)(this["Multiline"] ?? false); }
			set { this["Multiline"] = value; }
		}

		public Font Font
		{
			get { return (Font)(this["Font"] ?? (this["Font"] = new Font())); }
			set { this["Font"] = value; }
		}

		public Color ForeColor
		{
			get { return (Color)(this["ForeColor"] ?? Colors.Black); }
			set { this["ForeColor"] = value; }
		}

		public Color? StrokeColor
		{
			get { return (Color?) this["StrokeColor"]; }
			set { this["StrokeColor"] = value; }
		}

		public double StrokeWidth
		{
			get { return (double)(this["StrokeWidth"] ?? 0.0); }
			set { this["StrokeWidth"] = value; }
		}

		public string Text
		{
			get { return (string)(this["Text"] ?? string.Empty); }
			set { this["Text"] = value; }
		}

		public TextAlignment HorizontalTextAlignment
		{
			get { return (TextAlignment)(this["HorizontalTextAlignment"] ?? TextAlignment.Left); }
			set { this["HorizontalTextAlignment"] = value; }
		}

		public VerticalAlignment VerticalTextAlignment
		{
			get { return (VerticalAlignment)(this["VerticalTextAlignment"] ?? VerticalAlignment.Top); }
			set { this["VerticalTextAlignment"] = value; }
		}

		public override bool HasFixedSize
		{
			get { return true; }
		}

		#endregion

        protected override void CreateImage()
		{
			// If width and height are not set, we need to measure the string.
			int calculatedWidth, calculatedHeight;
			Size measuredSize = MeasureString();
			if (Width == null || Height == null)
			{
				double width = Width ?? measuredSize.Width;
				double height = Height ?? measuredSize.Height;
				calculatedWidth = (int) width;
				calculatedHeight = (int)height;
			}
			else // otherwise just create the image at the desired size
			{
				calculatedWidth = Width.Value;
				calculatedHeight = Height.Value;
			}

			#region Draw text

			DrawingVisual dv = new DrawingVisual();
			DrawingContext dc = dv.RenderOpen();

			//RenderOptions.SetClearTypeHint(dv, ClearTypeHint.Auto);
			TextOptions.SetTextRenderingMode(dv, TextRenderingMode.Auto);
			//TextOptions.SetTextFormattingMode(dv, TextFormattingMode.Ideal)

			UseFormattedText(ft =>
			{
				Pen pen = null;
				if (StrokeWidth > 0 && StrokeColor != null)
					pen = new Pen(new SolidColorBrush(StrokeColor.Value.ToWpfColor()), StrokeWidth);

				// Calculate position to draw text at, based on vertical text alignment.
				int x = CalculateHorizontalPosition((int) measuredSize.Width);
				int y = CalculateVerticalPosition((int) measuredSize.Height);

				dc.DrawGeometry(new SolidColorBrush(ForeColor.ToWpfColor()), pen,
					ft.BuildGeometry(new Point(x, y)));
			});

			dc.Close();

			RenderTargetBitmap rtb = RenderTargetBitmapUtility.CreateRenderTargetBitmap(calculatedWidth, calculatedHeight);
			rtb.Render(dv);

			#endregion

			Bitmap = new FastBitmap(rtb);
		}

		private int CalculateHorizontalPosition(int measuredWidth)
		{
			switch (HorizontalTextAlignment)
			{
				case TextAlignment.Left:
				case TextAlignment.Justify :
					return 0;
				case TextAlignment.Right:
					if (Width != null)
						return Width.Value - measuredWidth;
					return 0;
				case TextAlignment.Center :
					if (Width != null)
						return (Width.Value - measuredWidth) / 2;
					return 0;
				default:
					throw new NotSupportedException();
			}
		}

		private int CalculateVerticalPosition(int measuredHeight)
		{
			switch (VerticalTextAlignment)
			{
				case VerticalAlignment.Top :
					return 0;
				case VerticalAlignment.Bottom :
					if (Height != null)
						return Height.Value - measuredHeight;
					return 0;
				case VerticalAlignment.Center :
					if (Height != null)
						return (Height.Value - measuredHeight) / 2;
					return 0;
				default :
					throw new NotSupportedException();
			}
		}

        private Size MeasureString()
		{
			Size size = System.Windows.Size.Empty;
			UseFormattedText(ft =>
			{
				size = new Size(ft.WidthIncludingTrailingWhitespace, ft.Height);
			});
			return size;
		}

        private void UseFormattedText(RenderCallback renderCallback)
		{
			Brush textBrush = new SolidColorBrush(ForeColor.ToWpfColor());
			FontDescription fontDescription = Font.GetFontDescription();
			var formattedText = new FormattedText(
				Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
				fontDescription.Typeface, fontDescription.Size, textBrush);
			formattedText.SetTextDecorations(fontDescription.TextDecorations);
			if (Width != null)
				formattedText.MaxTextWidth = Width.Value;
			if (Height != null)
				formattedText.MaxTextHeight = Height.Value;
			if (!Multiline)
				formattedText.MaxLineCount = 1;
			formattedText.Trimming = TextTrimming.None;
			//formattedText.TextAlignment = HorizontalTextAlignment;

			renderCallback(formattedText);
		}

		private delegate void RenderCallback(FormattedText formattedText);
	}
}
