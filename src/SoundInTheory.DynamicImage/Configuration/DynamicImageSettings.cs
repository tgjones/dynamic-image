namespace SoundInTheory.DynamicImage.Configuration
{
    public static class DynamicImageSettings
    {
        public static void Init(string serverPath)
        {
            if(string.IsNullOrEmpty(ServerPath))
            {
                _serverPath = serverPath;
            }
        }

        private static string _serverPath;

        public static string ServerPath
        {
            get
            {
                return _serverPath;
            }
        }
    }
}