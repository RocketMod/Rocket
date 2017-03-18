namespace Rocket.Core.Utils.Steam
{
    public static class Steam
    {
        public static bool IsValidCSteamID(string CSteamID)
        {
            ulong id = 0;
            if (ulong.TryParse(CSteamID, out id) && id > 76561197960265728)
            {
                return true;
            }
            return false;
        }
    }
}