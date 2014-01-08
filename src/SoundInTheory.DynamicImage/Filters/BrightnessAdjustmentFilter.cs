using System;
using System.Windows.Media.Effects;
using SoundInTheory.DynamicImage.ShaderEffects;
using SoundInTheory.DynamicImage.Util;

namespace SoundInTheory.DynamicImage.Filters
{
	/// <summary>
	/// Adjusts the brightness of the layer.
	/// </summary>
	public class BrightnessAdjustmentFilter : ShaderEffectFilter
	{
		#region Properties

		/// <summary>
		/// Gets or sets the brightness level. Values range from -100 to 100. Defaults to 0.
		/// </summary>
		public int Level
		{
			get { return (int)(this["Level"] ?? 0); }
			set
			{
				if (value < -100 || value > 100)
					throw new ArgumentOutOfRangeException("value", "Brightness level values must range from -100 to 100.");
				this["Level"] = value;
			}
		}

		#endregion

		#region Methods

        protected override Effect GetEffect(FastBitmap source)
		{
			return new BrightnessAdjustmentEffect
			{
				Level = this.Level/100.0
			};
		}

		#endregion
	}
}
