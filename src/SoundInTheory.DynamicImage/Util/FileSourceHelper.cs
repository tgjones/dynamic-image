using SoundInTheory.DynamicImage.Configuration;
using System;
using System.IO;

namespace SoundInTheory.DynamicImage.Util
{
	public static class FileSourceHelper
	{
        public static string FilePathOnServer(string filepath)
        {
            if (filepath.StartsWith("~/"))
            {
                filepath = filepath.Replace("~/", "");
            }
            filepath = filepath.Replace("/", @"\");

            return Path.Combine(DynamicImageSettings.ServerPath, filepath);
        }

		public static string ResolveFileName(string filename)
		{
			string fileName = null;
			if (!Path.IsPathRooted(filename))
			{
                fileName = FilePathOnServer(filename);

				if (fileName == null)
					throw new InvalidOperationException("Could not resolve source filename.");
			}
			else
			{
				fileName = filename;
			}

			return fileName;
		}
	}
}
