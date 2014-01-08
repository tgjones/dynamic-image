using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using SoundInTheory.DynamicImage.ShaderEffects;
using SoundInTheory.DynamicImage.Util;
using ImageSource = SoundInTheory.DynamicImage.Sources.ImageSource;

namespace SoundInTheory.DynamicImage.Filters
{
	/// <summary>
	/// Clips (reveals) the content of the layer based on the 
	/// non-transparent content of the specified image.
	/// </summary>
	public class ClippingMaskFilter : ShaderEffectFilter
	{
		#region Fields

		private int _maskImageWidth, _maskImageHeight;

		#endregion

		#region Properties

		public ImageSource MaskImage
		{
			get { return (ImageSource) this["MaskImage"]; }
			set { this["MaskImage"] = value; }
		}

		/// <summary>
		/// Gets or sets the x-position of the left side of the mask image.
		/// </summary>
		public int MaskPositionX
		{
			get { return (int)(this["MaskPositionX"] ?? 0); }
			set { this["MaskPositionX"] = value; }
		}

		/// <summary>
		/// Gets or sets the y-position of the top side of the mask image.
		/// </summary>
		public int MaskPositionY
		{
			get { return (int)(this["MaskPositionY"] ?? 0); }
			set { this["MaskPositionY"] = value; }
		}

		#endregion

		#region Methods

        protected override Effect GetEffect(FastBitmap source)
		{
            FastBitmap maskBitmap = MaskImage.GetBitmap();
			_maskImageWidth = maskBitmap.Width;
			_maskImageHeight = maskBitmap.Height;
			return new ClippingMaskEffect
			{
				Mask = new ImageBrush(maskBitmap.InnerBitmap),
				InputCoordsOffset = new Vector(MaskPositionX / (double) source.Width, MaskPositionY / (double) source.Height),
				InputCoordsScale = new Vector(_maskImageWidth / (double) source.Width, _maskImageHeight / (double) source.Height)
			};
		}

		protected override void ApplyFilter(FastBitmap source, DrawingContext dc, int width, int height)
		{
			dc.DrawImage(source.InnerBitmap, new Rect(MaskPositionX, MaskPositionY, _maskImageWidth, _maskImageHeight));
		}

		#endregion
	}
}
