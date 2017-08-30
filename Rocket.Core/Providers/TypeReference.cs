using System;
using System.Reflection;
using System.Xml.Serialization;

[Serializable]
public class TypeReference
{
    public TypeReference() { }
    public TypeReference(Type type)
    {
        Type = type;
        AssemblyName = type.Assembly.FullName;
        TypeName = type.FullName;
    }

    public override string ToString()
    {
        return AssemblyName + "," + Type;
    }

    public bool Resolve()
    {
        try
        {
            Assembly a = Assembly.Load(AssemblyName);
            Type = a.GetType(TypeName, true);
            return true;
        }
        catch (Exception)
        {
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

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}