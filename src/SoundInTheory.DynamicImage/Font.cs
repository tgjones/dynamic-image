using System.Windows;
using System.Windows.Media;
using SoundInTheory.DynamicImage.Util;

namespace SoundInTheory.DynamicImage
{
	public class Font : DirtyTrackingObject
	{
		#region Properties

		public bool Bold
		{
			get { return (bool)(this["Bold"] ?? false); }
			set { this["Bold"] = value; }
		}

		public bool Italic
		{
			get { return (bool)(this["Italic"] ?? false); }
			set { this["Italic"] = value; }
		}

		public bool Strikeout
		{
			get { return (bool)(this["Strikeout"] ?? false); }
			set { this["Strikeout"] = value; }
		}

		public bool Underline
		{
			get { return (bool)(this["Underline"] ?? false); }
			set { this["Underline"] = value; }
		}

		public string Name
		{
			get { return (string)(this["Name"] ?? "Verdana"); }
			set { this["Name"] = value; }
		}

		/// <summary>
		/// Gets or sets the custom font file to use for this Font object. The file can be specified as a relative URL (~/assets/fonts/arial.ttf), or full rooted path pointing to the custom font file on a local disk.
		/// </summary>
		public string CustomFontFile
		{
			get { return (string)(this["CustomFontFile"]); }
			set { this["CustomFontFile"] = value; }
		}

		public float Size
		{
			get { return (float)(this["Size"] ?? 18.0f); }
			set { this["Size"] = value; }
		}

		#endregion

        public FontDescription GetFontDescription()
		{
			FontFamily fontFamily;
			if (!string.IsNullOrEmpty(CustomFontFile))
			{
                string fontFileName = FileSourceHelper.ResolveFileName(CustomFontFile);
				fontFamily = new FontFamily(fontFileName + "#" + Name);
			}
			else
			{
				fontFamily = new FontFamily(Name);
			}
			Typeface typeface = new Typeface(fontFamily, GetStyle(), GetWeight(), FontStretches.Normal);
			return new FontDescription(typeface, GetTextDecorations(), Size);
		}

		private TextDecorationCollection GetTextDecorations()
		{
			TextDecorationCollection textDecorations = new TextDecorationCollection();
			if (Strikeout)
				textDecorations.Add(TextDecorations.Strikethrough);
			if (Underline)
				textDecorations.Add(TextDecorations.Underline);
			return textDecorations;
		}

		private FontStyle GetStyle()
		{
			if (Italic)
				return FontStyles.Italic;
			return FontStyles.Normal;
		}

		private FontWeight GetWeight()
		{
			if (Bold)
				return FontWeights.Bold;
			return FontWeights.Normal;
		}
	}
}
