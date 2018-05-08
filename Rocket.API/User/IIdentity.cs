namespace Rocket.API.User
{
    public enum IdentityType { Console = 0, Player = 1, Group = 2, Custom = 3 }

    public interface IIdentity   
    {
        string Id { get; }
        string Name { get; }

        IdentityType Type { get; }
    }
}