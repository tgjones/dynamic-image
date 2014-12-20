using System;
using System.ComponentModel;
using System.Net;
using System.Web.UI;
using SoundInTheory.DynamicImage.Util;
using SoundInTheory.DynamicImage.Caching;
using System.Collections.Generic;

namespace SoundInTheory.DynamicImage.Sources
{
    public class RemoteImageSource : ImageSource
    {
        [Category("Source"), Browsable(true), UrlProperty]
        public string Url
        {
            get { return (string) (this["Url"] ?? string.Empty); }
            set { this["Url"] = value; }
        }

        public int Timeout
        {
            get { return (int) (this["Timeout"] ?? 10000); }
            set { this["Timeout"] = value; }
        }

        public override FastBitmap GetBitmap(ImageGenerationContext context)
        {
            using (var webClient = new ImpatientWebClient(Timeout))
            {
                var bytes = webClient.DownloadData(Url);
                return new FastBitmap(bytes);
            }
        }

        public override void PopulateDependencies(List<Dependency> dependencies)
        {
            Dependency dependency = new Dependency();
            dependency.Text1 = Url;
            dependencies.Add(dependency);
        }

        private class ImpatientWebClient : WebClient
        {
            private readonly int _timeout;

            public ImpatientWebClient(int timeout)
            {
                _timeout = timeout;
            }

            protected override WebRequest GetWebRequest(Uri address)
            {
                var result = base.GetWebRequest(address);
                if (result != null)
                    result.Timeout = _timeout;
                return result;
            }
        }
    }
}