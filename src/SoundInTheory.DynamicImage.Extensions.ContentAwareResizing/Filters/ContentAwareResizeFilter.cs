using System.IO;
using SoundInTheory.DynamicImage.Util;

namespace SoundInTheory.DynamicImage.Filters
{
	public class ContentAwareResizeFilter : Filter
	{
		#region Properties

		/// <summary>
		/// Gets or sets the desired image width.
		/// </summary>
		public Unit Width
		{
			get { return (Unit)(this["Width"] ?? Unit.Empty); }
			set { this["Width"] = value; }
		}

		/// <summary>
		/// Gets or sets the desired image height.
		/// </summary>
		public Unit Height
		{
			get { return (Unit)(this["Height"] ?? Unit.Empty); }
			set { this["Height"] = value; }
		}

		/// <summary>
		/// Gets or sets the convolution type.
		/// </summary>
		public ContentAwareResizeFilterConvolutionType ConvolutionType
		{
			get { return (ContentAwareResizeFilterConvolutionType)(this["ConvolutionType"] ?? ContentAwareResizeFilterConvolutionType.Prewitt); }
			set { this["ConvolutionType"] = value; }
		}

		#endregion

        public override void ApplyFilter(FastBitmap bitmap)
		{
			if (Width == Unit.Empty && Height == Unit.Empty)
				throw new DynamicImageException("At least one of Width or Height must be set.");

			string sourceFileName = Path.GetTempFileName();
			try
			{
				bitmap.Save(sourceFileName);

				string outputFileName = Path.GetTempFileName();

				try
				{
					int width = (Width == Unit.Empty) ? bitmap.Width : Unit.GetCalculatedValue(Width, bitmap.Width);
					int height = (Height == Unit.Empty) ? bitmap.Height : Unit.GetCalculatedValue(Height, bitmap.Height);
					new CairWrapper().ProcessImage(sourceFileName, outputFileName, int.MaxValue,
						width, height, ConvolutionType);
					FastBitmap output = new FastBitmap(File.ReadAllBytes(outputFileName));
					if (output.InnerBitmap != null)
						bitmap.InnerBitmap = output.InnerBitmap;
				}
				finally
				{
					File.Delete(outputFileName);
				}
			}
			finally
			{
				File.Delete(sourceFileName);
			}
		}
	}
}