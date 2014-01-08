using System.Windows.Media.Effects;
using SoundInTheory.DynamicImage.ShaderEffects;
using SoundInTheory.DynamicImage.Util;
using SWMColor = System.Windows.Media.Color;

namespace SoundInTheory.DynamicImage.Filters
{
	/// <summary>
	/// Makes a specified color transparent in the output image. Can also use the
	/// top-left pixel in the source image as the transparent color.
	/// </summary>
	public class ColorKeyFilter : ShaderEffectFilter
	{
		#region Properties

		/// <summary>
		/// Gets or sets the transparent color. Defaults to White.
		/// </summary>
		public Color Color
		{
			get { return (Color)(this["Color"] ?? Colors.White); }
			set { this["Color"] = value; }
		}

		/// <summary>
		/// Gets or sets the color tolerance for transparency. This value is used to create a range around the Color parameter, within which colors will be considered transparent. Ranges from 0 to 255. Defaults to 0.
		/// </summary>
		public byte ColorTolerance
		{
			get { return (byte)(this["ColorTolerance"] ?? 0); }
			set { this["ColorTolerance"] = value; }
		}

		/// <summary>
		/// Gets or sets whether the first (top left) pixel of an image will be used as the transparent color. Defaults to false.
		/// </summary>
		public bool UseFirstPixel
		{
			get { return (bool)(this["UseFirstPixel"] ?? false); }
			set { this["UseFirstPixel"] = value; }
		}

		#endregion

        protected override Effect GetEffect(FastBitmap source)
		{
			SWMColor transparentColor;
			if (UseFirstPixel)
			{
				source.Lock();
				transparentColor = source[0, 0];
				source.Unlock();
			}
			else
			{
				transparentColor = Color.ToWpfColor();
			}

			return new ColorKeyEffect
			{
				ColorTolerance = ColorTolerance / 255.0,
				TransparentColor = transparentColor
			};
		}
	}
}