using System.Configuration;
using System.Windows.Media.Imaging;
using SoundInTheory.DynamicImage.Caching;
using SoundInTheory.DynamicImage.Configuration;
using System.Web;

namespace SoundInTheory.DynamicImage.Fluent
{
	public class CompositionBuilder
	{
		private readonly Composition _composition;

		public Composition Composition
		{
			get { return _composition; }
		}

        public CompositionBuilder()
        {
            _composition = new Composition();

            var config = (DynamicImageSection)ConfigurationManager.GetSection("soundInTheory/dynamicImage");
            if (config != null)
                _composition.ImageFormat = config.DefaultImageFormat;
        }

		public string Url
		{
			get { return ImageUrlGenerator.GetImageUrl(_composition); }
		}

		public BitmapSource GetImage()
		{
			return _composition.GenerateImage().Image;
		}

		public void SaveTo(string path)
		{
			_composition.GenerateImage().Save(path);
		}

		public CompositionBuilder Width(int width)
		{
			_composition.Width = width;
			_composition.AutoSize = false;
			return this;
		}

		public CompositionBuilder Height(int height)
		{
			_composition.Height = height;
			_composition.AutoSize = false;
			return this;
		}

		public CompositionBuilder ImageFormat(DynamicImageFormat imageFormat)
		{
			_composition.ImageFormat = imageFormat;
			return this;
		}

		public CompositionBuilder FillBackgroundColor(Color backgroundColor)
		{
			_composition.Fill.BackgroundColor = backgroundColor;
			return this;
		}

		public CompositionBuilder FillType(FillType type)
		{
			_composition.Fill.Type = type;
			return this;
		}

		public CompositionBuilder FillGradientColorStart(Color color)
		{
			_composition.Fill.GradientColorStart = color;
			return this;
		}

		public CompositionBuilder FillGradientColorEnd(Color color)
		{
			_composition.Fill.GradientColorEnd = color;
			return this;
		}

		public CompositionBuilder WithLayer(LayerBuilder layerBuilder)
		{
			_composition.Layers.Add(layerBuilder.ToLayer());
			return this;
		}

		public CompositionBuilder WithGlobalFilter(FilterBuilder filterBuilder)
		{
			_composition.Filters.Add(filterBuilder.ToFilter());
			return this;
		}
	}
}