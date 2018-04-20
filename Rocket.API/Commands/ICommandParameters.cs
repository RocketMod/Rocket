using System;
using System.Collections.Generic;

namespace Rocket.API.Commands
{
    public interface ICommandParameters
    {
        string this[int index] { get; }

        int Length { get; }

        T Get<T>(int index);

        object Get(int index, Type type);

        T Get<T>(int index, T defaultValue);

        object Get(int index, Type type, object defaultValue);

        bool TryGet<T>(int index, out T value);

        bool TryGet(int index, Type type, out object value);

        string[] ToArray();
    }
}