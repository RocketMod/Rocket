using Rocket.API.Permissions;

namespace Rocket.Core.Permissions
{
    public class PermissionGroup : IPermissionGroup
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
    }
}