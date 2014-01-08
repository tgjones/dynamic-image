using System.Windows.Media.Imaging;
using SoundInTheory.DynamicImage.Util;

namespace SoundInTheory.DynamicImage.Sources
{
	public class ImageImageSource : ImageSource
	{
		public BitmapSource Image
		{
			get { return (BitmapSource) this["Image"]; }
			set { this["Image"] = value; }
		}

        public override FastBitmap GetBitmap()
		{
			return new FastBitmap(Image);
		}
	}
}
