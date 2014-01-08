using System.ComponentModel;
using System.IO;
using System.Web;
using System.Web.UI;
using SoundInTheory.DynamicImage.Util;

namespace SoundInTheory.DynamicImage.Sources
{
	public class FileImageSource : ImageSource
	{
		[Category("Source"), Browsable(true), UrlProperty]
		public string FileName
		{
			get { return (string)(this["FileName"] ?? string.Empty); }
			set { this["FileName"] = value; }
		}

		public override FastBitmap GetBitmap()
		{
            string resolvedFileName = FileSourceHelper.ResolveFileName(FileName);
			if (File.Exists(resolvedFileName))
				return new FastBitmap(resolvedFileName);
			return null;
		}
	}
}
