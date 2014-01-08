using SoundInTheory.DynamicImage.Configuration;
using System;
using System.IO;

namespace SoundInTheory.DynamicImage.Util
{
	internal static class GhostscriptUtil
	{
        public static void EnsureDll()
		{
            string folder = (string.IsNullOrEmpty(DynamicImageSettings.ServerPath))
				? AppDomain.CurrentDomain.BaseDirectory
                : string.Format("{0}\bin", DynamicImageSettings.ServerPath);

			string gsDllPath = Path.Combine(folder, "gsdll.dll");

			if (!File.Exists(gsDllPath))
			{
				string gsDllFile = Environment.Is64BitProcess ? "gsdll64.dll" : "gsdll32.dll";
				using (var stream = typeof(GhostscriptUtil).Assembly.GetManifestResourceStream("SoundInTheory.DynamicImage.Resources." + gsDllFile))
				using (var fileStream = File.OpenWrite(gsDllPath))
					stream.CopyTo(fileStream);
			}
		}
	}
}