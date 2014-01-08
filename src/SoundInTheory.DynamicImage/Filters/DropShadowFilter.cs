using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using SoundInTheory.DynamicImage.Util;

namespace SoundInTheory.DynamicImage.Filters
{
	public class DropShadowFilter : ShadowFilterBase
	{
		#region Properties

		/// <summary>
		/// Gets or sets the angle of the light source. The angle is measured counter-clockwise from the x-axis. Defaulst to 315.
		/// </summary>
		public int Angle
		{
			get { return (int)(this["Angle"] ?? 315); }
			set
			{
				if (value < 0 || value > 360)
					throw new ArgumentException("The angle must be between 0 and 360 degrees.", "value");
				this["Angle"] = value;
			}
		}

		/// <summary>
		/// Gets or sets the offset for the shadow. Defaults to 5.
		/// </summary>
		public int Distance
		{
			get { return (int)(this["Distance"] ?? 5); }
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException("value", "The offset of the shadow must be greater than or equal to zero.");
				this["Distance"] = value;
			}
		}

		#endregion

		#region Methods

		protected override Vector GetPadding()
		{
			Vector offsetSize = GetOffsetSize();
			offsetSize.X = (int) Math.Ceiling(Math.Abs(offsetSize.X));
			offsetSize.Y = (int)Math.Ceiling(Math.Abs(offsetSize.Y));
			offsetSize.X += Size;
			offsetSize.Y += Size;
			return offsetSize;
		}

		private Vector GetOffsetSize()
		{
			float radians = MathUtility.ToRadians(Angle + 180);
			Vector result = new Vector(
				Math.Cos(radians) * Distance,
				-Math.Sin(radians) * Distance);
			return result;
		}

        protected override Effect GetEffect(FastBitmap source)
		{
			return new DropShadowEffect
			{
				Direction = Angle,
				Color = Color.ToWpfColor(),
				BlurRadius = Size,
				ShadowDepth = Distance,
				Opacity = Opacity
			};
		}

		protected override void ApplyFilter(FastBitmap source, DrawingContext dc, int width, int height)
		{
			Vector offsetSize = GetOffsetSize() + new Vector(Size, Size);
			dc.PushTransform(new TranslateTransform(offsetSize.X, offsetSize.Y));
			base.ApplyFilter(source, dc, width, height);
			dc.Pop();
		}

		#endregion
	}
}
