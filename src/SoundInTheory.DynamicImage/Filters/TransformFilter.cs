using System;
using System.Windows;
using SoundInTheory.DynamicImage.Util;
using SWMColor = System.Windows.Media.Color;
using SWMColors = System.Windows.Media.Colors;

namespace SoundInTheory.DynamicImage.Filters
{
	public abstract class TransformFilter : Filter
	{
		#region Properties

		/// <summary>
		/// Gets or sets the action to perform for pixels off the edge of the image.
		/// </summary>
		public EdgeAction EdgeAction
		{
			get { return (EdgeAction)(this["EdgeAction"] ?? EdgeAction.Zero); }
			set { this["EdgeAction"] = value; }
		}

		/// <summary>
		/// Gets or sets the type of interpolation to perform.
		/// </summary>
		public InterpolationMode InterpolationMode
		{
			get { return (InterpolationMode)(this["InterpolationMode"] ?? InterpolationMode.Bilinear); }
			set { this["InterpolationMode"] = value; }
		}

		protected Int32Rect OriginalSpace { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Inverse transform a point. This method needs to be overriden by all subclasses.
		/// </summary>
		/// <param name="x">The X position of the pixel in the output image.</param>
		/// <param name="y">The Y position of the pixel in the output image.</param>
		/// <returns>The position of the pixel in the input image.</returns>
		protected abstract Point TransformInverse(int x, int y);

		/// <summary>
		/// Forward transform a rectangle. Used to determine the size of the output image.
		/// </summary>
		/// <param name="rect">The rectangle to transform.</param>
		/// <returns>The transformed rectangle.</returns>
		protected virtual Int32Rect GetTransformedSpace(Int32Rect rect) { return rect; }

        public sealed override void ApplyFilter(FastBitmap bitmap)
		{
			OnBeginApplyFilter(bitmap);

			// get destination dimensions
			int destWidth, destHeight;
			bool shouldContinue = GetDestinationDimensions(bitmap, out destWidth, out destHeight);
			if (!shouldContinue)
				return;

			FastBitmap destination = new FastBitmap(destWidth, destHeight);

			// copy metadata
			// TODO
			/*foreach (PropertyItem propertyItem in bitmap.InnerBitmap.PropertyItems)
				destination.InnerBitmap.SetPropertyItem(propertyItem);*/

			int width = bitmap.Width;
			int height = bitmap.Height;

			OriginalSpace = new Int32Rect(0, 0, width, height);
			Int32Rect transformedSpace = GetTransformedSpace(OriginalSpace);

			try
			{
				bitmap.Lock();
				destination.Lock();

				if (InterpolationMode == InterpolationMode.NearestNeighbor)
					FilterPixelsNearestNeighbor(bitmap, destination, width, height, transformedSpace);
				else
					FilterPixelsBilinear(bitmap, destination, width, height, transformedSpace);
			}
			finally
			{
				destination.Unlock();
				bitmap.Unlock();
			}

			bitmap.InnerBitmap = destination.InnerBitmap;
		}

		/// <summary>
		/// This method allows derived classes to perform actions 
		/// prior to the filter being applied.
		/// </summary>
		protected virtual void OnBeginApplyFilter(FastBitmap bitmap) { }

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

		private void FilterPixelsNearestNeighbor(FastBitmap source, FastBitmap destination, int width, int height, Int32Rect transformedSpace)
		{
			int srcWidth = width;
			int srcHeight = height;
			int outWidth = transformedSpace.Width;
			int outHeight = transformedSpace.Height;

			int outX = transformedSpace.X;
			int outY = transformedSpace.Y;

			for (int y = 0; y < outHeight; y++)
			{
				for (int x = 0; x < outWidth; x++)
				{
					Point output = TransformInverse(outX + x, outY + y);
					int srcX = (int) output.X;
					int srcY = (int) output.Y;
					// int casting rounds towards zero, so we check out[0] < 0, not srcX < 0
					if (output.X < 0 || srcX >= srcWidth || output.Y < 0 || srcY >= srcHeight)
						destination[x, y] = GetPixel(source, srcX, srcY, srcWidth, srcHeight);
					else
						destination[x, y] = source[srcX, srcY];
				}
			}
		}

		private void FilterPixelsBilinear(FastBitmap source, FastBitmap destination, int width, int height, Int32Rect transformedSpace)
		{
			int srcWidth = width;
			int srcHeight = height;
			int srcWidth1 = width - 1;
			int srcHeight1 = height - 1;
			int outWidth = transformedSpace.Width;
			int outHeight = transformedSpace.Height;

			int outX = transformedSpace.X;
			int outY = transformedSpace.Y;

			for (int y = 0; y < outHeight; y++)
			{
				for (int x = 0; x < outWidth; x++)
				{
					Point output = TransformInverse(outX + x, outY + y);
					int srcX = (int)Math.Floor(output.X);
					int srcY = (int)Math.Floor(output.Y);
					double xWeight = output.X - srcX;
					double yWeight = output.Y - srcY;
					SWMColor nw, ne, sw, se;

					if (srcX >= 0 && srcX < srcWidth1 && srcY >= 0 && srcY < srcHeight1)
					{
						// Easy case, all corners are in the image
						nw = source[srcX, srcY];
						ne = source[srcX + 1, srcY];
						sw = source[srcX, srcY + 1];
						se = source[srcX + 1, srcY + 1];
					}
					else
					{
						// Some of the corners are off the image
						nw = GetPixel(source, srcX, srcY, srcWidth, srcHeight);
						ne = GetPixel(source, srcX + 1, srcY, srcWidth, srcHeight);
						sw = GetPixel(source, srcX, srcY + 1, srcWidth, srcHeight);
						se = GetPixel(source, srcX + 1, srcY + 1, srcWidth, srcHeight);
					}
					destination[x, y] = ImageMath.BilinearInterpolate(xWeight, yWeight, nw, ne, sw, se);
				}
			}
		}

		private SWMColor GetPixel(FastBitmap bitmap, int x, int y, int width, int height)
		{
			if (x < 0 || x >= width || y < 0 || y >= height)
			{
				switch (EdgeAction)
				{
					case EdgeAction.Wrap:
						return bitmap[ImageMath.Mod(x, width), ImageMath.Mod(y, height)];
					case EdgeAction.Clamp:
						return bitmap[ImageMath.Clamp(x, 0, width - 1), ImageMath.Clamp(y, 0, height - 1)];
					case EdgeAction.RgbClamp:
					{
						var edgeColor = bitmap[ImageMath.Clamp(x, 0, width - 1), ImageMath.Clamp(y, 0, height - 1)];
						return SWMColor.FromArgb(0, edgeColor.R, edgeColor.G, edgeColor.B);
					}
					case EdgeAction.Zero:
						return SWMColors.Transparent;
				}
			}
			return bitmap[x, y];
		}

		#endregion
	}
}