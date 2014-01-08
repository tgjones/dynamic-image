using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SoundInTheory.DynamicImage.Util;

namespace SoundInTheory.DynamicImage.Filters
{
	/// <summary>
	/// Provides the abstract base class for a filter which replaces the entire image.
	/// Examples of this filter include <see cref="SoundInTheory.DynamicImage.Filters.ResizeFilter" />
	/// and <see cref="SoundInTheory.DynamicImage.Filters.RotationFilter" />.
	/// </summary>
	public abstract class ImageReplacementFilter : Filter
	{
	    /// <summary>
	    /// Applies the filter to the specified <paramref name="bitmap"/>. This method
	    /// first calls <see cref="ImageReplacementFilter.GetDestinationDimensions(FastBitmap, out Int32, out Int32)" />
	    /// to calculate the size of the destination image. Then it calls
	    /// <see cref="ImageReplacementFilter.ApplyFilter(FastBitmap, DrawingContext, int, int)" /> 
	    /// which is where the overridden class implements its filter algorithm.
	    /// </summary>
	    /// <param name="context"></param>
	    /// <param name="bitmap">
	    /// Image to apply the <see cref="ImageReplacementFilter" /> to.
	    /// </param>
        public sealed override void ApplyFilter(FastBitmap bitmap)
		{
			OnBeginApplyFilter(bitmap);

			// get destination dimensions
			int width, height;
			bool shouldContinue = GetDestinationDimensions(bitmap, out width, out height);
			if (!shouldContinue)
				return;

			DrawingVisual dv = new DrawingVisual();
			ConfigureDrawingVisual(bitmap, dv);

			DrawingContext dc = dv.RenderOpen();

			ApplyFilter(bitmap, dc, width, height);
			dc.Close();

			RenderTargetBitmap rtb = RenderTargetBitmapUtility.CreateRenderTargetBitmap(width, height);
			rtb.Render(dv);
			FastBitmap destination = new FastBitmap(rtb);

			// copy metadata
			// TODO
			/*foreach (PropertyItem propertyItem in bitmap.InnerBitmap.PropertyItems)
				destination.InnerBitmap.SetPropertyItem(propertyItem);*/

			// set new image
			bitmap.InnerBitmap = destination.InnerBitmap;

			OnEndApplyFilter();
		}

		/// <summary>
		/// This method allows derived classes to perform actions 
		/// prior to the filter being applied.
		/// </summary>
		protected virtual void OnBeginApplyFilter(FastBitmap bitmap) { }

		/// <summary>
		/// This method allows derived classes to perform actions 
		/// after the filter has been applied.
		/// </summary>
		protected virtual void OnEndApplyFilter() { }

		/// <summary>
		/// When overridden in a derived class, returns the dimensions of the output image.
		/// </summary>
		/// <param name="source">The source image.</param>
		/// <param name="width">The desired width of the output image.</param>
		/// <param name="height">The desired height of the output image.</param>
		/// <returns><c>true</c> if the destination image should be created; otherwise <c>false</c>.
		/// Some filters may not need to change the image; for example, if the <see cref="SoundInTheory.DynamicImage.Filters.ResizeFilter" />
		/// detects that the requested dimensions are the same as the source's dimensions, then it will
		/// return <c>false</c>.</returns>
		protected abstract bool GetDestinationDimensions(FastBitmap source, out int width, out int height);

	    /// <summary>
	    /// Inherited classes can use this to configure the supplied DrawingVisual. For example,
	    /// RotationFilter sets the BitmapScalingMode render option.
	    /// </summary>
	    /// <param name="context"></param>
	    /// <param name="source"> </param>
	    /// <param name="drawingVisual"></param>
	    protected virtual void ConfigureDrawingVisual(FastBitmap source, DrawingVisual drawingVisual)
		{
			
		}

		/// <summary>
		/// Inherited classes can use this to clean up the supplied DrawingVisual. For example,
		/// ShaderEffectFilter disposes the shader effect.
		/// </summary>
		/// <param name="source"> </param>
		/// <param name="drawingVisual"></param>
		protected virtual void CleanUpDrawingVisual(FastBitmap source, DrawingVisual drawingVisual)
		{

		}

		/// <summary>
		/// When overridden in a derived class, applies the filter to the specified source.
		/// </summary>
		/// <param name="source">The source image.</param>
		/// <param name="destination">The destination image.</param>
		/// <param name="dc">A <see cref="System.Windows.Media.DrawingContext" /> object, created from <paramref name="destination"/>.</param>
		protected abstract void ApplyFilter(FastBitmap source, DrawingContext dc, int width, int height);
	}
}
