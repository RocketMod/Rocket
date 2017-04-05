using Rocket.API.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.API.Collections;
using Rocket.API.Providers.Plugins;
using Rocket.API.Serialisation;
using Rocket.Core.Assets;
using Rocket.API.Providers.Translations;

namespace Rocket.Core.Providers.Translation
{
    public class RocketBuiltinTranslationProvider : IRocketTranslationDataProvider
    {
        public void Unload()
        {
            throw new NotImplementedException();
        }

        public void Load(bool isReload = false)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public string Translate(string key, string language, params object[] args)
        {
            throw new NotImplementedException();
        }

        public string Translate(IRocketPlugin plugin, string key, string language, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void RegisterDefaultTranslations(IRocketPlugin plugin, TranslationList defaultTranslations)
        {
            throw new NotImplementedException();
        }

        public void RegisterDefaultTranslations(TranslationList defaultTranslations)
        {
            throw new NotImplementedException();
        }
    }
}
