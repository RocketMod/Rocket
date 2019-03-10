using Rocket.API.Permissions;
using Rocket.API.User;

namespace Rocket.Core.Permissions
{
    public static class PermissionExtensions
    {
        public static string GetDisplayName(this IPermissionEntity target)
        {
            if (target is IUser user)
            {
                return user.DisplayName;
            }

            return target.Id;
        }
    }
}