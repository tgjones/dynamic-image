using System.IO;
using SoundInTheory.DynamicImage.Util;

namespace SoundInTheory.DynamicImage.Layers
{
	public class WebsiteScreenshotLayer : Layer
	{
		public string WebsiteUrl
		{
			get { return this["WebsiteUrl"] as string ?? string.Empty; }
			set { this["WebsiteUrl"] = value; }
		}

		/// <summary>
		/// Maximum time to wait for the screenshot, in milliseconds.
		/// </summary>
		public int Timeout
		{
			get { return (int)(this["Timeout"] ?? 5000); }
			set { this["Timeout"] = value; }
		}

		/// <summary>
		/// Width to use for browser rendering, in pixels.
		/// </summary>
		public int BrowserWidth
		{
			get { return (int)(this["BrowserWidth"] ?? 1024); }
			set { this["BrowserWidth"] = value; }
		}

		public override bool HasFixedSize
		{
			get { return true; }
		}

        protected override void CreateImage()
		{
			string outputFileName = Path.GetTempFileName();

			try
			{
				if (!new CutyCaptWrapper().SaveScreenShot(WebsiteUrl, outputFileName, Timeout, BrowserWidth))
					return;
				Bitmap = new FastBitmap(File.ReadAllBytes(outputFileName));
			}
			finally
			{
				File.Delete(outputFileName);
			}
		}
	}
}