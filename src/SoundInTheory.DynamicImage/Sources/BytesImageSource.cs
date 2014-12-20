using SoundInTheory.DynamicImage.Caching;
using SoundInTheory.DynamicImage.Util;
using System.Collections.Generic;

namespace SoundInTheory.DynamicImage.Sources
{
	public class BytesImageSource : ImageSource
	{
		public byte[] Bytes
		{
			get { return (byte[]) this["Bytes"]; }
			set { this["Bytes"] = value; }
		}

		public override FastBitmap GetBitmap(ImageGenerationContext context)
		{
			byte[] bytes = this.Bytes;
			if (bytes != null && bytes.Length > 0)
				return new FastBitmap(bytes);
			return null;
		}

        public override void PopulateDependencies(List<Dependency> dependencies)
        {
            // TO-DO: GERAR UM HASH DO BYTEARRAY E COLOCAR COMO DEPENDÊNCIA
            // Dependency dependency = new Dependency();
            // dependency.Text1 = Hash(Bytes);
            // dependencies.Add(dependency);
        }
	}
}
