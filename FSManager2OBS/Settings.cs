using System.Configuration;

namespace FSManager2OBS
{
    internal static class settings
    {
        private static string isStringNull(string value)
        {
            return isStringNull(value, string.Empty);
        }
        private static string isStringNull(string value, string defaultValue)
        {
            if (value == null)
            {
                value = defaultValue;
            }
            return value;
        }

        private static int getIntValue(string setting)
        {
            string value = ConfigurationManager.AppSettings[setting];
            value = isStringNull(value);
            int.TryParse(value, out int v);
            return v;
        }

        private static string getStringValue(string setting)
        {
            return getStringValue(setting, string.Empty);
        }
        private static string getStringValue(string setting, string defaultValue)
        {
            string value = ConfigurationManager.AppSettings[setting];
            value = isStringNull(value, defaultValue);
            return value;
        }

        public static bool FormStarFS_FullScreen
        {
            get
            {
                string value = ConfigurationManager.AppSettings["FormStarFS_FullScreen"];
                value = isStringNull(value);
                return value.ToLower() == "true";
            }
            set
            {
                saveAppSettings("FormStarFS_FullScreen", (value ? "true" : "false"));
            }
        }

        public static int delayOnIce
        {
            get => getIntValue("delayOnIce");
            set => saveAppSettings("delayOnIce", value.ToString());
        }
        public static int delayStarted
        {
            get => getIntValue("delayStarted");
            set => saveAppSettings("delayStarted", value.ToString());
        }
        public static int delayFinished
        {
            get => getIntValue("delayFinished");
            set => saveAppSettings("delayFinished", value.ToString());
        }
        public static int delayScore
        {
            get => getIntValue("delayScore");
            set => saveAppSettings("delayScore", value.ToString());
        }
        public static int delayWarmup
        {
            get => getIntValue("delayWarmup");
            set => saveAppSettings("delayWarmup", value.ToString());
        }
        public static int delayResurface
        {
            get => getIntValue("delayResurface");
            set => saveAppSettings("delayResurface", value.ToString());
        }


        public static int durationOnIce
        {
            get => getIntValue("durationOnIce");
            set => saveAppSettings("durationOnIce", value.ToString());
        }
        public static int durationStarted
        {
            get => getIntValue("durationStarted");
            set => saveAppSettings("durationStarted", value.ToString());
        }
        public static int durationFinished
        {
            get => getIntValue("durationFinished");
            set => saveAppSettings("durationFinished", value.ToString());
        }
        public static int durationScore
        {
            get => getIntValue("durationScore");
            set => saveAppSettings("durationScore", value.ToString());
        }
        public static int durationWarmup
        {
            get => getIntValue("durationWarmup");
            set => saveAppSettings("durationWarmup", value.ToString());
        }
        public static int durationResurface
        {
            get => getIntValue("durationResurface");
            set => saveAppSettings("durationResurface", value.ToString());
        }


        public static string sceneOnIce
        {
            get => getStringValue("sceneOnIce");
            set => saveAppSettings("sceneOnIce", value);
        }
        public static string sceneStarted
        {
            get => getStringValue("sceneStarted");
            set => saveAppSettings("sceneStarted", value);
        }
        public static string sceneFinished
        {
            get => getStringValue("sceneFinished");
            set => saveAppSettings("sceneFinished", value);
        }
        public static string sceneScore
        {
            get => getStringValue("sceneScore");
            set => saveAppSettings("sceneScore", value);
        }
        public static string sceneWarmup
        {
            get => getStringValue("sceneWarmup");
            set => saveAppSettings("sceneWarmup", value);
        }
        public static string sceneResurface
        {
            get => getStringValue("sceneResurface");
            set => saveAppSettings("sceneResurface", value);
        }

        public static string transitionOnIce
        {
            get => getStringValue("transitionOnIce");
            set => saveAppSettings("transitionOnIce", value);
        }
        public static string transitionStarted
        {
            get => getStringValue("transitionStarted");
            set => saveAppSettings("transitionStarted", value);
        }
        public static string transitionFinished
        {
            get => getStringValue("transitionFinished");
            set => saveAppSettings("transitionFinished", value);
        }
        public static string transitionScore
        {
            get => getStringValue("transitionScore");
            set => saveAppSettings("transitionScore", value);
        }
        public static string transitionWarmup
        {
            get => getStringValue("transitionWarmup");
            set => saveAppSettings("transitionWarmup", value);
        }
        public static string transitionResurface
        {
            get => getStringValue("transitionResurface");
            set => saveAppSettings("transitionResurface", value);
        }



        private static void saveAppSettings(string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //Does the key exist?
            var s = config.AppSettings.Settings[key];
            if (s == null)
            {//No, create key
                config.AppSettings.Settings.Add(key, value);
            }
            else
            {//Update key
                s.Value = value;
            }

            config.Save(ConfigurationSaveMode.Minimal);

            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
