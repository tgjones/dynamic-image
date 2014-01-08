using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SoundInTheory.DynamicImage.Caching;
using SoundInTheory.DynamicImage.Filters;
using SoundInTheory.DynamicImage.Util;
using SWMColor = System.Windows.Media.Color;

namespace SoundInTheory.DynamicImage
{
	public class Composition : DirtyTrackingObject
	{
		#region Properties

		public bool AutoSize
		{
			get { return (bool)(this["AutoSize"] ?? true); }
			set { this["AutoSize"] = value; }
		}

		public int? Width
		{
			get { return (int?) this["Width"]; }
			set { this["Width"] = value; }
		}

		public int? Height
		{
			get { return (int?)this["Height"]; }
			set { this["Height"] = value; }
		}

		public DynamicImageFormat ImageFormat
		{
			get { return (DynamicImageFormat)(this["ImageFormat"] ?? DynamicImageFormat.Jpeg); }
			set { this["ImageFormat"] = value; }
		}

		public int? JpegCompressionLevel
		{
			get { return (int?)(this["JpegCompressionLevel"] ?? 90); }
			set { this["JpegCompressionLevel"] = value; }
		}

		public int ColorDepth
		{
			get { return (int)(this["ColorDepth"] ?? 32); }
			set { this["ColorDepth"] = value; }
		}

		public Fill Fill
		{
			get { return (Fill)(this["Fill"] ?? (this["Fill"] = new Fill())); }
			set { this["Fill"] = value; }
		}

		public LayerCollection Layers
		{
			get { return (LayerCollection)(this["Layers"] ?? (this["Layers"] = new LayerCollection())); }
			set { this["Layers"] = value; }
		}

		private IEnumerable<Layer> VisibleLayers
		{
			get { return Layers.Where(l => l.Visible); }
		}

		public FilterCollection Filters
		{
			get { return (FilterCollection)(this["Filters"] ?? (this["Filters"] = new FilterCollection())); }
			set { this["Filters"] = value; }
		}

		#endregion

        public GeneratedImage GenerateImage()
	    {
	        return GenerateImageAsync().Result;
	    }

        public Task<GeneratedImage> GenerateImageAsync()
	    {
            return new TaskFactory(DispatcherTaskScheduler.Default)
                .StartNew(() =>
                {
                    var image = CreateImage();

                    var properties = new ImageProperties
                    {
                        ColorDepth = ColorDepth,
                        Format = ImageFormat,
                        JpegCompressionLevel = JpegCompressionLevel
                    };
                    if (image != null)
                    {
                        image.Freeze();
                        properties.Width = image.PixelWidth;
                        properties.Height = image.PixelHeight;
                    }
                    properties.IsImagePresent = (image != null);
                    return new GeneratedImage(properties, image);
                });
	    }

        private BitmapSource CreateImage()
		{
			ValidateParameters();

			// First, we process layers which have a specific size.
			foreach (Layer layer in VisibleLayers)
				if (layer.HasFixedSize)
					layer.Process();

			// Second, for SizeMode = Auto, we calculate the output dimensions
			// based on the union of all layers' (which have an explicit size) dimensions.
			int outputWidth, outputHeight;
			if (AutoSize)
			{
				Int32Rect outputDimensions = Int32Rect.Empty;
				foreach (Layer layer in VisibleLayers)
				{
					// Calculate dimensions of layers; the dimensions of some layers may be omitted
					// at design time and are then derived from layers which have a fixed size.
					// Only include layer in bounds calculation if the Anchor property is None.
					if (layer.Anchor == AnchorStyles.None)
					{
						Int32Rect? bounds = layer.Bounds;
						if (bounds != null)
							outputDimensions = Int32RectUtility.Union(outputDimensions, bounds.Value);
					}
				}
				outputWidth = outputDimensions.Width;
				outputHeight = outputDimensions.Height;
			}
			else
			{
				outputWidth = Width.Value;
				outputHeight = Height.Value;
			}

			// If at this point we don't have a valid size, return - this means that
			// layers without an explicit size will never force an image to be created.
			if (outputWidth == 0 && outputHeight == 0)
				return null;

			// Second, layers which don't have explicit sizes set, we now set their sizes
			// based on the overall composition size, and process the layer.
			foreach (Layer layer in this.VisibleLayers)
				if (!layer.HasFixedSize)
				{
					layer.CalculatedWidth = outputWidth;
					layer.CalculatedHeight = outputHeight;

					layer.Process();
				}

			// If any of the layers are not present, we don't create the image
			foreach (Layer layer in this.VisibleLayers)
				if (layer.Bitmap == null)
					return null;
		
			// Calculate X and Y for layers which are anchored.
			foreach (Layer layer in this.VisibleLayers)
			{
				if (layer.Bitmap != null && layer.Anchor != AnchorStyles.None)
				{
					// Set X.
					switch (layer.Anchor)
					{
						case AnchorStyles.BottomCenter :
						case AnchorStyles.MiddleCenter :
						case AnchorStyles.TopCenter :
							layer.X = (outputWidth - layer.Size.Value.Width) / 2;
							break;
						case AnchorStyles.BottomLeft :
						case AnchorStyles.MiddleLeft :
						case AnchorStyles.TopLeft :
							layer.X = layer.AnchorPadding;
							break;
						case AnchorStyles.BottomRight:
						case AnchorStyles.MiddleRight:
						case AnchorStyles.TopRight:
							layer.X = outputWidth - layer.Size.Value.Width - layer.AnchorPadding;
							break;
					}

					// Set Y.
					switch (layer.Anchor)
					{
						case AnchorStyles.BottomCenter:
						case AnchorStyles.BottomLeft:
						case AnchorStyles.BottomRight:
							layer.Y = outputHeight - layer.Size.Value.Height - layer.AnchorPadding;
							break;
						case AnchorStyles.MiddleCenter:
						case AnchorStyles.MiddleLeft:
						case AnchorStyles.MiddleRight:
							layer.Y = (outputHeight - layer.Size.Value.Height) / 2;
							break;
						case AnchorStyles.TopCenter:
						case AnchorStyles.TopLeft:
						case AnchorStyles.TopRight:
							layer.Y = layer.AnchorPadding;
							break;
					}
				}
			}

			// Apply fill.
			DrawingVisual dv = new DrawingVisual();
			DrawingContext dc = dv.RenderOpen();

			// Apply fill.
			Fill.Apply(dc, new Rect(0, 0, outputWidth, outputHeight));

			dc.Close();

			// create output bitmap and lock data
			RenderTargetBitmap rtb = RenderTargetBitmapUtility.CreateRenderTargetBitmap(outputWidth, outputHeight);
			rtb.Render(dv);
			FastBitmap output = new FastBitmap(rtb);

			// Blend layers using specified blend mode.
			output = LayerBlender.BlendLayers(output, VisibleLayers);

			// Apply global filters.
			foreach (Filter filter in Filters)
				if (filter.Enabled)
					filter.ApplyFilter(output);

			// If image format doesn't support transparency, make all transparent pixels totally opaque.
			// Otherwise WPF wants to save them as black.
			if (ImageFormat == DynamicImageFormat.Bmp || ImageFormat == DynamicImageFormat.Jpeg)
			{
				output.Lock();
				for (int y = 0; y < output.Height; ++y)
					for (int x = 0; x < output.Width; ++x)
					{
						var c = output[x, y];
						//if (output[x, y].A == 0 && output[x, y].R == 0 && output[x, y].G == 0 && output[x, y].B == 0)
						output[x, y] = SWMColor.FromArgb(255, c.R, c.G, c.B);
					}
				output.Unlock();
			}

			// Freeze the bitmap - this means any thread can access it, as well
			// as perhaps providing some performance benefit.
			output.InnerBitmap.Freeze();
			return output.InnerBitmap;
		}

		private void ValidateParameters()
		{
			// if SizeMode is set to Manual, we must have both a Width and a Height set
			if (!this.AutoSize)
				if (this.Width == null || this.Height == null)
					throw new InvalidOperationException("If AutoSize is false then both Width and Height must be set");

			// conversely, if SizeMode is set to Auto, we must have neither Width nor Height set
			if (this.AutoSize)
				if (this.Width != null || this.Height != null)
					throw new InvalidOperationException("If AutoSize is true then neither Width nor Height can be set");

			// If we don't have any layers, then AutoSize must be false otherwise no image will be generated.
			if (this.Layers.Count == 0 && this.AutoSize)
				throw new InvalidOperationException("If no layers are specified, then AutoSize must be set to false.");
		}

		internal Dependency[] GetDependencies()
		{
			List<Dependency> dependencies = new List<Dependency>();
			foreach (Layer layer in this.Layers)
				layer.PopulateDependencies(dependencies);
			return dependencies.ToArray();
		}

		public string GetCacheKey()
		{
			return Util.Util.CalculateShaHash(((IDirtyTrackingObject) this).GetDirtyProperties());
		}
	}
}
