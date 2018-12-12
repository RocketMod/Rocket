using System;
using Rocket.API.Permissions;

namespace Rocket.Core.Permissions
{
    public class PermissionGroup : IPermissionGroup
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public int Priority { get; set; }

        public bool AutoAssign { get; set; }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
                return Name.ToString(formatProvider);

            if (format.Equals("name", StringComparison.OrdinalIgnoreCase))
                return Name.ToString(formatProvider);

            if (format.Equals("id", StringComparison.OrdinalIgnoreCase))
                return Id.ToString(formatProvider);

            throw new FormatException($"Unknown format for permission groups: {format}");
        }
    }
}