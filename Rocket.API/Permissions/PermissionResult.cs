namespace Rocket.API.Permissions
{
    public class PermissionResult
    {
        public PermissionResult(PermissionResultType result, PermissionPriority priority)
        {
            Result = result;
            Priority = priority;
        }

        public PermissionResultType Result { get; set; }
        public PermissionPriority Priority { get; set; }
    }

    public enum PermissionPriority
    {
        LOWEST,
        LOW,
        NORMAL,
        HIGH,
        HIGHEST
    }

    public enum PermissionResultType
    {
        DENY,
        GRANT,
        DEFAULT
    }
}