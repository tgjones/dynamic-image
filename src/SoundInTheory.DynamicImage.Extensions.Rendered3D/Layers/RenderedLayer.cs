using System.Collections.Generic;
using System.Windows.Media.Imaging;
using DotWarp;
using Meshellator;
using Nexus.Graphics;
using SoundInTheory.DynamicImage.Caching;
using SoundInTheory.DynamicImage.Util;

namespace SoundInTheory.DynamicImage.Layers
{
	public class RenderedLayer : Layer
	{
		#region Properties

		public SceneSource Source
		{
			get { return (SceneSource)this["Source"]; }
			set { this["Source"] = value; }
		}

		/// <summary>
		/// Width of the layer
		/// </summary>
		public int Width
		{
			get { return (int)(this["Width"] ?? 400); }
			set { this["Width"] = value; }
		}

		/// <summary>
		/// Height of the layer
		/// </summary>
		public int Height
		{
			get { return (int)(this["Height"] ?? 300); }
			set { this["Height"] = value; }
		}

		public bool ReverseWindingOrder
		{
			get { return (bool)(this["ReverseWindingOrder"] ?? false); }
			set { this["ReverseWindingOrder"] = value; }
		}

		public Color BackgroundColour
		{
			get { return (Color)(this["BackgroundColor"] ?? Colors.Transparent); }
			set { this["BackgroundColor"] = value; }
		}

		public bool LightingEnabled
		{
			get { return (bool)(this["LightingEnabled"] ?? true); }
			set { this["LightingEnabled"] = value; }
		}

		/// <summary>
		/// Shortcut route to Source/FileMeshSource
		/// </summary>
		public string SourceFileName
		{
			set { Source = new FileSceneSource { FileName = value }; }
		}

		public Camera Camera
		{
			get { return (Camera) this["Camera"]; }
			set { this["Camera"] = value; }
		}

		public override bool HasFixedSize
		{
			get { return true; }
		}

		#endregion

        protected override void CreateImage()
		{
			Scene scene = Source.GetScene();
			using (var renderer = new WarpSceneRenderer(scene, Width, Height))
			{
				renderer.Initialize();

				renderer.Options.TriangleWindingOrderReversed = ReverseWindingOrder;
				renderer.Options.BackgroundColor = new Nexus.Color(BackgroundColour.A, BackgroundColour.R, BackgroundColour.G, BackgroundColour.B);
				renderer.Options.LightingEnabled = LightingEnabled;

				Viewport viewport = new Viewport(0, 0, Width, Height);
				Nexus.Graphics.Cameras.Camera camera = (Camera != null)
					? Camera.GetNexusCamera(scene, viewport)
					: new AutoCamera().GetNexusCamera(scene, viewport);
				BitmapSource renderedBitmap = renderer.Render(camera);
				Bitmap = new FastBitmap(renderedBitmap);
			}
		}

		public override void PopulateDependencies(List<Dependency> dependencies)
		{
			base.PopulateDependencies(dependencies);
			Source.PopulateDependencies(dependencies);
		}
	}
}