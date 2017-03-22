namespace Sitecore.Support.ItemWebApi.Serialization
{
    using Sitecore.Configuration;
    using Sitecore.Diagnostics;
    using Sitecore.ItemWebApi.Serialization;
    using System;
    using System.Globalization;
    using System.Web.Script.Serialization;

    public class JsonSerializer : ISerializer
    {
        public string Serialize(object value)
        {
            int num;
            Assert.ArgumentNotNull(value, "value");
            JavaScriptSerializer serializer = new JavaScriptSerializer
            {
                MaxJsonLength = 0x200000
            };
            string setting = Settings.GetSetting("JsonSerialization.MaxLength");
            if ((!string.IsNullOrEmpty(setting) && !serializer.MaxJsonLength.ToString(CultureInfo.InvariantCulture).Equals(setting, StringComparison.InvariantCultureIgnoreCase)) && int.TryParse(setting, out num))
            {
                serializer.MaxJsonLength = num;
            }
            return serializer.Serialize(value);
        }

        public string SerializedDataMediaType =>
            "application/json";
    }
}
