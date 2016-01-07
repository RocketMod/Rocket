namespace Rocket.Core.Assets
{
    public delegate void AssetLoaded<T>(IAsset<T> asset) where T : class;
    public delegate void AssetUnloaded<T>(IAsset<T> asset) where T : class;

    public interface IAsset<T> where T : class
    {
        T Instance { get; set; }
        T Save();
        void Load(AssetLoaded<T> callback = null, bool update = false);
        void Unload(AssetUnloaded<T> callback = null);
    }
}
