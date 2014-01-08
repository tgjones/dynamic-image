using SoundInTheory.DynamicImage.Configuration;
using SoundInTheory.DynamicImage.Util;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace SoundInTheory.DynamicImage.Caching
{
	public class XmlCacheProvider : DiskCacheProviderBase, IDisposable
	{
		private string _docPath;
		private FileSystemWatcher _watcher;
		private XDocument _doc;

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);

            _docPath = FileSourceHelper.FilePathOnServer(string.Format("{0}/DynamicImageCache.xml", CachePath));
			EnsureDocument();

			_watcher = new FileSystemWatcher(Path.GetDirectoryName(_docPath), "DynamicImageCache.xml");
			_watcher.Deleted += (sender, e) => _doc = null;
		}

		void IDisposable.Dispose()
		{
			if (_watcher != null)
				_watcher.Dispose();
		}

        private void EnsureDocument()
		{
			lock (this)
			{
                string imageCacheFolder = FileSourceHelper.FilePathOnServer(CachePath);
				if (!Directory.Exists(imageCacheFolder))
					Directory.CreateDirectory(imageCacheFolder);

				if (!File.Exists(_docPath))
				{
					string xml = new XElement("cache").ToString();
					using (var stream = File.CreateText(_docPath))
						stream.Write(xml);
				}

				if (_doc == null)
					_doc = XDocument.Load(_docPath);
			}
		}

		public override bool ExistsInCache(string cacheKey)
		{
			EnsureDocument();
			return _doc.Root.Elements().Any(e => e.Attribute("id").Value == cacheKey);
		}

		public override void AddToCache(string cacheKey, GeneratedImage generatedImage, Dependency[] dependencies)
		{
			EnsureDocument();

			// Save image to disk.
			SaveImageToDiskCache(cacheKey, generatedImage);

			lock (_doc)
			{
				// Double-check that item hasn't been added to cache since we checked.
				if (ExistsInCache(cacheKey))
					return;

				var itemElement = new XElement("item",
					new XAttribute("id", cacheKey),
					new XAttribute("isImagePresent", generatedImage.Properties.IsImagePresent),
					new XAttribute("format", generatedImage.Properties.Format),
					new XAttribute("colorDepth", generatedImage.Properties.ColorDepth));
				if (generatedImage.Properties.Width != null)
					itemElement.Add(new XAttribute("width", generatedImage.Properties.Width.Value));
				if (generatedImage.Properties.Height != null)
					itemElement.Add(new XAttribute("height", generatedImage.Properties.Height.Value));
				if (generatedImage.Properties.JpegCompressionLevel != null)
					itemElement.Add(new XAttribute("jpegCompressionLevel", generatedImage.Properties.JpegCompressionLevel.Value));

				XElement dependenciesElement = new XElement("dependencies");
				itemElement.Add(dependenciesElement);

				foreach (var dependency in dependencies)
					dependenciesElement.Add(new XElement("dependency",
						new XAttribute("text1", dependency.Text1 ?? string.Empty),
						new XAttribute("text2", dependency.Text2 ?? string.Empty),
						new XAttribute("text3", dependency.Text3 ?? string.Empty),
						new XAttribute("text4", dependency.Text4 ?? string.Empty)));

				_doc.Root.Add(itemElement);
				_doc.Save(_docPath);
			}
		}

        public override ImageProperties GetPropertiesFromCache(string cacheKey)
		{
			EnsureDocument();

			lock (_doc)
			{
				ImageProperties result = null;

				XElement itemElement = _doc.Root.Elements().SingleOrDefault(e => e.Attribute("id").Value == cacheKey);
				if (itemElement != null)
					result = GetImageProperties(itemElement);

				return result;
			}
		}

		private static ImageProperties GetImageProperties(XElement itemElement)
		{
			string cacheKey;
			ImageProperties result;
			GetImageProperties(itemElement, out cacheKey, out result);
			return result;
		}

		private static void GetImageProperties(XElement itemElement, out string cacheKey, out ImageProperties imageProperties)
		{
			imageProperties = new ImageProperties();
			imageProperties.IsImagePresent = Convert.ToBoolean(itemElement.Attribute("isImagePresent").Value);
			imageProperties.Format = (DynamicImageFormat)Enum.Parse(typeof(DynamicImageFormat), itemElement.Attribute("format").Value);
			if (itemElement.Attribute("width") != null)
				imageProperties.Width = Convert.ToInt32(itemElement.Attribute("width").Value);
			if (itemElement.Attribute("height") != null)
				imageProperties.Height = Convert.ToInt32(itemElement.Attribute("height").Value);
			imageProperties.ColorDepth = Convert.ToInt32(itemElement.Attribute("colorDepth").Value);
			if (itemElement.Attribute("jpegCompressionLevel") != null)
				imageProperties.JpegCompressionLevel = Convert.ToInt32(itemElement.Attribute("jpegCompressionLevel").Value);
			cacheKey = itemElement.Attribute("id").Value;
		}

		public override void RemoveAllFromCache()
		{
			EnsureDocument();

			lock (_doc)
			{
				foreach (XElement itemElement in _doc.Root.Elements())
				{
					string cacheKey;
					ImageProperties imageProperties;
					GetImageProperties(itemElement, out cacheKey, out imageProperties);
					DeleteImageFromDiskCache(cacheKey, imageProperties);
					itemElement.Remove();
				}
				_doc.Save(_docPath);
			}
        }

		public override void RemoveFromCache(Dependency dependency)
		{
			EnsureDocument();

			lock (_doc)
			{
				Func<XElement, bool> attributeMatcher = d => d.Attribute("text1").Value == dependency.Text1
					&& d.Attribute("text2").Value == dependency.Text2
						&& d.Attribute("text3").Value == dependency.Text3
							&& d.Attribute("text4").Value == dependency.Text4;

				var items = _doc.Root.Elements().Where(e => e.Element("dependencies").Elements().Any(attributeMatcher)).ToList();
				foreach (XElement itemElement in items)
				{
					string cacheKey;
					ImageProperties imageProperties;
					GetImageProperties(itemElement, out cacheKey, out imageProperties);
					DeleteImageFromDiskCache(cacheKey, imageProperties);
					itemElement.Remove();
				}

				_doc.Save(_docPath);
			}
		}
	}
}