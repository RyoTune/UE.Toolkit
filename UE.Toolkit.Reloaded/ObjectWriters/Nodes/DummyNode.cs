using System.Xml;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;

namespace UE.Toolkit.Reloaded.ObjectWriters.Nodes;

public class DummyNode(IFProperty property) : IFieldNode
{
    private IFProperty Property => property;
    
    public void ConsumeNode(XmlReader reader)
    {
        Log.Warning($"Unimplemented XML Node for property type {Property.ClassPrivate.Name}!");
        using var subReader = reader.ReadSubtree();
        subReader.MoveToContent();
        while (subReader.Read()) {}
    }
}