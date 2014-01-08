using System.Linq;
using System.Web;

namespace SoundInTheory.DynamicImage.Caching
{
	public static class ImageUrlGenerator
	{
        public static string GetImageUrl(Composition composition)
		{
			string cacheKey = composition.GetCacheKey();

			if (DynamicImageCacheManager.Exists(cacheKey))
				return GetUrl(cacheKey, DynamicImageCacheManager.GetProperties(cacheKey));

			GeneratedImage generatedImage = composition.GenerateImage();
			if (generatedImage.Properties.IsImagePresent || DynamicImageCacheManager.StoreMissingImagesInCache)
			{
				Dependency[] dependencies = composition.GetDependencies().Distinct().ToArray();
				DynamicImageCacheManager.Add(cacheKey, generatedImage, dependencies);
			}
			return GetUrl(cacheKey, generatedImage.Properties);
		}

		private static string GetUrl(string cacheKey, ImageProperties imageProperties)
		{
			const string path = "~/Assets/Images/DynamicImages/";
			string fileName = string.Format("{0}.{1}", cacheKey, imageProperties.FileExtension);
			return VirtualPathUtility.ToAbsolute(path) + fileName;
		}
	}
}