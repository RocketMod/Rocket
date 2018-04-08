using System.IO;

namespace Rocket.API.I18N
{
    public interface ITranslations
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="translationKey"></param>
        /// <param name="bindings"></param>
        /// <returns></returns>
        string GetLocalizedMessage(string translationKey, params object[] bindings);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="translationKey"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        string SetLocalizedMessage(string translationKey, string message);

        void Load(IEnvironmentContext context);

        void Reload();
    }
}