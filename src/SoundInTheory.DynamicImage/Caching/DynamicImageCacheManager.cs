using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Web;
using SoundInTheory.DynamicImage.Configuration;
using System.Web.Configuration;
using System.Collections.Generic;
using SoundInTheory.DynamicImage.Sources;

namespace SoundInTheory.DynamicImage.Caching
{
	public static class DynamicImageCacheManager
	{
		private static DynamicImageCacheProvider _provider;
		private static bool? _storeMissingImagesInCache;

		private static DynamicImageSection _configSection;

		private static DynamicImageSection ConfigSection
		{
			get
			{
				if (_configSection == null)
					_configSection = (DynamicImageSection)ConfigurationManager.GetSection("soundInTheory/dynamicImage");
				return _configSection;
			}
		}

		private static DynamicImageCacheProvider Provider
		{
			get
			{
				if (_provider == null)
				{
					DynamicImageSection config = ConfigSection;
					if (config == null)
					{
						// Default to XML cache provider.
						_provider = new XmlCacheProvider();
						_provider.Initialize("XmlCacheProvider", new NameValueCollection());
					}
					else
					{
						switch (config.Caching.Mode)
						{
							case DynamicImageCachingMode.Off:
								goto default;
							case DynamicImageCachingMode.InProc:
								_provider = new InProcDynamicImageCacheProvider();
								break;
							case DynamicImageCachingMode.Custom:
								string customProvider = config.Caching.CustomProvider;
								PropertyInformation propertyInformation = config.ElementInformation.Properties["customProvider"];
								if (string.IsNullOrEmpty(customProvider))
									throw new ConfigurationErrorsException(
										"Invalid DynamicImage caching custom provider: '" + customProvider + "'",
										propertyInformation.Source,
										propertyInformation.LineNumber);
								ProviderSettings providerSettings = config.Caching.Providers[customProvider];
								if (providerSettings == null)
									throw new ConfigurationErrorsException(
										"Missing DynamicImage caching custom provider: '" + customProvider + "'",
										propertyInformation.Source,
										propertyInformation.LineNumber);
								_provider = (DynamicImageCacheProvider) ProvidersHelper.InstantiateProvider(
									providerSettings, typeof (DynamicImageCacheProvider));
								break;
							default:
								_provider = new TransientCacheProvider();
								break;
						}
					}
				}
				return _provider;
			}
		}

		internal static bool StoreMissingImagesInCache
		{
			get
			{
				if (_storeMissingImagesInCache == null)
				{
					DynamicImageSection config = ConfigSection;
					if (config == null)
						_storeMissingImagesInCache = false;
					else
						_storeMissingImagesInCache = config.Caching.StoreMissingImagesInCache;
				}
				return _storeMissingImagesInCache.Value;
			}
		}

        internal static bool Exists(string cacheKey)
		{
			return Provider.ExistsInCache(cacheKey);
		}

        internal static void Add(string cacheKey, GeneratedImage generatedImage, Dependency[] dependencies)
		{
			Provider.AddToCache(cacheKey, generatedImage, dependencies);
		}

		internal static DateTime GetImageLastModifiedDate(string cacheProviderKey, string fileExtension)
		{
			return Provider.GetImageLastModifiedDate(cacheProviderKey, fileExtension);
		}

        internal static ImageProperties GetProperties(string cacheKey)
		{
			return Provider.GetPropertiesFromCache(cacheKey);
		}

        public static void Remove(ImageSource source)
		{
			var dependencies = new List<Dependency>();
			source.PopulateDependencies(dependencies);
            dependencies.ForEach(RemoveByDependency);
		}

		public static void RemoveAll()
		{
			Provider.RemoveAllFromCache();
		}

        private static void RemoveByDependency(Dependency dependency)
		{
			Provider.RemoveFromCache(dependency);
		}

		internal static void SendImageToHttpResponse(HttpContext context, string cacheProviderKey, string fileExtension)
		{
			Provider.SendImageToHttpResponse(context, cacheProviderKey, fileExtension);
		}
	}
}
