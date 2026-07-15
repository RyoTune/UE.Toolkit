using System.Xml;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;

namespace UE.Toolkit.Reloaded.ObjectWriters.Nodes;

public class ObjectNode(IFObjectProperty property, nint value, NodeFactory factory) : IFieldNode
{
    private IFObjectProperty Property => property;
    private nint Value => value;
    private NodeFactory Factory => factory;
    
    public void ConsumeNode(XmlReader reader)
    {
        new StructDocument(Property.NamePrivate, Property.PropertyClass, Value, Factory)
            .ConsumeNode(reader);
    }
}