using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using SoundInTheory.DynamicImage.ShaderEffects;
using SoundInTheory.DynamicImage.Util;

namespace SoundInTheory.DynamicImage.Filters
{
	/// <summary>
	/// Sharpens an image by unsharp masking.
	/// </summary>
	public class UnsharpMaskFilter : ShaderEffectFilter
	{
		#region Properties

		/// <summary>
		/// Gets or sets the amount. Values range from 0 to 100. Defaults to 50.
		/// </summary>
		public int Amount
		{
			get { return (int)(this["Amount"] ?? 50); }
			set
			{
				if (value < 0 || value > 100)
					throw new ArgumentOutOfRangeException("value", "Unsharp amounts must range from 0 to 100.");

				this["Amount"] = value;
			}
		}

		/// <summary>
		/// Gets or sets the feather radius. Values range from 0 to 100. Defaults to 5.
		/// </summary>
		public int Radius
		{
			get { return (int)(this["Radius"] ?? 5); }
			set
			{
				if (value < 0 || value > 100)
					throw new ArgumentOutOfRangeException("value", "Blur radius values must range from 0 to 100.");
				this["Radius"] = value;
			}
		}

		/// <summary>
		/// The threshold beyond which different pixel values will be subtracted. Defaults to 1.
		/// </summary>
		public int Threshold
		{
			get { return (int)(this["Threshold"] ?? 1); }
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException("value", "Unsharp mask threshold must be greater than 0.");
				this["Threshold"] = value;
			}
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
			dc.DrawImage(source.InnerBitmap, new Rect(0, 0, source.Width, source.Height));
			dc.Close();

			RenderTargetBitmap rtb = RenderTargetBitmapUtility.CreateRenderTargetBitmap(source.Width, source.Height);
			rtb.Render(dv);

			Brush blurredImage = new ImageBrush(rtb);

			return new UnsharpMaskEffect
			{
				BlurMask = blurredImage,
				Amount = Amount / 100.0,
				Threshold = Threshold
			};
		}

		#endregion
	}
}
