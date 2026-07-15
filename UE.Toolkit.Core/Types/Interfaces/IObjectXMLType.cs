namespace UE.Toolkit.Core.Types.Interfaces;

public interface IObjectXMLType
{
    public bool IsEnumType { get; }
    
    public bool IsPrimitiveType { get; }
    
    public string TypeName { get; }
}