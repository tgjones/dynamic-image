using SoundInTheory.DynamicImage.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.Web;

namespace SoundInTheory.DynamicImage.Util
{
	/// <summary>
	/// Thanks to http://nolovelust.com/post/C-Website-Screenshot-Generator-AKA-Get-Screenshot-of-Webpage-With-Aspnet-C.aspx
	/// for providing the starting point for this class.
	/// </summary>
	public class CutyCaptWrapper
	{
		private static string GetCairFolder()
		{
            string folder = (string.IsNullOrEmpty(DynamicImageSettings.ServerPath))
				? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CutyCapt")
				: FileSourceHelper.FilePathOnServer("~/App_Data/CutyCapt");
			if (!Directory.Exists(folder))
				Directory.CreateDirectory(folder);
			return folder;
		}

		private static string CutyCaptPath
		{
			get { return Path.Combine(GetCairFolder(), "CutyCapt.exe"); }
		}

		public string CutyCaptDefaultArguments { get; set; }

		public CutyCaptWrapper()
		{
			if (!File.Exists(CutyCaptPath))
			{
				using (var stream = typeof(CutyCaptWrapper).Assembly.GetManifestResourceStream("SoundInTheory.DynamicImage.Resources.CutyCapt.exe"))
				using (var fileStream = File.OpenWrite(CutyCaptPath))
					stream.CopyTo(fileStream);
			}

			CutyCaptDefaultArguments = " --max-wait=0 --out-format=png --javascript=off --java=off --plugins=off --js-can-open-windows=off --js-can-access-clipboard=off --private-browsing=on";
		}

		public bool SaveScreenShot(string url, string destinationFile, int timeout, int width)
		{
			if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
				return false;

			string runArguments = " --url=" + url + " --out=" + destinationFile + CutyCaptDefaultArguments + " --min-width=" + width;

			var info = new ProcessStartInfo(CutyCaptPath, runArguments)
			{
				UseShellExecute = false,
				RedirectStandardInput = true,
				RedirectStandardError = true,
				RedirectStandardOutput = true,
				CreateNoWindow = true,
				//WorkingDirectory = CutyCaptWorkingDirectory;
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