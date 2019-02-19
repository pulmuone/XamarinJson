using System;
using System.Collections.Generic;
using System.Text;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace XamarinJson.Helpers
{
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        public static string AuthToken
        {
            get => AppSettings.GetValueOrDefault(nameof(AuthToken), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(AuthToken), value);
        }
    }
}