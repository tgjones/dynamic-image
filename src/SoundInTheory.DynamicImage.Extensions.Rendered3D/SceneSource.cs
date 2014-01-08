using System.Collections.Generic;
using Meshellator;
using SoundInTheory.DynamicImage.Caching;
using SoundInTheory.DynamicImage.Util;

namespace SoundInTheory.DynamicImage
{
	public abstract class SceneSource : DirtyTrackingObject
	{
        public abstract Scene GetScene();

		public virtual void PopulateDependencies(List<Dependency> dependencies) { }
	}
}