using System.Collections.Generic;
using SoundInTheory.DynamicImage.Caching;
using SoundInTheory.DynamicImage.Util;

namespace SoundInTheory.DynamicImage.Sources
{
	public abstract class ImageSource : DirtyTrackingObject
	{
        public abstract FastBitmap GetBitmap();

		public virtual void PopulateDependencies(List<Dependency> dependencies) { }
	}
}
