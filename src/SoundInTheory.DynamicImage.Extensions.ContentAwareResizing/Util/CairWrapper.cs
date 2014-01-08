using System;
using System.Diagnostics;
using System.IO;
using System.Web;
using SoundInTheory.DynamicImage.Filters;
using SoundInTheory.DynamicImage.Configuration;

namespace SoundInTheory.DynamicImage.Util
{
	/// <summary>
	/// Thanks to http://sites.google.com/site/brainrecall/cair.
	/// </summary>
	internal class CairWrapper
	{
		private static string GetCairFolder()
		{
            string folder = (string.IsNullOrEmpty(DynamicImageSettings.ServerPath))
				? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CAIR")
                : FileSourceHelper.FilePathOnServer("~/App_Data/CAIR");
			if (!Directory.Exists(folder))
				Directory.CreateDirectory(folder);
			return folder;
		}

		private static string CairPath
		{
			get { return Path.Combine(GetCairFolder(), "CAIR.exe"); }
		}

		public string CairDefaultArguments { get; set; }

		public CairWrapper()
		{
			// Extract CAIR.exe and pthreadVSE2.dll (stored as embedded resources in this DLL)
			if (!File.Exists(CairPath))
			{
				using (var stream = typeof(CairWrapper).Assembly.GetManifestResourceStream("SoundInTheory.DynamicImage.Resources.CAIR.exe"))
				using (var fileStream = File.OpenWrite(CairPath))
					stream.CopyTo(fileStream);

				using (var stream = typeof(CairWrapper).Assembly.GetManifestResourceStream("SoundInTheory.DynamicImage.Resources.pthreadVSE2.dll"))
				using (var fileStream = File.OpenWrite(Path.Combine(GetCairFolder(), "pthreadVSE2.dll")))
					stream.CopyTo(fileStream);
			}
		}

		public bool ProcessImage(string sourceFile, string destinationFile, int timeout, int width, int height,
			ContentAwareResizeFilterConvolutionType convolutionType)
		{
			string runArguments = " -I \"" + sourceFile + "\" -O \"" + destinationFile + "\" -T 1 -C " + convolutionType;
			runArguments += " -X " + width;
			runArguments += " -Y " + height;

			var info = new ProcessStartInfo(CairPath, runArguments)
			{
				UseShellExecute = false,
				RedirectStandardInput = true,
				RedirectStandardError = true,
				RedirectStandardOutput = true,
				CreateNoWindow = true
			};
			using (Process scr = Process.Start(info))
			{
				bool result = scr.WaitForExit(timeout);
				if (!result)
					scr.Kill();
				return result;
			}
		}
	}
}