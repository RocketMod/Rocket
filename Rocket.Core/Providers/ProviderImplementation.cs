using Rocket.API.Extensions;
using Rocket.API.Providers;
using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using UnityEngine;

namespace Rocket.Core.Providers
{
    [Serializable]
    public class ProviderImplementation
    {
        public ProviderImplementation(Type definition,Type implementation)
        {
            if (!implementation.IsAssignableFrom(definition)) throw new Exception("Implementation does not match the definition");
            Definition = new TypeReference(definition);
            Implementation = new TypeReference(implementation);
        }

        [XmlIgnore]
        public TypeReference Definition { get; }
        public TypeReference Implementation { get; }
        public bool Enabled { get; internal set; } = true;

        [XmlIgnore]
        public ProviderBase Instance { get; internal set; }

        internal void Instanciate()
        {
            Instance = (ProviderBase)FormatterServices.GetUninitializedObject(Implementation.Type);
        }

        internal void Load()
        {

        }

        public void Destroy()
        {
            if (Implementation.Type.IsAssignableFrom(typeof(IDisposable)))
                ((IDisposable)Instance).Dispose();
            Instance = null;
        }

        public override bool Equals(object obj)
        {
            if (obj is ProviderImplementation) return Implementation == ((ProviderImplementation)obj).Implementation;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}