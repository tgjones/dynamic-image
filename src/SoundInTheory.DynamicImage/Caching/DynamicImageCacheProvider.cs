using System;
using System.Configuration.Provider;
using System.Web;

namespace SoundInTheory.DynamicImage.Caching
{
	public abstract class DynamicImageCacheProvider : ProviderBase
	{
		public abstract bool ExistsInCache(string cacheKey);
		public abstract void AddToCache(string cacheKey, GeneratedImage generatedImage, Dependency[] dependencies);
		public abstract ImageProperties GetPropertiesFromCache(string cacheKey);
		public abstract void RemoveAllFromCache();
		public abstract void RemoveFromCache(Dependency dependency);

		public abstract DateTime GetImageLastModifiedDate(HttpContext context, string cacheKey, string fileExtension);
		public abstract void SendImageToHttpResponse(HttpContext context, string cacheKey, string fileExtension);

    }
}
