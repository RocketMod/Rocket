using System;
using System.Runtime.Serialization;
using Rocket.API.Player;
using Rocket.API.Providers.Permissions;

namespace Rocket.Core.Player
{
    [Serializable]
    [DataContract]
    public class RocketPlayerBase : IRocketPlayer
    {
        [DataMember]
        public string Id { get; private set; }

        [DataMember]
        public string DisplayName { get; private set; }

        [DataMember]
        public bool IsAdmin { get; private set; }

        public RocketPlayerBase(string id, string displayName = null, bool isAdmin = false)
        {
            Id = id;
            DisplayName = displayName ?? id;
            IsAdmin = isAdmin;
        }

        public int CompareTo(object obj)
        {
            return String.Compare(Id, ((IRocketPlayer)obj).Id, StringComparison.Ordinal);
        }

        public virtual void Kick(string message)
        {
            //
        }

        public virtual void Ban(string message, uint duration)
        {
            //
        }

        public virtual bool HasPermission(string permission)
        {
            return R.Providers.GetProvider<IRocketPermissionsDataProvider>().CheckPermission(this, permission).Result == PermissionResultType.GRANT;
        }
    }
}