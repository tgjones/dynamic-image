using System.Windows.Media.Effects;
using SoundInTheory.DynamicImage.ShaderEffects;
using SoundInTheory.DynamicImage.Util;

namespace SoundInTheory.DynamicImage.Filters
{
	/// <summary>
	/// Inverts the colors of the image.
	/// </summary>
	public class InversionFilter : ShaderEffectFilter
	{
        protected override Effect GetEffect(FastBitmap source)
		{
			return new InversionEffect();
		}
	}
}
