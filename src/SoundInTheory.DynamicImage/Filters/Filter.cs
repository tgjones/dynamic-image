using SoundInTheory.DynamicImage.Util;

namespace SoundInTheory.DynamicImage.Filters
{
	/// <summary>
	/// Provides the abstract base class for filters.
	/// </summary>
	public abstract class Filter : DirtyTrackingObject
	{
		public bool Enabled
		{
			get { return (bool)(this["Enabled"] ?? true); }
			set { this["Enabled"] = value; }
		}

	    /// <summary>
	    /// When overridden in a derived class, applies the filter algorithm to
	    /// the specified image.
	    /// </summary>
	    /// <param name="context"></param>
	    /// <param name="bitmap">Image to apply the 
	    /// <see cref="SoundInTheory.DynamicImage.Filters.Filter" /> to.</param>
        public abstract void ApplyFilter(FastBitmap bitmap);
	}
}
