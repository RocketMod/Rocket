using System;
using Newtonsoft.Json.Linq;
using Rocket.API.Configuration;

namespace Rocket.Core.Configuration.Json
{
    public class JsonConfigurationSection : JsonConfiguration, IConfigurationSection
    {
        public JToken Token { get; protected set; }

        public JsonConfigurationSection(JToken token) : base(token as JObject)
        {
            Token = token;
        }

        public string Key => (Token as JProperty)?.Name;
        public string Path => Token.Path;

        public string Value
        {
            get => Token.Value<string>();
            set
            {
                if (!(Token is JProperty))
                    throw new Exception("Can not set value of: " + Token.Path);

                ((JProperty) Token).Value = new JValue(value);
            }
        }
    }
}