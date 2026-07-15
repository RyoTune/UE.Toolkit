using System.Globalization;
using System.Xml;
using UE.Toolkit.Core.Types;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;

namespace UE.Toolkit.Reloaded.ObjectWriters.Nodes;

public static class ObjectXMLFormatProvider
{
    public static IFormatProvider FloatProvider = new CultureInfo("en-US");
}


public abstract class BasePrimitiveNode<TProperty, TValue>(TProperty property, Ptr<TValue> value) : IFieldNode
    where TProperty : IFProperty
    where TValue : unmanaged
{
    protected TProperty Property => property;
    protected Ptr<TValue> Value => value;
    // protected TValue? InitialValue { get; set; }
    
    public void ConsumeNode(XmlReader reader)
    {
        var fieldValue = GetFieldValue(reader);
        SetInitialValue();
        SetField(fieldValue);
        
        reader.Read(); // Consume node.
        Log.Verbose($"{nameof(BasePrimitiveNode<TProperty, TValue>)} || Field '{property.NamePrivate}' node consumed.");
    }
    
    protected string GetFieldValue(XmlReader reader)
    {
        string? baseObjsDir = null; // TODO: Remake this
        // Get content from external text file.
        // Attribute path is relative to the objects' folder.
        if (baseObjsDir != null && reader.GetAttribute("ext-file") is { } extFileRelative)
        {
            var extFile = Path.Join(baseObjsDir, extFileRelative);
            return File.ReadAllText(extFile);
        }

        return reader.GetAttribute("value") ?? reader.ReadElementContentAsString();
    }

    protected abstract void SetField(string text);
    protected abstract void SetInitialValue();
}

public abstract class ValuePrimitiveNode<TProperty, TValue>(TProperty property, Ptr<TValue> value) 
    : BasePrimitiveNode<TProperty, TValue>(property, value)
    where TProperty : IFProperty
    where TValue: unmanaged
{
    protected TValue? InitialValue { get; set; }

    protected override void SetInitialValue()
    {
        unsafe { InitialValue ??= *Value.Value; }
    }
}

public abstract class TextPrimitiveNode<TProperty, TValue>(TProperty property, Ptr<TValue> value) 
    : BasePrimitiveNode<TProperty, TValue>(property, value)
    where TProperty : IFProperty
    where TValue: unmanaged
{
    protected byte[]? InitialValue { get; set; }
}