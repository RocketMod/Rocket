using System.IO;
using Rocket.API;
using Rocket.API.I18N;

namespace Rocket.Core.I18N
{
    public class Translations : ITranslations
    {
        public string GetLocalizedMessage(string translationKey, params object[] bindings)
        {
            throw new System.NotImplementedException();
        }

        public string SetLocalizedMessage(string translationKey, string message)
        {
            throw new System.NotImplementedException();
        }

        public void Load(IEnvironmentContext context)
        {
            throw new System.NotImplementedException();
        }

        public void Reload()
        {
            throw new System.NotImplementedException();
        }
    }
}