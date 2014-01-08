using SoundInTheory.DynamicImage.Util;
using SWMColor = System.Windows.Media.Color;

namespace SoundInTheory.DynamicImage.Layers
{
	/// <summary>
	/// The base class for fractal layers.
	/// </summary>
	/// <see cref="http://student.kuleuven.be/~m0216922/CG/juliamandelbrot.html"/>
	public abstract class FractalLayer : Layer
	{
		#region Properties

		/// <summary>
		/// Width of the layer.
		/// </summary>
		public int Width
		{
			get { return (int)(this["Width"] ?? 400); }
			set { this["Width"] = value; }
		}

		/// <summary>
		/// Height of the layer.
		/// </summary>
		public int Height
		{
			get { return (int)(this["Height"] ?? 300); }
			set { this["Height"] = value; }
		}

		public int MaxIterations
		{
			get { return (int)(this["MaxIterations"] ?? 300); }
			set { this["MaxIterations"] = value; }
		}

		public float Zoom
		{
			get { return (float)(this["Zoom"] ?? 1.0f); }
			set { this["Zoom"] = value; }
		}

		public virtual float OffsetX
		{
			get { return (float)(this["OffsetX"] ?? 0.0f); }
			set { this["OffsetX"] = value; }
		}

		public virtual float OffsetY
		{
			get { return (float)(this["OffsetY"] ?? 0.0f); }
			set { this["OffsetY"] = value; }
		}

		public override bool HasFixedSize
		{
			get { return true; }
		}

		#endregion

        protected override void CreateImage()
		{
			Bitmap = new FastBitmap(Width, Height);
			Bitmap.Lock();

			for (int y = 0; y < Height; y++)
				for (int x = 0; x < Width; x++)
				{
					ColorHsv colorHsv = CalculateFractalColor(x, y);
					var color = (Color) colorHsv;
					Bitmap[x, y] = color.ToWpfColor();
				}

			Bitmap.Unlock();
		}

		internal abstract ColorHsv CalculateFractalColor(int x, int y);
	}
}
