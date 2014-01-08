using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SoundInTheory.DynamicImage.Util;

namespace SoundInTheory.DynamicImage.Filters
{
	/// <summary>
	/// Crops an image. The rectangular section to crop is defined by X/Y coordinates and a size.
	/// </summary>
	public class CropFilter : ImageReplacementFilter
	{
		#region Properties

		/// <summary>
		/// Gets or sets the X-coordinate of the rectangular section to crop. Defaults to 0.
		/// </summary>
		public int X
		{
			get { return (int) (this["X"] ?? 0); }
			set
			{
				if (value < 0)
					throw new ArgumentException("The X-coordinate of the rectangular section must be greater than or equal to zero.", "value");
				this["X"] = value;
			}
		}

		/// <summary>
		/// Gets or sets the Y-coordinate of the rectangular section to crop. Defaults to 0.
		/// </summary>
		public int Y
		{
			get { return (int)(this["Y"] ?? 0); }
			set
			{
				if (value < 0)
					throw new ArgumentException("The Y-coordinate of the rectangular section must be greater than or equal to zero.", "value");
				this["Y"] = value;
			}
		}

		/// <summary>
		/// Gets or sets the width of the rectangular section to crop. Defaults to 200.
		/// </summary>
		public int Width
		{
			get { return (int)(this["Width"] ?? 200); }
			set
			{
				if (value < 1)
					throw new ArgumentException("The width of the rectangular section must be greater than one.", "value");
				this["Width"] = value;
			}
		}

		/// <summary>
		/// Gets or sets the height of the rectangular section to crop. Defaults to 200.
		/// </summary>
		public int Height
		{
			get { return (int)(this["Height"] ?? 200); }
			set
			{
				if (value < 1)
					throw new ArgumentException("The height of the rectangular section must be greater than one.", "value");
				this["Height"] = value;
			}
		}

		#endregion

		#region Methods

		protected override bool GetDestinationDimensions(FastBitmap source, out int width, out int height)
		{
			width = Width;
			height = Height;
			return true;
		}

		protected override void ApplyFilter(FastBitmap source, DrawingContext dc, int width, int height)
		{
            if (Width > source.Width)
                Width = source.Width;

            if (Height > source.Height)
                Height = source.Height;

			BitmapSource bitmapSource = new CroppedBitmap(source.InnerBitmap, new System.Windows.Int32Rect(X, Y, Width, Height));
			dc.DrawImage(bitmapSource, new System.Windows.Rect(0, 0, width, height));
		}

		#endregion
	}
}
