using System;
using System.Windows.Media;
using System.Windows.Media.Effects;
using SoundInTheory.DynamicImage.Util;

namespace SoundInTheory.DynamicImage.Filters
{
	/// <summary>
	/// The base class for filters that apply an HLSL shader to the image.
	/// </summary>
	public abstract class ShaderEffectFilter : ImageReplacementFilter
	{
		/// <summary>
		/// Returns the dimensions of the output image.
		/// </summary>
		/// <param name="source">The source image.</param>
		/// <param name="width">The desired width of the output image.</param>
		/// <param name="height">The desired height of the output image.</param>
		/// <returns><c>true</c> if the destination image should be created; otherwise <c>false</c>.</returns>
		protected override bool GetDestinationDimensions(FastBitmap source, out int width, out int height)
		{
			width = source.Width;
			height = source.Height;
			return true;
		}

        protected override void ConfigureDrawingVisual(FastBitmap source, DrawingVisual drawingVisual)
		{
			Effect shaderEffect = GetEffect(source);
			drawingVisual.Effect = shaderEffect;
		}

        protected abstract Effect GetEffect(FastBitmap source);

		protected override void CleanUpDrawingVisual(FastBitmap source, DrawingVisual drawingVisual)
		{
			if (drawingVisual.Effect is IDisposable)
				((IDisposable) drawingVisual.Effect).Dispose();
			base.CleanUpDrawingVisual(source, drawingVisual);
		}

		/// <summary>
		/// Applies the filter to the specified source.
		/// </summary>
		/// <param name="source">The source image.</param>
		/// <param name="dc">A <see cref="System.Windows.Media.DrawingContext"/> object, created from the destination image.</param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		protected override void ApplyFilter(FastBitmap source, DrawingContext dc, int width, int height)
		{
			dc.DrawImage(source.InnerBitmap, new System.Windows.Rect(0, 0, source.Width, source.Height));
		}
	}
}