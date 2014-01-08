using System.Windows.Media.Effects;
using SoundInTheory.DynamicImage.ShaderEffects;
using SoundInTheory.DynamicImage.Util;

namespace SoundInTheory.DynamicImage.Filters
{
	/// <summary>
	/// Embosses an image.
	/// </summary>
	public class EmbossFilter : ShaderEffectFilter
	{
		#region Fields

		//private const float PIXEL_SCALE = 255.9f;

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the bump height for embossing. Defaults to 3.
		/// </summary>
		public float Amount
		{
			get { return (float)(this["Amount"] ?? 3.0f); }
			set
			{
				//if (value < 2 || value > 50)
				//    throw new ArgumentException("The bump height must be between 2 and 50.", "value");
				this["Amount"] = value;
			}
		}

		/// <summary>
		/// Gets or sets the bump height for embossing. Defaults to 3.
		/// </summary>
		public float Width
		{
			get { return (float)(this["Width"] ?? 3.0f); }
			set
			{
				//if (value < 2 || value > 50)
				//	throw new ArgumentException("The bump height must be between 2 and 50.", "value");
				this["Width"] = value;
			}
		}

		#endregion

		#region Methods

        protected override Effect GetEffect(FastBitmap source)
		{
			return new EmbossEffect
			{
				Amount = Amount,
				Width = Width/(double) source.Width
			};
		}

		#endregion
	}
}
