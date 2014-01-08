using System;
using System.Windows.Media.Effects;
using SoundInTheory.DynamicImage.ShaderEffects;
using SoundInTheory.DynamicImage.Util;

namespace SoundInTheory.DynamicImage.Filters
{
	/// <summary>
	/// Adjusts the color tint of the layer.
	/// </summary>
	public class ColorTintFilter : ShaderEffectFilter
	{
		#region Properties

		/// <summary>
		/// Gets or sets the color used to tint the layer. Defaults to Red.
		/// </summary>
		public Color Color
		{
			get { return (Color) (this["Color"] ?? Colors.Red); }
			set { this["Color"] = value; }
		}

		/// <summary>
		/// Gets or sets the color tint amount. Values range from 0 (image will be grayscale) to 100. Defaults to 25.
		/// </summary>
		public int Amount
		{
			get { return (int)(this["Amount"] ?? 25); }
			set
			{
				if (value < 0 || value > 100)
					throw new ArgumentOutOfRangeException("value", "Color tint amounts must range from 0 to 100.");
				this["Amount"] = value;
			}
		}

		#endregion

		#region Methods

        protected override Effect GetEffect(FastBitmap source)
		{
			return new ColorTintEffect
			{
				Amount = Amount/100.0,
				RequiredColor = Color.ToWpfColor()
			};
		}

		#endregion
	}
}
