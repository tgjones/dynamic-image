using System.Collections.Generic;
using SoundInTheory.DynamicImage.Caching;
using SoundInTheory.DynamicImage.Sources;
using SoundInTheory.DynamicImage.Util;

namespace SoundInTheory.DynamicImage.Layers
{
	public class ImageLayer : Layer
	{
		#region Properties

		public ImageSource Source
		{
			get { return (ImageSource) this["Source"]; }
			set { this["Source"] = value; }
		}

		/// <summary>
		/// Shortcut route to Source/FileImageSource
		/// </summary>
		public string SourceFileName
		{
			set { Source = new FileImageSource { FileName = value }; }
		}

		public ImageSource AlternateSource
		{
			get { return (ImageSource)this["AlternateSource"]; }
			set { this["AlternateSource"] = value; }
		}

		public override bool HasFixedSize
		{
			get { return true; }
		}

		#endregion

        protected override void CreateImage()
		{
            FastBitmap sourceValue = this.Source.GetBitmap();
			if (sourceValue != null && sourceValue.InnerBitmap != null)
			{
				this.Bitmap = sourceValue;
			}
			else if (AlternateSource != null)
			{
				sourceValue = this.AlternateSource.GetBitmap();
				if (sourceValue != null && sourceValue.InnerBitmap != null)
					this.Bitmap = sourceValue;
			}
		}

		public override void PopulateDependencies(List<Dependency> dependencies)
		{
			base.PopulateDependencies(dependencies);
			Source.PopulateDependencies(dependencies);
			if (AlternateSource != null)
				AlternateSource.PopulateDependencies(dependencies);
		}
	}
}
