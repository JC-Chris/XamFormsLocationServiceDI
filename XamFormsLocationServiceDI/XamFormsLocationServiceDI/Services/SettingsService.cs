using System;
using System.Collections.Generic;

namespace XamFormsLocationServiceDI
{
    public interface ISettingsService
    {
        string GetSetting(string key);
    }

    public class DebugSettingsService : ISettingsService
    {
        private Dictionary<string, string> _settings;
        public DebugSettingsService()
        {
            _settings = new Dictionary<string, string>();
            _settings.Add(SettingsConstants.LocationAccuracyKey, "1000");
            _settings.Add(SettingsConstants.LocationDelaysKey, "5,10,15,30,60");
            _settings.Add(SettingsConstants.LocationDistanceTriggerKey, "1000");
        }

        public string GetSetting(string key)
        {
            return _settings[key];
        }
    }
}

