using System.Windows.Media;
using System.Windows.Media.Effects;
using SoundInTheory.DynamicImage.ShaderEffects;
using SoundInTheory.DynamicImage.Util;
using SWMColor = System.Windows.Media.Color;

namespace SoundInTheory.DynamicImage.Filters
{
	public abstract class TransferFilter : ShaderEffectFilter
	{
        protected override Effect GetEffect(FastBitmap source)
		{
			FastBitmap transferLookup = new FastBitmap(1, 256);
			transferLookup.Lock();
			for (int y = 0; y < 256; ++y)
			{
				byte colorComponent = (byte) (255 * GetTransferFunctionValue(y / 255.0f));
				transferLookup[0, y] = SWMColor.FromRgb(colorComponent, colorComponent, colorComponent);
			}
			transferLookup.Unlock();

			return new TransferEffect
			{
				TransferLookup = new ImageBrush(transferLookup.InnerBitmap)
			};
		}

		protected abstract float GetTransferFunctionValue(float value);
	}
}