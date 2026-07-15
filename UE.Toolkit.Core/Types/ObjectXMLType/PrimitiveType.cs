using UE.Toolkit.Core.Types.Interfaces;

namespace UE.Toolkit.Core.Types.ObjectXMLType;

public class PrimitiveType(string typeName) : IObjectXMLType
{
    public bool IsEnumType => false;
    public bool IsPrimitiveType => true;
    public string TypeName => typeName;
}