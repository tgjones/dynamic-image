using System;
using System.Configuration;
using System.Globalization;
using System.Web;
using SoundInTheory.DynamicImage.Caching;
using SoundInTheory.DynamicImage.Configuration;
using System.IO;

namespace SoundInTheory.DynamicImage
{
	///<summary>
	///</summary>
	public class DynamicImageModule : IHttpModule
	{


		public void Init(HttpApplication context)
		{
			context.PostAuthorizeRequest += OnContextPostAuthorizeRequest;
			context.PreSendRequestHeaders += OnContextPreSendRequestHeaders;
		}

		private static void OnContextPreSendRequestHeaders(object sender, EventArgs e)
		{
			HttpApplication app = sender as HttpApplication;
			if (app == null)
				return;
			SetHeaders(app.Context);
		}

		private static void SetHeaders(HttpContext context)
		{
			if (context == null || context.Response == null || context.Items["FinalCachedFile"] == null)
				return;

			if (context.Items["BrowserCachingEnabled"] == null || !((bool) context.Items["BrowserCachingEnabled"]))
				return;

			context.Response.AddFileDependency((string) context.Items["FinalCachedFile"]);

			if (context.Items["ContentExpires"] != null)
				context.Response.Cache.SetExpires((DateTime) context.Items["ContentExpires"]);

			if (context.Items["ContentETag"] != null)
			{
				// In IIS 7 Classic mode (and probably IIS6), the ETag will already have been set by this point,
				// and setting it again throws an exception.
				try
				{
					context.Response.Cache.SetETag((string) context.Items["ContentETag"]);
				}
				catch
				{
				}
			}

			// Enables in-memory caching
			context.Response.Cache.SetCacheability(HttpCacheability.Public);
			context.Response.Cache.SetLastModifiedFromFileDependencies();
			context.Response.Cache.SetValidUntilExpires(false);
		}

		private static void OnContextPostAuthorizeRequest(object sender, EventArgs e)
		{
			HttpApplication app = sender as HttpApplication;
			if (app == null || app.Context == null || app.Context.Request == null)
				return;

            DynamicImageSection config = ((DynamicImageSection)ConfigurationManager.GetSection("soundInTheory/dynamicImage")) ?? new DynamicImageSection();
			if (!string.IsNullOrEmpty(app.Context.Request.Path) && VirtualPathUtility.ToAppRelative(app.Context.Request.Path).StartsWith(config.BaseVirtualPath, StringComparison.InvariantCultureIgnoreCase))
				HandleRequest(app.Context);
		}

		private static void HandleRequest(HttpContext context)
		{
			string fileName = Path.GetFileName(context.Request.Path);
			string cacheProviderKey = Path.GetFileNameWithoutExtension(fileName);
			string fileExtension = Path.GetExtension(fileName).Substring(1);

			// Set cache headers.
			DynamicImageSection config = (DynamicImageSection) ConfigurationManager.GetSection("soundInTheory/dynamicImage");
			if (config != null && config.BrowserCaching != null && config.BrowserCaching.Enabled)
			{
				DateTime tempDate = DynamicImageCacheManager.GetImageLastModifiedDate(context, cacheProviderKey, fileExtension);
				DateTime lastModifiedDate = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, tempDate.Hour, tempDate.Minute,
					tempDate.Second, 0); // This code copied from System.Web.StaticFileHandler.ProcessRequestInternal
				DateTime now = DateTime.Now;
				if (lastModifiedDate > now)
					lastModifiedDate = new DateTime(now.Ticks - (now.Ticks % 0x989680L));

				string eTag = GenerateETag(lastModifiedDate, now);

				context.Items["BrowserCachingEnabled"] = true;
				context.Items["ContentExpires"] = now.Add(config.BrowserCaching.Duration);
				context.Items["ContentETag"] = eTag;

				// Check if we actually need to send back the image.
				if (!IsImageModified(context, lastModifiedDate, eTag))
				{
					//context.Response.SuppressContent = true;
					context.Response.StatusCode = 304;
					context.Response.StatusDescription = "Not Modified";

					// Explicitly set the Content-Length header so the client doesn't wait for
					// content but keeps the connection open for other requests
					context.Response.AddHeader("Content-Length", "0");

					SetHeaders(context);

					context.Response.End();

					return;
				}
			}

			// Save to output stream - each caching provider might have its own way of doing this.
			// For example, LinqDynamicImageCacheProvider does a context.Rewrite to the cached file on disk.
			DynamicImageCacheManager.SendImageToHttpResponse(context, cacheProviderKey, fileExtension);
		}

		private static string GenerateETag(DateTime lastModified, DateTime now)
		{
			long num = lastModified.ToFileTime();
			long num2 = now.ToFileTime();
			string str = num.ToString("X8", CultureInfo.InvariantCulture);
			if ((num2 - num) <= 0x1c9c380L)
				return ("W/\"" + str + "\"");
			return ("\"" + str + "\"");
		}

		private static bool IsImageModified(HttpContext context, DateTime responseLastModified, string responseETagHeader)
		{
			bool dateModified = false;

			string requestETagHeader = context.Request.Headers["If-None-Match"] ?? string.Empty;
			string requestIfModifiedSinceHeader = context.Request.Headers["If-Modified-Since"] ?? string.Empty;
			DateTime requestIfModifiedSince;

			if (DateTime.TryParse(requestIfModifiedSinceHeader, out requestIfModifiedSince))
			{
				if (responseLastModified > requestIfModifiedSince)
				{
					TimeSpan diff = responseLastModified - requestIfModifiedSince;
					if (diff > TimeSpan.FromSeconds(1))
						dateModified = true;
				}
			}
			else
			{
				dateModified = true;
			}

			bool eTagModified = !responseETagHeader.Equals(requestETagHeader, StringComparison.Ordinal);

			return (dateModified || eTagModified);
		}

		public void Dispose()
		{

		}
	}
}