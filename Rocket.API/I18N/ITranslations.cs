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

        /// <summary>
        /// Load the translation
        /// </summary>
        /// <param name="stream"></param>
        void Load(Stream stream);

        /// <summary>
        /// Save the translation
        /// </summary>
        /// <param name="stream"></param>
        void Save(Stream stream);
    }
}