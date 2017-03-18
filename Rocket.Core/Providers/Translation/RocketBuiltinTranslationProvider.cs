using Rocket.API.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.API.Plugins;
using Rocket.API.Assets;
using Rocket.API.Serialisation;

namespace Rocket.Core.Providers.Translation
{
    public class RocketBuiltinTranslationProvider : RocketProviderBase, IRocketTranslationDataProvider
    {
        Translation = new XMLFileAsset<TranslationList>(String.Format(Environment.TranslationFile, Settings.Instance.LanguageCode), new Type[] { typeof(TranslationList), typeof(TranslationListEntry)


        public override void Load()
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void Translate(string key, string language)
        {
            throw new NotImplementedException();
        }

        public void Translate(IRocketPlugin plugin, string key, string language)
        {
            throw new NotImplementedException();
        }

        public override void Unload()
        {
            throw new NotImplementedException();
        }
    }
}
