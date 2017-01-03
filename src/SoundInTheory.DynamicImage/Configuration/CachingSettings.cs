using System;
using System.Configuration;
using SoundInTheory.DynamicImage.Caching;

namespace SoundInTheory.DynamicImage.Configuration
{
	internal class CachingSettings : ConfigurationElement
	{
		[ConfigurationProperty("mode")]
		public DynamicImageCachingMode Mode
		{
			get { return (DynamicImageCachingMode) this["mode"]; }
			set { this["mode"] = value; }
		}

		[ConfigurationProperty("customProvider")]
		public string CustomProvider
		{
			get { return this["customProvider"] as string; }
			set { this["customProvider"] = value; }
		}

		[ConfigurationProperty("cachePath")]
	        public string cachePath
	        {
	            get { return this["cachePath"] as string; }
	            set { this["cachePath"] = value; }
	        }
	        
		[ConfigurationProperty("storeMissingImagesInCache", DefaultValue = true)]
		public bool StoreMissingImagesInCache
		{
			get { return (bool) this["storeMissingImagesInCache"]; }
			set { this["storeMissingImagesInCache"] = value; }
		}

		[ConfigurationProperty("providers")]
		public ProviderSettingsCollection Providers
		{
			get { return (ProviderSettingsCollection) base["providers"]; }
		}
	}
}
