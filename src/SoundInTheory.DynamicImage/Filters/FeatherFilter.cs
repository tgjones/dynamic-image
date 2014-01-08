using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using SoundInTheory.DynamicImage.ShaderEffects;
using SoundInTheory.DynamicImage.Util;
using SWMColors = System.Windows.Media.Colors;

namespace SoundInTheory.DynamicImage.Filters
{
	/// <summary>
	/// 
	/// </summary>
	public class FeatherFilter : ShaderEffectFilter
	{
		#region Properties

		/// <summary>
		/// Gets or sets the feather radius. Values range from 0 to 100. Defaults to 5.
		/// </summary>
		public int Radius
		{
			get { return (int) (this["Radius"] ?? 5); }
			set
			{
				if (value < 0 || value > 100)
					throw new ArgumentOutOfRangeException("value", "Feather radius values must range from 0 to 100.");
				this["Radius"] = value;
			}
		}

		/// <summary>
		/// Gets or sets the shape to use for feathering. Defaults to Rectangle.
		/// </summary>
		public FeatherShape Shape
		{
			get { return (FeatherShape)(this["Shape"] ?? FeatherShape.Rectangle); }
			set { this["Shape"] = value; }
		}

		#endregion

		#region Methods

		protected override bool GetDestinationDimensions(FastBitmap source, out int width, out int height)
		{
			width = source.Width;
			height = source.Height;
			return true;
		}

        protected override Effect GetEffect(FastBitmap source)
		{
			// Fill temporary graphics buffer with mask (currently always a rectangle).
			DrawingVisual dv = new DrawingVisual
			{
				Effect = new BlurEffect
				{
					Radius = Radius,
					KernelType = KernelType.Gaussian
				}
			};
			DrawingContext dc = dv.RenderOpen();
			dc.DrawRectangle(new SolidColorBrush(SWMColors.Transparent), null, new Rect(0, 0, source.Width, source.Height));

			switch (Shape)
			{
				case FeatherShape.Rectangle :
					dc.DrawRectangle(new SolidColorBrush(SWMColors.White), null, new Rect(Radius, Radius, source.Width - Radius * 2, source.Height - Radius * 2));
					break;
				case FeatherShape.Oval :
					dc.DrawEllipse(new SolidColorBrush(SWMColors.White), null, new Point(source.Width / 2.0, source.Height / 2.0), source.Width / 2 - Radius * 2, source.Height / 2 - Radius * 2);
					break;
				default :
					throw new NotSupportedException();
			}
			dc.Close();

			RenderTargetBitmap rtb = RenderTargetBitmapUtility.CreateRenderTargetBitmap(source.Width, source.Height);
			rtb.Render(dv);

			Brush alphaMask = new ImageBrush(rtb);

			return new FeatherEffect
			{
				AlphaMask = alphaMask
			};
		}

		#endregion
	}

	public enum FeatherShape
	{
		Rectangle,
		Oval
	}
}
