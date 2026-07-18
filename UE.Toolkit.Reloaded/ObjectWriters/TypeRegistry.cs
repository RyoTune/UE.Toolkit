using System.Diagnostics.CodeAnalysis;
using UE.Toolkit.Interfaces.ObjectWriters;

namespace UE.Toolkit.Reloaded.ObjectWriters;

// !!!! WARNING
// Type providers are no longer required for Object XML to work as of 1.9.0 since the object information is
// retrieved at runtime. This class is now a no-op!
// !!!! WARNING
public class TypeRegistry : ITypeRegistry
{
    private readonly Dictionary<string, ITypeProvider> _providers = [];
    
    public void RegisterProvider(ITypeProvider provider) {}

    public bool TryGetType(string typeName, string? typeHint, string? providerId, [NotNullWhen(true)] out Type? type)
        => throw new Exception("TypeRegistry is no longer used");

    public bool TryGetType(string typeName, string? providerId, [NotNullWhen(true)] out Type? type)
        => throw new Exception("TypeRegistry is no longer used");
}
