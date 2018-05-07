using System;
using System.Xml.Serialization;

namespace Rocket.Core.Migration.LegacyTranslations
{
    public class Translation
    {
        public string Id { get; set; }

        public string Value { get; set; }
    }
}