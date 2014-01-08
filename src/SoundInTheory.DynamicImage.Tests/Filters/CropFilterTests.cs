using System;
using SoundInTheory.DynamicImage.Filters;
using SoundInTheory.DynamicImage.Util;
using NUnit.Framework;

namespace SoundInTheory.DynamicImage.Tests.Filters
{
	[TestFixture]
	public class CropFilterTests
	{
		[Test]
		public void CropFilter_UseXYWidthHeight_CalculatedCorrectly()
		{
			FastBitmap bitmap = new FastBitmap("Resources/Tulips.png", UriKind.Relative);

			CropFilter cropFilter = new CropFilter();
			cropFilter.X = 30;
			cropFilter.Y = 30;
			cropFilter.Width = 200;
			cropFilter.Height = 200;

			cropFilter.ApplyFilter(bitmap);

			Assert.AreEqual(200, bitmap.Width);
			Assert.AreEqual(200, bitmap.Height);

			bitmap.Save("TulipsCropped200x200.png");

			FastBitmap expectedBitmap = new FastBitmap("Resources/TulipsCropped200x200.png", UriKind.Relative);
			FastBitmapTestUtility.AssertEqual(expectedBitmap, bitmap);
		}
	}
}