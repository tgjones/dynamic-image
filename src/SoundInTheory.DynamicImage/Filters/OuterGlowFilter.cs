using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using SoundInTheory.DynamicImage.Util;

namespace SoundInTheory.DynamicImage.Filters
{
	public class OuterGlowFilter : ShadowFilterBase
	{
		#region Methods

        protected override Effect GetEffect(FastBitmap source)
		{
			return new DropShadowEffect
			{
				Direction = 0,
				Color = Color.ToWpfColor(),
				BlurRadius = Size,
				ShadowDepth = 0,
				Opacity = Opacity
			};
		}

		protected override Vector GetPadding()
		{
			return new Vector(Size * 2, Size * 2);
		}

		protected override void ApplyFilter(FastBitmap source, DrawingContext dc, int width, int height)
		{
			dc.PushTransform(new TranslateTransform(Size, Size));
			base.ApplyFilter(source, dc, width, height);
			dc.Pop();
		}

		#endregion
	}
}
