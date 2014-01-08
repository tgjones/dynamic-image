using System.Web;

namespace SoundInTheory.DynamicImage.Caching
{
	/// <summary>
	/// This is how the "no caching" option is implemented.
	/// </summary>
	internal class TransientCacheProvider : InProcDynamicImageCacheProvider
	{
        public override bool ExistsInCache(string cacheKey)
		{
			return false;
		}

		public override void SendImageToHttpResponse(HttpContext context, string cacheKey, string fileExtension)
		{
			base.SendImageToHttpResponse(context, cacheKey, fileExtension);
			Cache.Remove(cacheKey);
		}
	}
}