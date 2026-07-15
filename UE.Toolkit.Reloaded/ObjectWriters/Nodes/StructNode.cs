using System.Xml;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;

namespace UE.Toolkit.Reloaded.ObjectWriters.Nodes;

public class StructNode(IFStructProperty property, nint value, NodeFactory factory) : IFieldNode
{
    private IFStructProperty Property => property;
    private nint Value => value;
    private NodeFactory Factory => factory;
    
    public void ConsumeNode(XmlReader reader)
    {
        new StructDocument(Property.NamePrivate, Property.Struct, Value, Factory)
            .ConsumeNode(reader);
    }
}