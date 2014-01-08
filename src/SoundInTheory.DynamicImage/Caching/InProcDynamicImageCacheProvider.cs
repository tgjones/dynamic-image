using System;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Collections;

namespace SoundInTheory.DynamicImage.Caching
{
	internal class InProcDynamicImageCacheProvider : DynamicImageCacheProvider
	{
		private Cache _cache;

		public Cache Cache
		{
			get { return _cache ?? (_cache = HttpRuntime.Cache); }
		}

        public override bool ExistsInCache(string cacheKey)
		{
			return (Cache.Get(cacheKey) != null);
		}

        public override void AddToCache(string cacheKey, GeneratedImage generatedImage, Dependency[] dependencies)
		{
			InProcCacheEntry cacheEntry = new InProcCacheEntry
			{
				GeneratedImage = generatedImage,
				Dependencies = dependencies
			};
			Cache.Insert(cacheKey, cacheEntry);
		}

        public override ImageProperties GetPropertiesFromCache(string cacheKey)
		{
			return ((InProcCacheEntry)Cache.Get(cacheKey)).GeneratedImage.Properties;
		}

		public override void RemoveAllFromCache()
		{
			Cache.Cast<DictionaryEntry>().Where(de => de.Value is InProcCacheEntry).ToList()
				.ForEach(de => Cache.Remove((string) de.Key));
		}

        public override void RemoveFromCache(Dependency dependency)
		{
			foreach (var dictionaryEntry in Cache.Cast<DictionaryEntry>().Where(de => de.Value is InProcCacheEntry))
			{
				InProcCacheEntry cacheEntry = (InProcCacheEntry) dictionaryEntry.Value;
				if (cacheEntry.Dependencies.Contains(dependency))
					Cache.Remove((string) dictionaryEntry.Key);
			}
		}

		public override DateTime GetImageLastModifiedDate(string cacheKey, string fileExtension)
		{
			return DateTime.Now;
		}

		protected InProcCacheEntry GetCacheEntry(string cacheProviderKey)
		{
			foreach (var dictionaryEntry in Cache.Cast<DictionaryEntry>().Where(de => de.Value is InProcCacheEntry))
				if (((string)dictionaryEntry.Key) == cacheProviderKey)
					return (InProcCacheEntry)dictionaryEntry.Value;
			throw new InvalidOperationException("Could not find image in cache");
		}

		public override void SendImageToHttpResponse(HttpContext context, string cacheKey, string fileExtension)
		{
			InProcCacheEntry cacheEntry = GetCacheEntry(cacheKey);
			Util.Util.SendImageToHttpResponse(context, cacheEntry.GeneratedImage);
		}

		internal class InProcCacheEntry
		{
			public GeneratedImage GeneratedImage;
			public Dependency[] Dependencies;
		}
	}
}
