using Rocket.API.Assets;
using Rocket.API.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Rocket.API.Serialisation
{
    public static class TranslationListExtension
    {
        public static void AddUnknownEntries(this IAsset<TranslationList> translations, TranslationList defaultTranslations)
        {
            bool hasChanged = false;
            foreach (PropertyListEntry entry in defaultTranslations)
            {
                if (translations.Instance[entry.Key] == null)
                {
                    translations.Instance.Add(entry);
                    hasChanged = true;
                }
            }
            if (hasChanged)
                translations.Save();
        }
    }

    [XmlRoot("Translations")]
    [XmlType(AnonymousType = false, IncludeInSchema = true, TypeName = "Translation")]
    [Serializable]
    public class TranslationList :  PropertyList, IDefaultable
    {
        public string Translate(string translationKey, params object[] placeholder)
        {
            string value = this[translationKey];
            if (String.IsNullOrEmpty(value)) return translationKey;

            if (value.Contains("{") && value.Contains("}") && placeholder != null && placeholder.Length != 0)
            {
                for (int i = 0; i < placeholder.Length; i++)
                {
                    if (placeholder[i] == null) placeholder[i] = "NULL";
                }
                value = String.Format(value, placeholder);
            }
            return value;
        }

        public virtual void LoadDefaults()
        {
        }
    }
}