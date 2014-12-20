using System.Configuration;

namespace SoundInTheory.DynamicImage.Configuration
{
	internal class DynamicImageSection : ConfigurationSection
	{
		[ConfigurationProperty("caching")]
		public CachingSettings Caching
		{
			get { return (CachingSettings) base["caching"]; }
		}

		[ConfigurationProperty("browserCaching")]
		public BrowserCachingSettings BrowserCaching
		{
			get { return (BrowserCachingSettings) base["browserCaching"]; }
		}

		[ConfigurationProperty("defaultImageFormat", DefaultValue = DynamicImageFormat.Jpeg)]
		public DynamicImageFormat DefaultImageFormat
		{
			get { return (DynamicImageFormat)this["defaultImageFormat"]; }
			set { this["defaultImageFormat"] = value; }
		}
        
        
        [ConfigurationProperty("baseVirtualPath", DefaultValue = "~/assets/images/dynamicimages/")]
        public string BaseVirtualPath
        {
            get { return (string)base["baseVirtualPath"]; }
            set { this["baseVirtualPath"] = value; }
        }

	}
}
