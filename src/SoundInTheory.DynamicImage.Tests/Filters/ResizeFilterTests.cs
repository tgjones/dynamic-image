using System;
using SoundInTheory.DynamicImage.Filters;
using SoundInTheory.DynamicImage.Util;
using NUnit.Framework;

namespace SoundInTheory.DynamicImage.Tests.Filters
{
	[TestFixture]
	public class ResizeFilterTests
	{
		[Test]
		public void ResizeFilterInUseWidthModeCalculatesDimensionsCorrectly()
		{
			// Arrange.
			var bitmap = new FastBitmap(@"Resources\Tulips.png", UriKind.Relative);
			var resizeFilter = new ResizeFilter
			{
				Mode = ResizeMode.UseWidth,
				Width = Unit.Pixel(200)
			};

			// Act.
			resizeFilter.ApplyFilter(bitmap);

			// Assert.
			Assert.AreEqual(200, bitmap.Width);
			Assert.AreEqual(133, bitmap.Height);
		}

		[Test]
		public void ResizeFilterInUseHeightModeCalculatesDimensionsCorrectly()
		{
			// Arrange.
			var bitmap = new FastBitmap(@"Resources\Tulips.png", UriKind.Relative);
			var resizeFilter = new ResizeFilter
			{
				Mode = ResizeMode.UseHeight,
				Height = Unit.Pixel(200)
			};

			// Act.
            resizeFilter.ApplyFilter(bitmap);

			// Assert.
			Assert.AreEqual(300, bitmap.Width);
			Assert.AreEqual(200, bitmap.Height);
		}

		[Test]
		public void ResizeFilterInFillModeCalculatesDimensionsCorrectly()
		{
			// Arrange.
			var bitmap = new FastBitmap(@"Resources\Tulips.png", UriKind.Relative);
			var resizeFilter = new ResizeFilter
			{
				Mode = ResizeMode.Fill,
				Width = Unit.Pixel(250),
				Height = Unit.Pixel(249)
			};

			// Act.
            resizeFilter.ApplyFilter(bitmap);

			// Assert.
			Assert.AreEqual(250, bitmap.Width);
			Assert.AreEqual(249, bitmap.Height);
		}

		[Test]
		public void ResizeFilterInUniformModeWithDominantWidthCalculatesDimensionsCorrectly()
		{
			// Arrange.
			var bitmap = new FastBitmap(@"Resources\Tulips.png", UriKind.Relative);
			var resizeFilter = new ResizeFilter
			{
				Mode = ResizeMode.Uniform,
				Width = Unit.Pixel(200),
				Height = Unit.Pixel(200)
			};

			// Act.
            resizeFilter.ApplyFilter(bitmap);

			// Assert.
			Assert.AreEqual(200, bitmap.Width);
			Assert.AreEqual(133, bitmap.Height);
		}

		[Test]
		public void ResizeFilterInUseWidthModeWithDominantHeightCalculatesDimensionsCorrectly()
		{
			// Arrange.
			var bitmap = new FastBitmap(@"Resources\Tulips.png", UriKind.Relative);
			var resizeFilter = new ResizeFilter
			{
				Mode = ResizeMode.Uniform,
				Width = Unit.Pixel(200),
				Height = Unit.Pixel(100)
			};

			// Act.
            resizeFilter.ApplyFilter(bitmap);

			// Assert.
			Assert.AreEqual(150, bitmap.Width);
			Assert.AreEqual(100, bitmap.Height);
		}

		[Test]
		public void ResizeFilterInUniformFillModeCalculatesDimensionsCorrectly()
		{
			// Arrange.
			var bitmap = new FastBitmap(@"Resources\Tulips.png", UriKind.Relative);
			var resizeFilter = new ResizeFilter
			{
				Mode = ResizeMode.UniformFill,
				Width = Unit.Pixel(250),
				Height = Unit.Pixel(250)
			};

			// Act.
            resizeFilter.ApplyFilter(bitmap);

			// Assert.
			Assert.AreEqual(250, bitmap.Width);
			Assert.AreEqual(250, bitmap.Height);
		}
	}
}