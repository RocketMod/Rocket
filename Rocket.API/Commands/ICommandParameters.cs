using System.Collections.Generic;

namespace Rocket.API.Commands
{
    public interface ICommandParameters
    {
        string this[int index] { get; }

        int Length { get; }

        T Get<T>(int index);
    }
}