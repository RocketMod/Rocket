namespace Rocket.API.User
{
    public enum IdentityType { Player = 0, Group = 1 }

    public interface IIdentity   
    {
        string Id { get; }
        string Name { get; }

        IdentityType Type { get; }
    }
}