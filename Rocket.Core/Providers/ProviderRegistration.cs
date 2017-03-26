using Rocket.API.Extensions;
using Rocket.API.Providers;
using Rocket.Core.Managers;
using System;
using System.Reflection;
using System.Xml.Serialization;
using UnityEngine;

namespace Rocket.Core.Providers
{
    [Serializable]
    public class ProviderRegistration
    {
        public ProviderRegistration() { }

        public ProviderRegistration(string providerType,Type provider) {
            ProviderType = providerType;
            Provider = new TypeReference(provider);
        }

        public string ProviderType { get; private set; }
        public TypeReference Provider { get; }
        public bool Enabled { get; private set; } = false;


        public void Load()
        {
            if (!Enabled)
            {
                if (Provider.Type.IsAssignableFrom(typeof(MonoBehaviour)))
                    Implementation = (IRocketProviderBase)RocketProviderManager.providersGameObject.TryAddComponent(Provider.Type);
                else
                    Implementation = (IRocketProviderBase)Activator.CreateInstance(Provider.Type);
                Enabled = true;
            }
        }

        public void Unload()
        {
            if (Enabled)
            {
                if (Provider.Type.IsAssignableFrom(typeof(MonoBehaviour)))
                    GameObject.Destroy((MonoBehaviour)Implementation);
                Implementation = null;
                Enabled = false;
            }
        }

        [XmlIgnore]
        public IRocketProviderBase Implementation { get; set; }

        public override bool Equals(object obj) {
            if (obj is ProviderRegistration) return this.Provider == ((ProviderRegistration) obj).Provider;
            return base.Equals(obj);
        }
    }

    [Serializable]
    public class TypeReference
    {
        public TypeReference() { }
        public TypeReference(Type type) {
            Type = type;
            AssemblyName = type.Assembly.FullName;
            TypeName = type.FullName;
        }

        public bool Restore() {
            try
            {
                Assembly a = Assembly.Load(AssemblyName);
                Type = a.GetType(TypeName, true);
                return true;
            }
            catch (Exception e) {
                return false;
            }
        }

        [XmlAttribute]
        public string AssemblyName = "";

        [XmlAttribute]
        public string TypeName = "";

        [XmlIgnore]
        public Type Type { get; private set; }

        public override bool Equals(object obj)
        {
            if (obj is TypeReference) return this.AssemblyName == ((TypeReference)obj).AssemblyName && this.TypeName == ((TypeReference)obj).TypeName;
            return base.Equals(obj);
        }
    }

}
