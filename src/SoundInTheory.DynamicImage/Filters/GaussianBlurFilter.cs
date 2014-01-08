using System;
using System.Windows.Media.Effects;
using SoundInTheory.DynamicImage.Util;

namespace SoundInTheory.DynamicImage.Filters
{
	/// <summary>
	/// Blurs the image. A Gaussian blur removes fine image detail
	/// and noise leaving only larger scale changes.
	/// </summary>
	public class GaussianBlurFilter : ShaderEffectFilter
	{
		#region Properties

		/// <summary>
		/// Gets or sets the radius used in the blur kernel. A larger radius implies more blurring.
		/// Valid values are between 0 and 20 (inclusive). Defaults to 0.
		/// </summary>
		public float Radius
		{
			get { return (float) (this["Radius"] ?? 0.0f); }
			set
			{
				if (value <= 0 || value > 20)
					throw new ArgumentOutOfRangeException("value", "Gaussian blur radius must range from 0 to less than 20.");
				this["Radius"] = value;
			}
		}

		#endregion

        protected override Effect GetEffect(FastBitmap source)
		{
			return new BlurEffect
			{
				KernelType = KernelType.Gaussian,
				Radius = Radius,
				RenderingBias = RenderingBias.Quality
			};
		}
	}
}
