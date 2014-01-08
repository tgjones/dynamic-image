using System;
using System.Windows.Media.Effects;
using SoundInTheory.DynamicImage.ShaderEffects;
using SoundInTheory.DynamicImage.Util;

namespace SoundInTheory.DynamicImage.Filters
{
	/// <summary>
	/// Adjusts the contrast of the layer.
	/// </summary>
	public class ContrastAdjustmentFilter : ShaderEffectFilter
	{
		#region Properties

		/// <summary>
		/// Gets or sets the contrast level. Values range from -100 to 100. Defaults to 0.
		/// </summary>
		public int Level
		{
			get { return (int)(this["Level"] ?? 0); }
			set
			{
				if (value < -100 || value > 100)
					throw new ArgumentOutOfRangeException("value", "Contrast level values must range from -100 to 100.");
				this["Level"] = value;
			}
		}

		#endregion

		#region Methods

        protected override Effect GetEffect(FastBitmap source)
		{
			return new ContrastAdjustmentEffect
			{
				Level = (Level + 100) / 40.0
			};
		}

		#endregion
	}
}
